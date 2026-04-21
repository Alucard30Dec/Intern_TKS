using BlazorApp1.Domain.Entities;
using BlazorApp1.Infrastructure.Data;
using BlazorApp1.Models.Common;
using BlazorApp1.Models.NhapKho;
using BlazorApp1.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp1.Services;

/// <summary>
/// Xu ly nghiep vu bai 7 den bai 10: quan ly, hieu chinh, in va xoa mem phieu nhap kho.
/// </summary>
public sealed class NhapKhoService : INhapKhoService
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
    private readonly ILogger<NhapKhoService> _logger;

    public NhapKhoService(
        IDbContextFactory<AppDbContext> dbContextFactory,
        ILogger<NhapKhoService> logger)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }

    /// <summary>
    /// Lay danh sach phieu nhap kho cho man hinh quan ly.
    /// </summary>
    public async Task<IReadOnlyList<NhapKhoListItemVm>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await dbContext.NhapKhos
            .AsNoTracking()
            .Where(x => x.Is_Active)
            .OrderByDescending(x => x.Ngay_Nhap_Kho)
            .ThenByDescending(x => x.Nhap_Kho_ID)
            .Select(x => new NhapKhoListItemVm
            {
                Nhap_Kho_ID = x.Nhap_Kho_ID,
                So_Phieu_Nhap_Kho = x.So_Phieu_Nhap_Kho,
                Kho_ID = x.Kho_ID,
                Ten_Kho = x.Kho != null ? x.Kho.Ten_Kho : string.Empty,
                NCC_ID = x.NCC_ID,
                Ten_NCC = x.NCC != null ? x.NCC.Ten_NCC : string.Empty,
                Ngay_Nhap_Kho = x.Ngay_Nhap_Kho,
                Ghi_Chu = x.Ghi_Chu,
                So_Dong_Chi_Tiet = x.Nhap_Kho_Raw_Datas.Count(d => d.Is_Active),
                Tong_Tri_Gia = x.Nhap_Kho_Raw_Datas
                    .Where(d => d.Is_Active)
                    .Select(d => (decimal?)(d.SL_Nhap * d.Don_Gia_Nhap))
                    .Sum() ?? 0m,
                Is_Active = x.Is_Active
            })
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Lay chi tiet cua phieu nhap kho theo ID.
    /// </summary>
    public async Task<ServiceResult<IReadOnlyList<NhapKhoDetailListItemVm>>> GetDetailsAsync(
        int nhapKhoId,
        CancellationToken cancellationToken = default)
    {
        if (nhapKhoId <= 0)
        {
            return ServiceResult<IReadOnlyList<NhapKhoDetailListItemVm>>.Fail("ID phiếu nhập không hợp lệ.");
        }

        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var headerExists = await dbContext.NhapKhos.AnyAsync(
                x => x.Nhap_Kho_ID == nhapKhoId && x.Is_Active,
                cancellationToken);
            if (!headerExists)
            {
                return ServiceResult<IReadOnlyList<NhapKhoDetailListItemVm>>.Fail("Không tìm thấy phiếu nhập kho.");
            }

            var details = await dbContext.NhapKhoRawDatas
                .AsNoTracking()
                .Where(x => x.Nhap_Kho_ID == nhapKhoId && x.Is_Active)
                .OrderBy(x => x.San_Pham != null ? x.San_Pham.Ten_San_Pham : string.Empty)
                .ThenBy(x => x.Nhap_Kho_Raw_Data_ID)
                .Select(x => new NhapKhoDetailListItemVm
                {
                    Nhap_Kho_Raw_Data_ID = x.Nhap_Kho_Raw_Data_ID,
                    San_Pham_ID = x.San_Pham_ID,
                    Ma_San_Pham = x.San_Pham != null ? x.San_Pham.Ma_San_Pham : string.Empty,
                    Ten_San_Pham = x.San_Pham != null ? x.San_Pham.Ten_San_Pham : string.Empty,
                    SL_Nhap = x.SL_Nhap,
                    Don_Gia_Nhap = x.Don_Gia_Nhap
                })
                .ToListAsync(cancellationToken);

            return ServiceResult<IReadOnlyList<NhapKhoDetailListItemVm>>.Ok(details);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Get NhapKho details failed. Nhap_Kho_ID={Nhap_Kho_ID}", nhapKhoId);
            return ServiceResult<IReadOnlyList<NhapKhoDetailListItemVm>>.Fail("Không thể tải chi tiết phiếu nhập kho.");
        }
    }

    /// <summary>
    /// Lay du lieu phuc vu in phieu nhap kho (bai 10).
    /// </summary>
    public async Task<ServiceResult<NhapKhoPrintVm>> GetPrintDataAsync(
        int nhapKhoId,
        CancellationToken cancellationToken = default)
    {
        if (nhapKhoId <= 0)
        {
            return ServiceResult<NhapKhoPrintVm>.Fail("ID phiếu nhập không hợp lệ.");
        }

        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var header = await dbContext.NhapKhos
                .AsNoTracking()
                .Where(x => x.Nhap_Kho_ID == nhapKhoId && x.Is_Active)
                .Select(x => new NhapKhoPrintVm
                {
                    Nhap_Kho_ID = x.Nhap_Kho_ID,
                    So_Phieu_Nhap_Kho = x.So_Phieu_Nhap_Kho,
                    Ngay_Nhap_Kho = x.Ngay_Nhap_Kho,
                    Ten_Kho = x.Kho != null ? x.Kho.Ten_Kho : string.Empty,
                    Ten_NCC = x.NCC != null ? x.NCC.Ten_NCC : string.Empty,
                    Ghi_Chu = x.Ghi_Chu
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (header is null)
            {
                return ServiceResult<NhapKhoPrintVm>.Fail("Không tìm thấy phiếu nhập để in.");
            }

            var lines = await dbContext.NhapKhoRawDatas
                .AsNoTracking()
                .Where(x => x.Nhap_Kho_ID == nhapKhoId && x.Is_Active)
                .OrderBy(x => x.Nhap_Kho_Raw_Data_ID)
                .Select(x => new NhapKhoPrintLineVm
                {
                    Ma_San_Pham = x.San_Pham != null ? x.San_Pham.Ma_San_Pham : string.Empty,
                    Ten_San_Pham = x.San_Pham != null ? x.San_Pham.Ten_San_Pham : string.Empty,
                    Ten_Don_Vi_Tinh = x.San_Pham != null && x.San_Pham.Don_Vi_Tinh != null
                        ? x.San_Pham.Don_Vi_Tinh.Ten_Don_Vi_Tinh
                        : string.Empty,
                    SL_Nhap = x.SL_Nhap,
                    Don_Gia_Nhap = x.Don_Gia_Nhap
                })
                .ToListAsync(cancellationToken);

            header.Lines = lines;

            return ServiceResult<NhapKhoPrintVm>.Ok(header);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Get NhapKho print data failed. Nhap_Kho_ID={Nhap_Kho_ID}", nhapKhoId);
            return ServiceResult<NhapKhoPrintVm>.Fail("Không thể tải dữ liệu in phiếu nhập kho.");
        }
    }

    /// <summary>
    /// Tao moi phieu nhap kho va chi tiet.
    /// </summary>
    public async Task<ServiceResult> CreateAsync(NhapKhoCreateVm model, CancellationToken cancellationToken = default)
    {
        var validation = ValidateBusinessRules(model);
        if (!validation.Success)
        {
            return validation;
        }

        var normalizedSoPhieu = NormalizeCode(model.So_Phieu_Nhap_Kho);
        var normalizedGhiChu = NormalizeNullableText(model.Ghi_Chu);
        var normalizedDetails = NormalizeDetails(model.Chi_Tiets);

        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var duplicatedSoPhieu = await dbContext.NhapKhos.AnyAsync(
                x => x.So_Phieu_Nhap_Kho.ToUpper() == normalizedSoPhieu.ToUpper(),
                cancellationToken);
            if (duplicatedSoPhieu)
            {
                return ServiceResult.Fail("Số phiếu nhập đã tồn tại.");
            }

            if (!await KhoExistsAsync(dbContext, model.Kho_ID, cancellationToken))
            {
                return ServiceResult.Fail("Kho không tồn tại hoặc đã ngưng sử dụng.");
            }

            if (!await NhaCungCapExistsAsync(dbContext, model.NCC_ID, cancellationToken))
            {
                return ServiceResult.Fail("Nhà cung cấp không tồn tại hoặc đã ngưng sử dụng.");
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

            var entity = new NhapKho
            {
                So_Phieu_Nhap_Kho = normalizedSoPhieu,
                Kho_ID = model.Kho_ID,
                NCC_ID = model.NCC_ID,
                Ngay_Nhap_Kho = model.Ngay_Nhap_Kho.Date,
                Ghi_Chu = normalizedGhiChu,
                Is_Active = true,
                Nhap_Kho_Raw_Datas = normalizedDetails.Select(x => new NhapKhoRawData
                {
                    San_Pham_ID = x.San_Pham_ID,
                    SL_Nhap = x.SL_Nhap,
                    Don_Gia_Nhap = x.Don_Gia_Nhap,
                    Is_Active = true
                }).ToList()
            };

            dbContext.NhapKhos.Add(entity);
            await dbContext.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok("Thêm phiếu nhập kho thành công.");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Create NhapKho failed. So_Phieu_Nhap_Kho={So_Phieu_Nhap_Kho}", normalizedSoPhieu);
            return ServiceResult.Fail("Không thể thêm phiếu nhập kho. Có thể dữ liệu đã bị trùng.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while creating NhapKho. So_Phieu_Nhap_Kho={So_Phieu_Nhap_Kho}", normalizedSoPhieu);
            return ServiceResult.Fail("Không thể thêm phiếu nhập kho do lỗi hệ thống.");
        }
    }

    /// <summary>
    /// Hieu chinh thong tin phieu nhap kho (phan header - bai 8).
    /// </summary>
    public async Task<ServiceResult> UpdateHeaderAsync(NhapKhoCreateVm model, CancellationToken cancellationToken = default)
    {
        if (model.Nhap_Kho_ID <= 0)
        {
            return ServiceResult.Fail("ID phiếu nhập không hợp lệ.");
        }

        var validation = ValidateHeaderRules(model);
        if (!validation.Success)
        {
            return validation;
        }

        var normalizedSoPhieu = NormalizeCode(model.So_Phieu_Nhap_Kho);
        var normalizedGhiChu = NormalizeNullableText(model.Ghi_Chu);

        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var entity = await dbContext.NhapKhos
                .FirstOrDefaultAsync(
                    x => x.Nhap_Kho_ID == model.Nhap_Kho_ID && x.Is_Active,
                    cancellationToken);
            if (entity is null)
            {
                return ServiceResult.Fail("Không tìm thấy phiếu nhập kho để cập nhật.");
            }

            var duplicatedSoPhieu = await dbContext.NhapKhos.AnyAsync(
                x => x.Nhap_Kho_ID != model.Nhap_Kho_ID
                    && x.So_Phieu_Nhap_Kho.ToUpper() == normalizedSoPhieu.ToUpper(),
                cancellationToken);
            if (duplicatedSoPhieu)
            {
                return ServiceResult.Fail("Số phiếu nhập đã tồn tại.");
            }

            if (!await KhoExistsAsync(dbContext, model.Kho_ID, cancellationToken))
            {
                return ServiceResult.Fail("Kho không tồn tại hoặc đã ngưng sử dụng.");
            }

            if (!await NhaCungCapExistsAsync(dbContext, model.NCC_ID, cancellationToken))
            {
                return ServiceResult.Fail("Nhà cung cấp không tồn tại hoặc đã ngưng sử dụng.");
            }

            entity.So_Phieu_Nhap_Kho = normalizedSoPhieu;
            entity.Kho_ID = model.Kho_ID;
            entity.NCC_ID = model.NCC_ID;
            entity.Ngay_Nhap_Kho = model.Ngay_Nhap_Kho.Date;
            entity.Ghi_Chu = normalizedGhiChu;

            await dbContext.SaveChangesAsync(cancellationToken);
            return ServiceResult.Ok("Hiệu chỉnh thông tin phiếu nhập kho thành công.");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Update NhapKho header failed. Nhap_Kho_ID={Nhap_Kho_ID}", model.Nhap_Kho_ID);
            return ServiceResult.Fail("Không thể hiệu chỉnh thông tin phiếu nhập. Có thể dữ liệu đã bị trùng.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while updating NhapKho header. Nhap_Kho_ID={Nhap_Kho_ID}", model.Nhap_Kho_ID);
            return ServiceResult.Fail("Không thể hiệu chỉnh thông tin phiếu nhập do lỗi hệ thống.");
        }
    }

    /// <summary>
    /// Them moi dong chi tiet cho phieu nhap kho (bai 9).
    /// </summary>
    public async Task<ServiceResult> AddDetailAsync(
        int nhapKhoId,
        NhapKhoDetailCreateVm model,
        CancellationToken cancellationToken = default)
    {
        if (nhapKhoId <= 0)
        {
            return ServiceResult.Fail("ID phiếu nhập không hợp lệ.");
        }

        if (model is null)
        {
            return ServiceResult.Fail("Dữ liệu dòng chi tiết không hợp lệ.");
        }

        if (model.San_Pham_ID <= 0)
        {
            return ServiceResult.Fail("Mã sản phẩm không được để trống.");
        }

        if (model.SL_Nhap <= 0)
        {
            return ServiceResult.Fail("Số lượng nhập phải lớn hơn 0.");
        }

        if (model.Don_Gia_Nhap <= 0)
        {
            return ServiceResult.Fail("Đơn giá nhập phải lớn hơn 0.");
        }

        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var headerExists = await dbContext.NhapKhos.AnyAsync(
                x => x.Nhap_Kho_ID == nhapKhoId && x.Is_Active,
                cancellationToken);
            if (!headerExists)
            {
                return ServiceResult.Fail("Không tìm thấy phiếu nhập kho.");
            }

            var productExists = await dbContext.SanPhams.AnyAsync(
                x => x.San_Pham_ID == model.San_Pham_ID && x.Is_Active,
                cancellationToken);
            if (!productExists)
            {
                return ServiceResult.Fail("Sản phẩm không tồn tại hoặc đã ngưng sử dụng.");
            }

            var duplicatedProduct = await dbContext.NhapKhoRawDatas.AnyAsync(
                x => x.Nhap_Kho_ID == nhapKhoId
                    && x.San_Pham_ID == model.San_Pham_ID
                    && x.Is_Active,
                cancellationToken);
            if (duplicatedProduct)
            {
                return ServiceResult.Fail("Sản phẩm đã tồn tại trong phiếu nhập. Vui lòng sửa dòng hiện có.");
            }

            dbContext.NhapKhoRawDatas.Add(new NhapKhoRawData
            {
                Nhap_Kho_ID = nhapKhoId,
                San_Pham_ID = model.San_Pham_ID,
                SL_Nhap = decimal.Round(model.SL_Nhap, 2, MidpointRounding.AwayFromZero),
                Don_Gia_Nhap = decimal.Round(model.Don_Gia_Nhap, 2, MidpointRounding.AwayFromZero),
                Is_Active = true
            });

            await dbContext.SaveChangesAsync(cancellationToken);
            return ServiceResult.Ok("Thêm dòng chi tiết thành công.");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Add NhapKho detail failed. Nhap_Kho_ID={Nhap_Kho_ID}", nhapKhoId);
            return ServiceResult.Fail("Không thể thêm dòng chi tiết do ràng buộc dữ liệu.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while adding NhapKho detail. Nhap_Kho_ID={Nhap_Kho_ID}", nhapKhoId);
            return ServiceResult.Fail("Không thể thêm dòng chi tiết do lỗi hệ thống.");
        }
    }

    /// <summary>
    /// Cap nhat chi tiet phieu nhap (chi sua so luong, don gia - bai 9).
    /// </summary>
    public async Task<ServiceResult> UpdateDetailAsync(
        NhapKhoDetailUpdateVm model,
        CancellationToken cancellationToken = default)
    {
        if (model is null || model.Nhap_Kho_Raw_Data_ID <= 0)
        {
            return ServiceResult.Fail("ID dòng chi tiết không hợp lệ.");
        }

        if (model.SL_Nhap <= 0)
        {
            return ServiceResult.Fail("Số lượng nhập phải lớn hơn 0.");
        }

        if (model.Don_Gia_Nhap <= 0)
        {
            return ServiceResult.Fail("Đơn giá nhập phải lớn hơn 0.");
        }

        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var line = await dbContext.NhapKhoRawDatas
                .Include(x => x.Nhap_Kho)
                .FirstOrDefaultAsync(
                    x => x.Nhap_Kho_Raw_Data_ID == model.Nhap_Kho_Raw_Data_ID && x.Is_Active,
                    cancellationToken);

            if (line is null)
            {
                return ServiceResult.Fail("Không tìm thấy dòng chi tiết để cập nhật.");
            }

            if (line.Nhap_Kho is null || !line.Nhap_Kho.Is_Active)
            {
                return ServiceResult.Fail("Không thể cập nhật vì phiếu nhập đã bị ngưng sử dụng.");
            }

            line.SL_Nhap = decimal.Round(model.SL_Nhap, 2, MidpointRounding.AwayFromZero);
            line.Don_Gia_Nhap = decimal.Round(model.Don_Gia_Nhap, 2, MidpointRounding.AwayFromZero);

            await dbContext.SaveChangesAsync(cancellationToken);
            return ServiceResult.Ok("Cập nhật dòng chi tiết thành công.");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Update NhapKho detail failed. Nhap_Kho_Raw_Data_ID={Nhap_Kho_Raw_Data_ID}", model.Nhap_Kho_Raw_Data_ID);
            return ServiceResult.Fail("Không thể cập nhật dòng chi tiết do ràng buộc dữ liệu.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while updating NhapKho detail. Nhap_Kho_Raw_Data_ID={Nhap_Kho_Raw_Data_ID}", model.Nhap_Kho_Raw_Data_ID);
            return ServiceResult.Fail("Không thể cập nhật dòng chi tiết do lỗi hệ thống.");
        }
    }

    /// <summary>
    /// Xoa mem dong chi tiet phieu nhap kho (bai 9).
    /// </summary>
    public async Task<ServiceResult> DeleteDetailAsync(int nhapKhoRawDataId, CancellationToken cancellationToken = default)
    {
        if (nhapKhoRawDataId <= 0)
        {
            return ServiceResult.Fail("ID dòng chi tiết không hợp lệ.");
        }

        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var line = await dbContext.NhapKhoRawDatas
                .Include(x => x.Nhap_Kho)
                .FirstOrDefaultAsync(
                    x => x.Nhap_Kho_Raw_Data_ID == nhapKhoRawDataId && x.Is_Active,
                    cancellationToken);

            if (line is null)
            {
                return ServiceResult.Fail("Không tìm thấy dòng chi tiết để xóa.");
            }

            if (line.Nhap_Kho is null || !line.Nhap_Kho.Is_Active)
            {
                return ServiceResult.Fail("Không thể xóa vì phiếu nhập đã bị ngưng sử dụng.");
            }

            line.Is_Active = false;
            await dbContext.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok("Đã xóa dòng chi tiết khỏi danh sách hiển thị.");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Delete NhapKho detail failed. Nhap_Kho_Raw_Data_ID={Nhap_Kho_Raw_Data_ID}", nhapKhoRawDataId);
            return ServiceResult.Fail("Không thể xóa dòng chi tiết do ràng buộc dữ liệu.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while deleting NhapKho detail. Nhap_Kho_Raw_Data_ID={Nhap_Kho_Raw_Data_ID}", nhapKhoRawDataId);
            return ServiceResult.Fail("Không thể xóa dòng chi tiết do lỗi hệ thống.");
        }
    }

    /// <summary>
    /// Xoa mem phieu nhap kho theo ID (an khoi UI, van giu du lieu trong DB).
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

            var entity = await dbContext.NhapKhos
                .Include(x => x.Nhap_Kho_Raw_Datas)
                .FirstOrDefaultAsync(x => x.Nhap_Kho_ID == id, cancellationToken);

            if (entity is null)
            {
                return ServiceResult.Fail("Không tìm thấy phiếu nhập kho để xóa.");
            }

            if (!entity.Is_Active)
            {
                return ServiceResult.Fail("Phiếu nhập kho này đã được xóa khỏi danh sách hiển thị trước đó.");
            }

            entity.Is_Active = false;
            foreach (var detail in entity.Nhap_Kho_Raw_Datas)
            {
                detail.Is_Active = false;
            }

            await dbContext.SaveChangesAsync(cancellationToken);
            return ServiceResult.Ok("Đã xóa khỏi danh sách hiển thị. Dữ liệu vẫn được lưu trong hệ thống.");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Soft delete NhapKho failed. Nhap_Kho_ID={Nhap_Kho_ID}", id);
            return ServiceResult.Fail("Không thể xóa khỏi danh sách hiển thị vì dữ liệu đang được ràng buộc ở nghiệp vụ khác.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while soft deleting NhapKho. Nhap_Kho_ID={Nhap_Kho_ID}", id);
            return ServiceResult.Fail("Không thể xóa khỏi danh sách hiển thị do lỗi hệ thống.");
        }
    }

    private static ServiceResult ValidateBusinessRules(NhapKhoCreateVm model)
    {
        if (model is null)
        {
            return ServiceResult.Fail("Dữ liệu không hợp lệ.");
        }

        var normalizedSoPhieu = NormalizeCode(model.So_Phieu_Nhap_Kho);
        if (string.IsNullOrWhiteSpace(normalizedSoPhieu))
        {
            return ServiceResult.Fail("Số phiếu nhập không được để trống.");
        }

        if (normalizedSoPhieu.Length > 50)
        {
            return ServiceResult.Fail("Số phiếu nhập tối đa 50 ký tự.");
        }

        if (model.Kho_ID <= 0)
        {
            return ServiceResult.Fail("Kho không được để trống.");
        }

        if (model.NCC_ID <= 0)
        {
            return ServiceResult.Fail("Nhà cung cấp không được để trống.");
        }

        if (model.Ngay_Nhap_Kho == default)
        {
            return ServiceResult.Fail("Ngày nhập kho không được để trống.");
        }

        var ghiChu = NormalizeNullableText(model.Ghi_Chu);
        if (ghiChu is not null && ghiChu.Length > 255)
        {
            return ServiceResult.Fail("Ghi chú tối đa 255 ký tự.");
        }

        if (model.Chi_Tiets is null || model.Chi_Tiets.Count == 0)
        {
            return ServiceResult.Fail("Phiếu nhập phải có ít nhất 1 dòng chi tiết.");
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
                return ServiceResult.Fail($"Dòng {lineNo}: Sản phẩm đã bị trùng trong cùng phiếu nhập.");
            }

            if (line.SL_Nhap <= 0)
            {
                return ServiceResult.Fail($"Dòng {lineNo}: Số lượng nhập phải lớn hơn 0.");
            }

            if (line.Don_Gia_Nhap <= 0)
            {
                return ServiceResult.Fail($"Dòng {lineNo}: Đơn giá nhập phải lớn hơn 0.");
            }
        }

        return ServiceResult.Ok();
    }

    private static ServiceResult ValidateHeaderRules(NhapKhoCreateVm model)
    {
        if (model is null)
        {
            return ServiceResult.Fail("Dữ liệu không hợp lệ.");
        }

        var normalizedSoPhieu = NormalizeCode(model.So_Phieu_Nhap_Kho);
        if (string.IsNullOrWhiteSpace(normalizedSoPhieu))
        {
            return ServiceResult.Fail("Số phiếu nhập không được để trống.");
        }

        if (normalizedSoPhieu.Length > 50)
        {
            return ServiceResult.Fail("Số phiếu nhập tối đa 50 ký tự.");
        }

        if (model.Kho_ID <= 0)
        {
            return ServiceResult.Fail("Kho không được để trống.");
        }

        if (model.NCC_ID <= 0)
        {
            return ServiceResult.Fail("Nhà cung cấp không được để trống.");
        }

        if (model.Ngay_Nhap_Kho == default)
        {
            return ServiceResult.Fail("Ngày nhập kho không được để trống.");
        }

        var ghiChu = NormalizeNullableText(model.Ghi_Chu);
        if (ghiChu is not null && ghiChu.Length > 255)
        {
            return ServiceResult.Fail("Ghi chú tối đa 255 ký tự.");
        }

        return ServiceResult.Ok();
    }

    private static List<NhapKhoRawDataUpsertVm> NormalizeDetails(IEnumerable<NhapKhoRawDataUpsertVm> details)
    {
        return details
            .Select(x => new NhapKhoRawDataUpsertVm
            {
                San_Pham_ID = x.San_Pham_ID,
                SL_Nhap = decimal.Round(x.SL_Nhap, 2, MidpointRounding.AwayFromZero),
                Don_Gia_Nhap = decimal.Round(x.Don_Gia_Nhap, 2, MidpointRounding.AwayFromZero)
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

    private static Task<bool> NhaCungCapExistsAsync(
        AppDbContext dbContext,
        int nccId,
        CancellationToken cancellationToken)
    {
        return dbContext.NhaCungCaps.AnyAsync(
            x => x.NCC_ID == nccId && x.Is_Active,
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
