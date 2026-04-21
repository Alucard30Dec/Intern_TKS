using BlazorApp1.Domain.Entities;
using BlazorApp1.Infrastructure.Data;
using BlazorApp1.Models.Common;
using BlazorApp1.Models.XuatKho;
using BlazorApp1.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp1.Services;

/// <summary>
/// Xu ly nghiep vu bai 11 den bai 13: quan ly, hieu chinh va xoa mem phieu xuat kho.
/// </summary>
public sealed class XuatKhoService : IXuatKhoService
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
    private readonly ILogger<XuatKhoService> _logger;

    public XuatKhoService(
        IDbContextFactory<AppDbContext> dbContextFactory,
        ILogger<XuatKhoService> logger)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }

    /// <summary>
    /// Lay danh sach phieu xuat kho cho man hinh quan ly.
    /// </summary>
    public async Task<IReadOnlyList<XuatKhoListItemVm>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await dbContext.XuatKhos
            .AsNoTracking()
            .Where(x => x.Is_Active)
            .OrderByDescending(x => x.Ngay_Xuat_Kho)
            .ThenByDescending(x => x.Xuat_Kho_ID)
            .Select(x => new XuatKhoListItemVm
            {
                Xuat_Kho_ID = x.Xuat_Kho_ID,
                So_Phieu_Xuat_Kho = x.So_Phieu_Xuat_Kho,
                Kho_ID = x.Kho_ID,
                Ten_Kho = x.Kho != null ? x.Kho.Ten_Kho : string.Empty,
                Ngay_Xuat_Kho = x.Ngay_Xuat_Kho,
                Ghi_Chu = x.Ghi_Chu,
                So_Dong_Chi_Tiet = x.Xuat_Kho_Raw_Datas.Count(d => d.Is_Active),
                Tong_Tri_Gia = x.Xuat_Kho_Raw_Datas
                    .Where(d => d.Is_Active)
                    .Select(d => (decimal?)(d.SL_Xuat * d.Don_Gia_Xuat))
                    .Sum() ?? 0m,
                Is_Active = x.Is_Active
            })
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Lay chi tiet cua phieu xuat kho theo ID.
    /// </summary>
    public async Task<ServiceResult<IReadOnlyList<XuatKhoDetailListItemVm>>> GetDetailsAsync(
        int xuatKhoId,
        CancellationToken cancellationToken = default)
    {
        if (xuatKhoId <= 0)
        {
            return ServiceResult<IReadOnlyList<XuatKhoDetailListItemVm>>.Fail("ID phiếu xuất không hợp lệ.");
        }

        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var headerExists = await dbContext.XuatKhos.AnyAsync(
                x => x.Xuat_Kho_ID == xuatKhoId && x.Is_Active,
                cancellationToken);
            if (!headerExists)
            {
                return ServiceResult<IReadOnlyList<XuatKhoDetailListItemVm>>.Fail("Không tìm thấy phiếu xuất kho.");
            }

            var details = await dbContext.XuatKhoRawDatas
                .AsNoTracking()
                .Where(x => x.Xuat_Kho_ID == xuatKhoId && x.Is_Active)
                .OrderBy(x => x.San_Pham != null ? x.San_Pham.Ten_San_Pham : string.Empty)
                .ThenBy(x => x.Xuat_Kho_Raw_Data_ID)
                .Select(x => new XuatKhoDetailListItemVm
                {
                    Xuat_Kho_Raw_Data_ID = x.Xuat_Kho_Raw_Data_ID,
                    San_Pham_ID = x.San_Pham_ID,
                    Ma_San_Pham = x.San_Pham != null ? x.San_Pham.Ma_San_Pham : string.Empty,
                    Ten_San_Pham = x.San_Pham != null ? x.San_Pham.Ten_San_Pham : string.Empty,
                    SL_Xuat = x.SL_Xuat,
                    Don_Gia_Xuat = x.Don_Gia_Xuat
                })
                .ToListAsync(cancellationToken);

            return ServiceResult<IReadOnlyList<XuatKhoDetailListItemVm>>.Ok(details);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Get XuatKho details failed. Xuat_Kho_ID={Xuat_Kho_ID}", xuatKhoId);
            return ServiceResult<IReadOnlyList<XuatKhoDetailListItemVm>>.Fail("Không thể tải chi tiết phiếu xuất kho.");
        }
    }

    /// <summary>
    /// Tao moi phieu xuat kho va chi tiet.
    /// </summary>
    public async Task<ServiceResult> CreateAsync(XuatKhoCreateVm model, CancellationToken cancellationToken = default)
    {
        var validation = ValidateBusinessRules(model);
        if (!validation.Success)
        {
            return validation;
        }

        var normalizedSoPhieu = NormalizeCode(model.So_Phieu_Xuat_Kho);
        var normalizedGhiChu = NormalizeNullableText(model.Ghi_Chu);
        var normalizedDetails = NormalizeDetails(model.Chi_Tiets);

        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var duplicatedSoPhieu = await dbContext.XuatKhos.AnyAsync(
                x => x.So_Phieu_Xuat_Kho.ToUpper() == normalizedSoPhieu.ToUpper(),
                cancellationToken);
            if (duplicatedSoPhieu)
            {
                return ServiceResult.Fail("Số phiếu xuất đã tồn tại.");
            }

            if (!await KhoExistsAsync(dbContext, model.Kho_ID, cancellationToken))
            {
                return ServiceResult.Fail("Kho không tồn tại hoặc đã ngưng sử dụng.");
            }

            var productIds = normalizedDetails
                .Select(x => x.San_Pham_ID)
                .Distinct()
                .ToList();

            var availableProductCount = await dbContext.SanPhams.CountAsync(
                x => x.Is_Active && productIds.Contains(x.San_Pham_ID),
                cancellationToken);
            if (availableProductCount != productIds.Count)
            {
                return ServiceResult.Fail("Một hoặc nhiều sản phẩm không tồn tại hoặc đã ngưng sử dụng.");
            }

            var entity = new XuatKho
            {
                So_Phieu_Xuat_Kho = normalizedSoPhieu,
                Kho_ID = model.Kho_ID,
                Ngay_Xuat_Kho = model.Ngay_Xuat_Kho.Date,
                Ghi_Chu = normalizedGhiChu,
                Is_Active = true,
                Xuat_Kho_Raw_Datas = normalizedDetails.Select(x => new XuatKhoRawData
                {
                    San_Pham_ID = x.San_Pham_ID,
                    SL_Xuat = x.SL_Xuat,
                    Don_Gia_Xuat = x.Don_Gia_Xuat,
                    Is_Active = true
                }).ToList()
            };

            dbContext.XuatKhos.Add(entity);
            await dbContext.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok("Thêm phiếu xuất kho thành công.");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Create XuatKho failed. So_Phieu_Xuat_Kho={So_Phieu_Xuat_Kho}", normalizedSoPhieu);
            return ServiceResult.Fail("Không thể thêm phiếu xuất kho. Có thể dữ liệu đã bị trùng.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while creating XuatKho. So_Phieu_Xuat_Kho={So_Phieu_Xuat_Kho}", normalizedSoPhieu);
            return ServiceResult.Fail("Không thể thêm phiếu xuất kho do lỗi hệ thống.");
        }
    }

    /// <summary>
    /// Hieu chinh thong tin phieu xuat kho (phan header - bai 12).
    /// </summary>
    public async Task<ServiceResult> UpdateHeaderAsync(XuatKhoCreateVm model, CancellationToken cancellationToken = default)
    {
        if (model.Xuat_Kho_ID <= 0)
        {
            return ServiceResult.Fail("ID phiếu xuất không hợp lệ.");
        }

        var validation = ValidateHeaderRules(model);
        if (!validation.Success)
        {
            return validation;
        }

        var normalizedSoPhieu = NormalizeCode(model.So_Phieu_Xuat_Kho);
        var normalizedGhiChu = NormalizeNullableText(model.Ghi_Chu);

        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var entity = await dbContext.XuatKhos
                .FirstOrDefaultAsync(
                    x => x.Xuat_Kho_ID == model.Xuat_Kho_ID && x.Is_Active,
                    cancellationToken);
            if (entity is null)
            {
                return ServiceResult.Fail("Không tìm thấy phiếu xuất kho để cập nhật.");
            }

            var duplicatedSoPhieu = await dbContext.XuatKhos.AnyAsync(
                x => x.Xuat_Kho_ID != model.Xuat_Kho_ID
                    && x.So_Phieu_Xuat_Kho.ToUpper() == normalizedSoPhieu.ToUpper(),
                cancellationToken);
            if (duplicatedSoPhieu)
            {
                return ServiceResult.Fail("Số phiếu xuất đã tồn tại.");
            }

            if (!await KhoExistsAsync(dbContext, model.Kho_ID, cancellationToken))
            {
                return ServiceResult.Fail("Kho không tồn tại hoặc đã ngưng sử dụng.");
            }

            entity.So_Phieu_Xuat_Kho = normalizedSoPhieu;
            entity.Kho_ID = model.Kho_ID;
            entity.Ngay_Xuat_Kho = model.Ngay_Xuat_Kho.Date;
            entity.Ghi_Chu = normalizedGhiChu;

            await dbContext.SaveChangesAsync(cancellationToken);
            return ServiceResult.Ok("Hiệu chỉnh thông tin phiếu xuất kho thành công.");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Update XuatKho header failed. Xuat_Kho_ID={Xuat_Kho_ID}", model.Xuat_Kho_ID);
            return ServiceResult.Fail("Không thể hiệu chỉnh thông tin phiếu xuất. Có thể dữ liệu đã bị trùng.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while updating XuatKho header. Xuat_Kho_ID={Xuat_Kho_ID}", model.Xuat_Kho_ID);
            return ServiceResult.Fail("Không thể hiệu chỉnh thông tin phiếu xuất do lỗi hệ thống.");
        }
    }

    /// <summary>
    /// Them moi dong chi tiet cho phieu xuat kho (bai 13).
    /// </summary>
    public async Task<ServiceResult> AddDetailAsync(
        int xuatKhoId,
        XuatKhoDetailCreateVm model,
        CancellationToken cancellationToken = default)
    {
        if (xuatKhoId <= 0)
        {
            return ServiceResult.Fail("ID phiếu xuất không hợp lệ.");
        }

        if (model is null)
        {
            return ServiceResult.Fail("Dữ liệu dòng chi tiết không hợp lệ.");
        }

        if (model.San_Pham_ID <= 0)
        {
            return ServiceResult.Fail("Mã sản phẩm không được để trống.");
        }

        if (model.SL_Xuat <= 0)
        {
            return ServiceResult.Fail("Số lượng xuất phải lớn hơn 0.");
        }

        if (model.Don_Gia_Xuat <= 0)
        {
            return ServiceResult.Fail("Đơn giá xuất phải lớn hơn 0.");
        }

        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var headerExists = await dbContext.XuatKhos.AnyAsync(
                x => x.Xuat_Kho_ID == xuatKhoId && x.Is_Active,
                cancellationToken);
            if (!headerExists)
            {
                return ServiceResult.Fail("Không tìm thấy phiếu xuất kho.");
            }

            var productExists = await dbContext.SanPhams.AnyAsync(
                x => x.San_Pham_ID == model.San_Pham_ID && x.Is_Active,
                cancellationToken);
            if (!productExists)
            {
                return ServiceResult.Fail("Sản phẩm không tồn tại hoặc đã ngưng sử dụng.");
            }

            var duplicatedProduct = await dbContext.XuatKhoRawDatas.AnyAsync(
                x => x.Xuat_Kho_ID == xuatKhoId
                    && x.San_Pham_ID == model.San_Pham_ID
                    && x.Is_Active,
                cancellationToken);
            if (duplicatedProduct)
            {
                return ServiceResult.Fail("Sản phẩm đã tồn tại trong phiếu xuất. Vui lòng sửa dòng hiện có.");
            }

            dbContext.XuatKhoRawDatas.Add(new XuatKhoRawData
            {
                Xuat_Kho_ID = xuatKhoId,
                San_Pham_ID = model.San_Pham_ID,
                SL_Xuat = decimal.Round(model.SL_Xuat, 2, MidpointRounding.AwayFromZero),
                Don_Gia_Xuat = decimal.Round(model.Don_Gia_Xuat, 2, MidpointRounding.AwayFromZero),
                Is_Active = true
            });

            await dbContext.SaveChangesAsync(cancellationToken);
            return ServiceResult.Ok("Thêm dòng chi tiết thành công.");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Add XuatKho detail failed. Xuat_Kho_ID={Xuat_Kho_ID}", xuatKhoId);
            return ServiceResult.Fail("Không thể thêm dòng chi tiết do ràng buộc dữ liệu.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while adding XuatKho detail. Xuat_Kho_ID={Xuat_Kho_ID}", xuatKhoId);
            return ServiceResult.Fail("Không thể thêm dòng chi tiết do lỗi hệ thống.");
        }
    }

    /// <summary>
    /// Cap nhat chi tiet phieu xuat (chi sua so luong, don gia - bai 13).
    /// </summary>
    public async Task<ServiceResult> UpdateDetailAsync(
        XuatKhoDetailUpdateVm model,
        CancellationToken cancellationToken = default)
    {
        if (model is null || model.Xuat_Kho_Raw_Data_ID <= 0)
        {
            return ServiceResult.Fail("ID dòng chi tiết không hợp lệ.");
        }

        if (model.SL_Xuat <= 0)
        {
            return ServiceResult.Fail("Số lượng xuất phải lớn hơn 0.");
        }

        if (model.Don_Gia_Xuat <= 0)
        {
            return ServiceResult.Fail("Đơn giá xuất phải lớn hơn 0.");
        }

        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var line = await dbContext.XuatKhoRawDatas
                .Include(x => x.Xuat_Kho)
                .FirstOrDefaultAsync(
                    x => x.Xuat_Kho_Raw_Data_ID == model.Xuat_Kho_Raw_Data_ID && x.Is_Active,
                    cancellationToken);

            if (line is null)
            {
                return ServiceResult.Fail("Không tìm thấy dòng chi tiết để cập nhật.");
            }

            if (line.Xuat_Kho is null || !line.Xuat_Kho.Is_Active)
            {
                return ServiceResult.Fail("Không thể cập nhật vì phiếu xuất đã bị ngưng sử dụng.");
            }

            line.SL_Xuat = decimal.Round(model.SL_Xuat, 2, MidpointRounding.AwayFromZero);
            line.Don_Gia_Xuat = decimal.Round(model.Don_Gia_Xuat, 2, MidpointRounding.AwayFromZero);

            await dbContext.SaveChangesAsync(cancellationToken);
            return ServiceResult.Ok("Cập nhật dòng chi tiết thành công.");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Update XuatKho detail failed. Xuat_Kho_Raw_Data_ID={Xuat_Kho_Raw_Data_ID}", model.Xuat_Kho_Raw_Data_ID);
            return ServiceResult.Fail("Không thể cập nhật dòng chi tiết do ràng buộc dữ liệu.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while updating XuatKho detail. Xuat_Kho_Raw_Data_ID={Xuat_Kho_Raw_Data_ID}", model.Xuat_Kho_Raw_Data_ID);
            return ServiceResult.Fail("Không thể cập nhật dòng chi tiết do lỗi hệ thống.");
        }
    }

    /// <summary>
    /// Xoa mem dong chi tiet phieu xuat kho (bai 13).
    /// </summary>
    public async Task<ServiceResult> DeleteDetailAsync(int xuatKhoRawDataId, CancellationToken cancellationToken = default)
    {
        if (xuatKhoRawDataId <= 0)
        {
            return ServiceResult.Fail("ID dòng chi tiết không hợp lệ.");
        }

        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var line = await dbContext.XuatKhoRawDatas
                .Include(x => x.Xuat_Kho)
                .FirstOrDefaultAsync(
                    x => x.Xuat_Kho_Raw_Data_ID == xuatKhoRawDataId && x.Is_Active,
                    cancellationToken);

            if (line is null)
            {
                return ServiceResult.Fail("Không tìm thấy dòng chi tiết để xóa.");
            }

            if (line.Xuat_Kho is null || !line.Xuat_Kho.Is_Active)
            {
                return ServiceResult.Fail("Không thể xóa vì phiếu xuất đã bị ngưng sử dụng.");
            }

            line.Is_Active = false;
            await dbContext.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok("Đã xóa dòng chi tiết khỏi danh sách hiển thị.");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Delete XuatKho detail failed. Xuat_Kho_Raw_Data_ID={Xuat_Kho_Raw_Data_ID}", xuatKhoRawDataId);
            return ServiceResult.Fail("Không thể xóa dòng chi tiết do ràng buộc dữ liệu.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while deleting XuatKho detail. Xuat_Kho_Raw_Data_ID={Xuat_Kho_Raw_Data_ID}", xuatKhoRawDataId);
            return ServiceResult.Fail("Không thể xóa dòng chi tiết do lỗi hệ thống.");
        }
    }

    /// <summary>
    /// Xoa mem phieu xuat kho theo ID (an khoi UI, van giu du lieu trong DB).
    /// </summary>
    public async Task<ServiceResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return ServiceResult.Fail("ID không hợp lệ.");
        }

        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var entity = await dbContext.XuatKhos
                .Include(x => x.Xuat_Kho_Raw_Datas)
                .FirstOrDefaultAsync(x => x.Xuat_Kho_ID == id, cancellationToken);

            if (entity is null)
            {
                return ServiceResult.Fail("Không tìm thấy phiếu xuất kho để xóa.");
            }

            if (!entity.Is_Active)
            {
                return ServiceResult.Fail("Phiếu xuất kho này đã được xóa khỏi danh sách hiển thị trước đó.");
            }

            entity.Is_Active = false;
            foreach (var detail in entity.Xuat_Kho_Raw_Datas)
            {
                detail.Is_Active = false;
            }

            await dbContext.SaveChangesAsync(cancellationToken);
            return ServiceResult.Ok("Đã xóa khỏi danh sách hiển thị. Dữ liệu vẫn được lưu trong hệ thống.");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Soft delete XuatKho failed. Xuat_Kho_ID={Xuat_Kho_ID}", id);
            return ServiceResult.Fail("Không thể xóa khỏi danh sách hiển thị vì dữ liệu đang được ràng buộc ở nghiệp vụ khác.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while soft deleting XuatKho. Xuat_Kho_ID={Xuat_Kho_ID}", id);
            return ServiceResult.Fail("Không thể xóa khỏi danh sách hiển thị do lỗi hệ thống.");
        }
    }

    private static ServiceResult ValidateBusinessRules(XuatKhoCreateVm model)
    {
        if (model is null)
        {
            return ServiceResult.Fail("Dữ liệu không hợp lệ.");
        }

        var normalizedSoPhieu = NormalizeCode(model.So_Phieu_Xuat_Kho);
        if (string.IsNullOrWhiteSpace(normalizedSoPhieu))
        {
            return ServiceResult.Fail("Số phiếu xuất không được để trống.");
        }

        if (normalizedSoPhieu.Length > 50)
        {
            return ServiceResult.Fail("Số phiếu xuất tối đa 50 ký tự.");
        }

        if (model.Kho_ID <= 0)
        {
            return ServiceResult.Fail("Kho không được để trống.");
        }

        if (model.Ngay_Xuat_Kho == default)
        {
            return ServiceResult.Fail("Ngày xuất kho không được để trống.");
        }

        var ghiChu = NormalizeNullableText(model.Ghi_Chu);
        if (ghiChu is not null && ghiChu.Length > 255)
        {
            return ServiceResult.Fail("Ghi chú tối đa 255 ký tự.");
        }

        if (model.Chi_Tiets is null || model.Chi_Tiets.Count == 0)
        {
            return ServiceResult.Fail("Phiếu xuất phải có ít nhất 1 dòng chi tiết.");
        }

        var productIds = new HashSet<int>();
        for (var i = 0; i < model.Chi_Tiets.Count; i++)
        {
            var line = model.Chi_Tiets[i];
            var lineNo = i + 1;

            if (line.San_Pham_ID <= 0)
            {
                return ServiceResult.Fail($"Dòng {lineNo}: Sản phẩm không được để trống.");
            }

            if (!productIds.Add(line.San_Pham_ID))
            {
                return ServiceResult.Fail($"Dòng {lineNo}: Sản phẩm đã bị trùng trong cùng phiếu xuất.");
            }

            if (line.SL_Xuat <= 0)
            {
                return ServiceResult.Fail($"Dòng {lineNo}: Số lượng xuất phải lớn hơn 0.");
            }

            if (line.Don_Gia_Xuat <= 0)
            {
                return ServiceResult.Fail($"Dòng {lineNo}: Đơn giá xuất phải lớn hơn 0.");
            }
        }

        return ServiceResult.Ok();
    }

    private static ServiceResult ValidateHeaderRules(XuatKhoCreateVm model)
    {
        if (model is null)
        {
            return ServiceResult.Fail("Dữ liệu không hợp lệ.");
        }

        var normalizedSoPhieu = NormalizeCode(model.So_Phieu_Xuat_Kho);
        if (string.IsNullOrWhiteSpace(normalizedSoPhieu))
        {
            return ServiceResult.Fail("Số phiếu xuất không được để trống.");
        }

        if (normalizedSoPhieu.Length > 50)
        {
            return ServiceResult.Fail("Số phiếu xuất tối đa 50 ký tự.");
        }

        if (model.Kho_ID <= 0)
        {
            return ServiceResult.Fail("Kho không được để trống.");
        }

        if (model.Ngay_Xuat_Kho == default)
        {
            return ServiceResult.Fail("Ngày xuất kho không được để trống.");
        }

        var ghiChu = NormalizeNullableText(model.Ghi_Chu);
        if (ghiChu is not null && ghiChu.Length > 255)
        {
            return ServiceResult.Fail("Ghi chú tối đa 255 ký tự.");
        }

        return ServiceResult.Ok();
    }

    private static List<XuatKhoRawDataUpsertVm> NormalizeDetails(IEnumerable<XuatKhoRawDataUpsertVm> details)
    {
        return details
            .Select(x => new XuatKhoRawDataUpsertVm
            {
                San_Pham_ID = x.San_Pham_ID,
                SL_Xuat = decimal.Round(x.SL_Xuat, 2, MidpointRounding.AwayFromZero),
                Don_Gia_Xuat = decimal.Round(x.Don_Gia_Xuat, 2, MidpointRounding.AwayFromZero)
            })
            .ToList();
    }

    private static Task<bool> KhoExistsAsync(
        AppDbContext dbContext,
        int khoId,
        CancellationToken cancellationToken)
    {
        return dbContext.Khos.AnyAsync(
            x => x.Kho_ID == khoId && x.Is_Active,
            cancellationToken);
    }

    private static string NormalizeCode(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        return value.Trim().ToUpperInvariant();
    }

    private static string? NormalizeNullableText(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value.Trim();
    }
}
