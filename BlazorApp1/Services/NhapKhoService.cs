using BlazorApp1.Domain.Entities;
using BlazorApp1.Infrastructure.Data;
using BlazorApp1.Models.Common;
using BlazorApp1.Models.NhapKho;
using BlazorApp1.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data;

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
                Don_Vi_Tien = x.Don_Vi_Tien,
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
                    Don_Vi_Tien = x.Don_Vi_Tien,
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
        var normalizedDonViTien = NormalizeDonViTien(model.Don_Vi_Tien);
        var normalizedGhiChu = NormalizeNullableText(model.Ghi_Chu);
        var normalizedDetails = NormalizeDetails(model.Chi_Tiets, normalizedDonViTien);

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
                Don_Vi_Tien = normalizedDonViTien,
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
        var normalizedDonViTien = NormalizeDonViTien(model.Don_Vi_Tien);
        var normalizedGhiChu = NormalizeNullableText(model.Ghi_Chu);

        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
            await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

            var entity = await dbContext.NhapKhos
                .Include(x => x.Nhap_Kho_Raw_Datas)
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

            var activeDetails = entity.Nhap_Kho_Raw_Datas
                .Where(x => x.Is_Active)
                .ToList();

            var oldNgayNhap = entity.Ngay_Nhap_Kho.Date;
            var newNgayNhap = model.Ngay_Nhap_Kho.Date;
            var oldKhoId = entity.Kho_ID;
            var newKhoId = model.Kho_ID;

            if (activeDetails.Count > 0)
            {
                if (oldKhoId != newKhoId)
                {
                    var stockValidation = await ValidateNhapMutationStockAsync(
                        dbContext,
                        oldKhoId,
                        activeDetails.Select(x => x.San_Pham_ID).ToList(),
                        excludeNhapKhoId: entity.Nhap_Kho_ID,
                        excludeNhapKhoRawDataId: null,
                        replacementNhapMovements: null,
                        "Không thể hiệu chỉnh phiếu nhập kho",
                        cancellationToken);
                    if (!stockValidation.Success)
                    {
                        return stockValidation;
                    }
                }
                else if (oldNgayNhap != newNgayNhap)
                {
                    var replacementNhapMovements = activeDetails
                        .Select(x => new ReplacementNhapMovement(
                            x.San_Pham_ID,
                            newNgayNhap,
                            x.SL_Nhap,
                            entity.Nhap_Kho_ID,
                            x.Nhap_Kho_Raw_Data_ID))
                        .ToList();

                    var stockValidation = await ValidateNhapMutationStockAsync(
                        dbContext,
                        oldKhoId,
                        activeDetails.Select(x => x.San_Pham_ID).ToList(),
                        excludeNhapKhoId: entity.Nhap_Kho_ID,
                        excludeNhapKhoRawDataId: null,
                        replacementNhapMovements,
                        "Không thể hiệu chỉnh phiếu nhập kho",
                        cancellationToken);
                    if (!stockValidation.Success)
                    {
                        return stockValidation;
                    }
                }
            }

            var currencyChanged = !string.Equals(entity.Don_Vi_Tien, normalizedDonViTien, StringComparison.Ordinal);

            entity.So_Phieu_Nhap_Kho = normalizedSoPhieu;
            entity.Kho_ID = newKhoId;
            entity.NCC_ID = model.NCC_ID;
            entity.Ngay_Nhap_Kho = newNgayNhap;
            entity.Don_Vi_Tien = normalizedDonViTien;
            entity.Ghi_Chu = normalizedGhiChu;

            if (currencyChanged)
            {
                foreach (var detail in entity.Nhap_Kho_Raw_Datas.Where(x => x.Is_Active))
                {
                    detail.Don_Gia_Nhap = DonViTienOptions.RoundAmount(detail.Don_Gia_Nhap, normalizedDonViTien);
                }
            }

            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
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

        if (!IsWholeQuantity(model.SL_Nhap))
        {
            return ServiceResult.Fail("Số lượng nhập phải là số nguyên.");
        }

        if (model.SL_Nhap > BusinessValidationRules.MaxQuantity)
        {
            return ServiceResult.Fail("Số lượng nhập vượt quá giới hạn cho phép của hệ thống.");
        }

        if (model.Don_Gia_Nhap <= 0)
        {
            return ServiceResult.Fail("Đơn giá nhập phải lớn hơn 0.");
        }

        if (model.Don_Gia_Nhap > BusinessValidationRules.MaxAmount)
        {
            return ServiceResult.Fail("Đơn giá nhập vượt quá giới hạn cho phép của hệ thống.");
        }

        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var header = await dbContext.NhapKhos.FirstOrDefaultAsync(
                x => x.Nhap_Kho_ID == nhapKhoId && x.Is_Active,
                cancellationToken);
            if (header is null)
            {
                return ServiceResult.Fail("Không tìm thấy phiếu nhập kho.");
            }

            if (!BusinessValidationRules.HasValidAmountScale(model.Don_Gia_Nhap, header.Don_Vi_Tien))
            {
                return ServiceResult.Fail(BuildAmountScaleMessage("Đơn giá nhập", header.Don_Vi_Tien));
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
                SL_Nhap = NormalizeQuantity(model.SL_Nhap),
                Don_Gia_Nhap = DonViTienOptions.RoundAmount(model.Don_Gia_Nhap, header.Don_Vi_Tien),
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

        if (!IsWholeQuantity(model.SL_Nhap))
        {
            return ServiceResult.Fail("Số lượng nhập phải là số nguyên.");
        }

        if (model.SL_Nhap > BusinessValidationRules.MaxQuantity)
        {
            return ServiceResult.Fail("Số lượng nhập vượt quá giới hạn cho phép của hệ thống.");
        }

        if (model.Don_Gia_Nhap <= 0)
        {
            return ServiceResult.Fail("Đơn giá nhập phải lớn hơn 0.");
        }

        if (model.Don_Gia_Nhap > BusinessValidationRules.MaxAmount)
        {
            return ServiceResult.Fail("Đơn giá nhập vượt quá giới hạn cho phép của hệ thống.");
        }

        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
            await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

            var line = await dbContext.NhapKhoRawDatas
                .Include(x => x.Nhap_Kho)
                .Include(x => x.San_Pham)
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

            if (!BusinessValidationRules.HasValidAmountScale(model.Don_Gia_Nhap, line.Nhap_Kho.Don_Vi_Tien))
            {
                return ServiceResult.Fail(BuildAmountScaleMessage("Đơn giá nhập", line.Nhap_Kho.Don_Vi_Tien));
            }

            var soLuongNhap = NormalizeQuantity(model.SL_Nhap);
            var stockValidation = await ValidateNhapMutationStockAsync(
                dbContext,
                line.Nhap_Kho.Kho_ID,
                [line.San_Pham_ID],
                excludeNhapKhoId: null,
                excludeNhapKhoRawDataId: line.Nhap_Kho_Raw_Data_ID,
                replacementNhapMovements:
                [
                    new ReplacementNhapMovement(
                        line.San_Pham_ID,
                        line.Nhap_Kho.Ngay_Nhap_Kho,
                        soLuongNhap,
                        line.Nhap_Kho_ID,
                        line.Nhap_Kho_Raw_Data_ID)
                ],
                "Không thể cập nhật dòng chi tiết nhập kho",
                cancellationToken);
            if (!stockValidation.Success)
            {
                return stockValidation;
            }

            line.SL_Nhap = soLuongNhap;
            line.Don_Gia_Nhap = DonViTienOptions.RoundAmount(model.Don_Gia_Nhap, line.Nhap_Kho.Don_Vi_Tien);

            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
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
            await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

            var line = await dbContext.NhapKhoRawDatas
                .Include(x => x.Nhap_Kho)
                .Include(x => x.San_Pham)
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

            var activeDetailCount = await dbContext.NhapKhoRawDatas.CountAsync(
                x => x.Nhap_Kho_ID == line.Nhap_Kho_ID && x.Is_Active,
                cancellationToken);
            if (activeDetailCount <= 1)
            {
                return ServiceResult.Fail("Phiếu nhập phải có ít nhất 1 dòng chi tiết, không thể xóa dòng cuối cùng.");
            }

            var stockValidation = await ValidateNhapMutationStockAsync(
                dbContext,
                line.Nhap_Kho.Kho_ID,
                [line.San_Pham_ID],
                excludeNhapKhoId: null,
                excludeNhapKhoRawDataId: line.Nhap_Kho_Raw_Data_ID,
                replacementNhapMovements: null,
                "Không thể xóa dòng chi tiết nhập kho",
                cancellationToken);
            if (!stockValidation.Success)
            {
                return stockValidation;
            }

            line.Is_Active = false;
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

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
            await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

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

            var activeDetails = entity.Nhap_Kho_Raw_Datas
                .Where(x => x.Is_Active)
                .ToList();
            if (activeDetails.Count > 0)
            {
                var stockValidation = await ValidateNhapMutationStockAsync(
                    dbContext,
                    entity.Kho_ID,
                    activeDetails.Select(x => x.San_Pham_ID).ToList(),
                    excludeNhapKhoId: entity.Nhap_Kho_ID,
                    excludeNhapKhoRawDataId: null,
                    replacementNhapMovements: null,
                    "Không thể xóa phiếu nhập kho",
                    cancellationToken);
                if (!stockValidation.Success)
                {
                    return stockValidation;
                }
            }

            entity.Is_Active = false;
            foreach (var detail in entity.Nhap_Kho_Raw_Datas)
            {
                detail.Is_Active = false;
            }

            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return ServiceResult.Ok("Đã xóa khỏi danh sách.");
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

        if (!BusinessValidationRules.IsValidCode(normalizedSoPhieu))
        {
            return ServiceResult.Fail("Số phiếu nhập chỉ gồm chữ in hoa, số và các ký tự . _ / -.");
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

        if (!BusinessValidationRules.IsValidVoucherDate(model.Ngay_Nhap_Kho))
        {
            return ServiceResult.Fail(
                $"Ngày nhập kho phải trong khoảng {BusinessValidationRules.MinVoucherDate:dd/MM/yyyy} đến {BusinessValidationRules.MaxVoucherDate:dd/MM/yyyy}.");
        }

        if (!DonViTienOptions.IsSupported(model.Don_Vi_Tien))
        {
            return ServiceResult.Fail("Đơn vị tiền không hợp lệ.");
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

            if (!IsWholeQuantity(line.SL_Nhap))
            {
                return ServiceResult.Fail($"Dòng {lineNo}: Số lượng nhập phải là số nguyên.");
            }

            if (line.SL_Nhap > BusinessValidationRules.MaxQuantity)
            {
                return ServiceResult.Fail($"Dòng {lineNo}: Số lượng nhập vượt quá giới hạn cho phép của hệ thống.");
            }

            if (line.Don_Gia_Nhap <= 0)
            {
                return ServiceResult.Fail($"Dòng {lineNo}: Đơn giá nhập phải lớn hơn 0.");
            }

            if (line.Don_Gia_Nhap > BusinessValidationRules.MaxAmount)
            {
                return ServiceResult.Fail($"Dòng {lineNo}: Đơn giá nhập vượt quá giới hạn cho phép của hệ thống.");
            }

            if (!BusinessValidationRules.HasValidAmountScale(line.Don_Gia_Nhap, model.Don_Vi_Tien))
            {
                return ServiceResult.Fail($"Dòng {lineNo}: {BuildAmountScaleMessage("Đơn giá nhập", model.Don_Vi_Tien)}");
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

        if (!BusinessValidationRules.IsValidCode(normalizedSoPhieu))
        {
            return ServiceResult.Fail("Số phiếu nhập chỉ gồm chữ in hoa, số và các ký tự . _ / -.");
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

        if (!BusinessValidationRules.IsValidVoucherDate(model.Ngay_Nhap_Kho))
        {
            return ServiceResult.Fail(
                $"Ngày nhập kho phải trong khoảng {BusinessValidationRules.MinVoucherDate:dd/MM/yyyy} đến {BusinessValidationRules.MaxVoucherDate:dd/MM/yyyy}.");
        }

        if (!DonViTienOptions.IsSupported(model.Don_Vi_Tien))
        {
            return ServiceResult.Fail("Đơn vị tiền không hợp lệ.");
        }

        var ghiChu = NormalizeNullableText(model.Ghi_Chu);
        if (ghiChu is not null && ghiChu.Length > 255)
        {
            return ServiceResult.Fail("Ghi chú tối đa 255 ký tự.");
        }

        return ServiceResult.Ok();
    }

    private static List<NhapKhoRawDataUpsertVm> NormalizeDetails(
        IEnumerable<NhapKhoRawDataUpsertVm> details,
        string donViTien)
    {
        return details
            .Select(x => new NhapKhoRawDataUpsertVm
            {
                San_Pham_ID = x.San_Pham_ID,
                SL_Nhap = NormalizeQuantity(x.SL_Nhap),
                Don_Gia_Nhap = DonViTienOptions.RoundAmount(x.Don_Gia_Nhap, donViTien)
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

    private static string NormalizeDonViTien(string? value)
    {
        return DonViTienOptions.Normalize(value);
    }

    private static bool IsWholeQuantity(decimal value)
    {
        return value == decimal.Truncate(value);
    }

    private static decimal NormalizeQuantity(decimal value)
    {
        return decimal.Round(value, 0, MidpointRounding.AwayFromZero);
    }

    private static string BuildAmountScaleMessage(string fieldLabel, string? donViTien)
    {
        return DonViTienOptions.UsesDecimalAmount(donViTien)
            ? $"{fieldLabel} tối đa 2 chữ số thập phân khi đơn vị tiền là USD."
            : $"{fieldLabel} không được có phần thập phân khi đơn vị tiền là VND.";
    }

    private async Task<ServiceResult> ValidateNhapMutationStockAsync(
        AppDbContext dbContext,
        int khoId,
        IReadOnlyCollection<int> sanPhamIds,
        int? excludeNhapKhoId,
        int? excludeNhapKhoRawDataId,
        IReadOnlyCollection<ReplacementNhapMovement>? replacementNhapMovements,
        string operationName,
        CancellationToken cancellationToken)
    {
        var normalizedSanPhamIds = sanPhamIds
            .Distinct()
            .ToList();
        if (normalizedSanPhamIds.Count == 0)
        {
            return ServiceResult.Ok();
        }

        var nhapQuery = dbContext.NhapKhoRawDatas
            .AsNoTracking()
            .Where(x => x.Is_Active
                        && normalizedSanPhamIds.Contains(x.San_Pham_ID)
                        && x.Nhap_Kho != null
                        && x.Nhap_Kho.Is_Active
                        && x.Nhap_Kho.Kho_ID == khoId);

        if (excludeNhapKhoId.HasValue)
        {
            nhapQuery = nhapQuery.Where(x => x.Nhap_Kho_ID != excludeNhapKhoId.Value);
        }

        if (excludeNhapKhoRawDataId.HasValue)
        {
            nhapQuery = nhapQuery.Where(x => x.Nhap_Kho_Raw_Data_ID != excludeNhapKhoRawDataId.Value);
        }

        var nhapMovements = await nhapQuery
            .Select(x => new StockLedgerMovement(
                x.San_Pham_ID,
                x.Nhap_Kho!.Ngay_Nhap_Kho,
                x.SL_Nhap,
                true,
                x.Nhap_Kho_ID,
                x.Nhap_Kho_Raw_Data_ID))
            .ToListAsync(cancellationToken);

        var xuatMovements = await dbContext.XuatKhoRawDatas
            .AsNoTracking()
            .Where(x => x.Is_Active
                        && normalizedSanPhamIds.Contains(x.San_Pham_ID)
                        && x.Xuat_Kho != null
                        && x.Xuat_Kho.Is_Active
                        && x.Xuat_Kho.Kho_ID == khoId)
            .Select(x => new StockLedgerMovement(
                x.San_Pham_ID,
                x.Xuat_Kho!.Ngay_Xuat_Kho,
                -x.SL_Xuat,
                false,
                x.Xuat_Kho_ID,
                x.Xuat_Kho_Raw_Data_ID))
            .ToListAsync(cancellationToken);

        var allMovements = new List<StockLedgerMovement>(nhapMovements.Count + xuatMovements.Count + (replacementNhapMovements?.Count ?? 0));
        allMovements.AddRange(nhapMovements);
        allMovements.AddRange(xuatMovements);

        if (replacementNhapMovements is not null)
        {
            allMovements.AddRange(replacementNhapMovements.Select(x => new StockLedgerMovement(
                x.SanPhamId,
                x.NgayNhap,
                x.SoLuongNhap,
                true,
                x.ChungTuId,
                x.LineId)));
        }

        var failure = FindFirstNegativeStock(allMovements, normalizedSanPhamIds);
        if (failure is null)
        {
            return ServiceResult.Ok();
        }

        var productInfo = await dbContext.SanPhams
            .AsNoTracking()
            .Where(x => x.San_Pham_ID == failure.SanPhamId)
            .Select(x => new
            {
                x.San_Pham_ID,
                x.Ma_San_Pham,
                x.Ten_San_Pham
            })
            .FirstOrDefaultAsync(cancellationToken);

        var tenSanPham = productInfo is null
            ? $"SP-{failure.SanPhamId}"
            : BuildSanPhamDisplayName(productInfo.Ma_San_Pham, productInfo.Ten_San_Pham, productInfo.San_Pham_ID);

        return ServiceResult.Fail(
            $"{operationName} vì sẽ làm âm tồn kho sản phẩm '{tenSanPham}' tại kho đã chọn vào ngày {failure.NgayPhatSinh:dd/MM/yyyy}. Tồn dự kiến sau phát sinh: {DonViTienOptions.FormatQuantity(failure.TonSauPhatSinh)}.");
    }

    private static NegativeStockIssue? FindFirstNegativeStock(
        IEnumerable<StockLedgerMovement> movements,
        IReadOnlyCollection<int> sanPhamIds)
    {
        var grouped = movements
            .GroupBy(x => x.SanPhamId)
            .ToDictionary(x => x.Key, x => x.ToList());

        foreach (var sanPhamId in sanPhamIds.OrderBy(x => x))
        {
            if (!grouped.TryGetValue(sanPhamId, out var sanPhamMovements))
            {
                continue;
            }

            var tonKho = 0m;
            foreach (var movement in sanPhamMovements
                         .OrderBy(x => x.NgayPhatSinh)
                         .ThenBy(x => x.IsNhap ? 0 : 1)
                         .ThenBy(x => x.ChungTuId)
                         .ThenBy(x => x.LineId))
            {
                tonKho += movement.SoLuongBienDong;
                if (tonKho < 0)
                {
                    return new NegativeStockIssue(sanPhamId, movement.NgayPhatSinh, tonKho);
                }
            }
        }

        return null;
    }

    private static string BuildSanPhamDisplayName(string? maSanPham, string? tenSanPham, int sanPhamId)
    {
        if (!string.IsNullOrWhiteSpace(maSanPham) && !string.IsNullOrWhiteSpace(tenSanPham))
        {
            return $"{maSanPham} - {tenSanPham}";
        }

        if (!string.IsNullOrWhiteSpace(maSanPham))
        {
            return maSanPham;
        }

        if (!string.IsNullOrWhiteSpace(tenSanPham))
        {
            return tenSanPham;
        }

        return $"SP-{sanPhamId}";
    }

    private sealed record StockLedgerMovement(
        int SanPhamId,
        DateTime NgayPhatSinh,
        decimal SoLuongBienDong,
        bool IsNhap,
        int ChungTuId,
        int LineId);

    private sealed record ReplacementNhapMovement(
        int SanPhamId,
        DateTime NgayNhap,
        decimal SoLuongNhap,
        int ChungTuId,
        int LineId);

    private sealed record NegativeStockIssue(
        int SanPhamId,
        DateTime NgayPhatSinh,
        decimal TonSauPhatSinh);

    private static string? NormalizeNullableText(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value.Trim();
    }
}
