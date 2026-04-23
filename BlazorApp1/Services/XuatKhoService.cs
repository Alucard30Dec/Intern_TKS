using BlazorApp1.Domain.Entities;
using BlazorApp1.Infrastructure.Data;
using BlazorApp1.Models.Common;
using BlazorApp1.Models.XuatKho;
using BlazorApp1.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data;

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
    public async Task<IReadOnlyList<XuatKhoListItemVm>> GetAllAsync(int? khoId = null, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var query = dbContext.XuatKhos
            .AsNoTracking()
            .Where(x => x.Is_Active);

        if (khoId.HasValue)
        {
            query = query.Where(x => x.Kho_ID == khoId.Value);
        }

        return await query
            .OrderByDescending(x => x.Ngay_Xuat_Kho)
            .ThenByDescending(x => x.Xuat_Kho_ID)
            .Select(x => new XuatKhoListItemVm
            {
                Xuat_Kho_ID = x.Xuat_Kho_ID,
                So_Phieu_Xuat_Kho = x.So_Phieu_Xuat_Kho,
                Kho_ID = x.Kho_ID,
                Ten_Kho = x.Kho != null ? x.Kho.Ten_Kho : string.Empty,
                Ngay_Xuat_Kho = x.Ngay_Xuat_Kho,
                Don_Vi_Tien = x.Don_Vi_Tien,
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
    /// Lay du lieu phuc vu in phieu xuat kho (bai 14).
    /// </summary>
    public async Task<ServiceResult<XuatKhoPrintVm>> GetPrintDataAsync(
        int xuatKhoId,
        CancellationToken cancellationToken = default)
    {
        if (xuatKhoId <= 0)
        {
            return ServiceResult<XuatKhoPrintVm>.Fail("ID phiếu xuất không hợp lệ.");
        }

        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var header = await dbContext.XuatKhos
                .AsNoTracking()
                .Where(x => x.Xuat_Kho_ID == xuatKhoId && x.Is_Active)
                .Select(x => new XuatKhoPrintVm
                {
                    Xuat_Kho_ID = x.Xuat_Kho_ID,
                    So_Phieu_Xuat_Kho = x.So_Phieu_Xuat_Kho,
                    Ngay_Xuat_Kho = x.Ngay_Xuat_Kho,
                    Ten_Kho = x.Kho != null ? x.Kho.Ten_Kho : string.Empty,
                    Don_Vi_Tien = x.Don_Vi_Tien,
                    Ghi_Chu = x.Ghi_Chu
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (header is null)
            {
                return ServiceResult<XuatKhoPrintVm>.Fail("Không tìm thấy phiếu xuất để in.");
            }

            var lines = await dbContext.XuatKhoRawDatas
                .AsNoTracking()
                .Where(x => x.Xuat_Kho_ID == xuatKhoId && x.Is_Active)
                .OrderBy(x => x.Xuat_Kho_Raw_Data_ID)
                .Select(x => new XuatKhoPrintLineVm
                {
                    Ma_San_Pham = x.San_Pham != null ? x.San_Pham.Ma_San_Pham : string.Empty,
                    Ten_San_Pham = x.San_Pham != null ? x.San_Pham.Ten_San_Pham : string.Empty,
                    Ten_Don_Vi_Tinh = x.San_Pham != null && x.San_Pham.Don_Vi_Tinh != null
                        ? x.San_Pham.Don_Vi_Tinh.Ten_Don_Vi_Tinh
                        : string.Empty,
                    SL_Xuat = x.SL_Xuat,
                    Don_Gia_Xuat = x.Don_Gia_Xuat
                })
                .ToListAsync(cancellationToken);

            header.Lines = lines;

            return ServiceResult<XuatKhoPrintVm>.Ok(header);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Get XuatKho print data failed. Xuat_Kho_ID={Xuat_Kho_ID}", xuatKhoId);
            return ServiceResult<XuatKhoPrintVm>.Fail("Không thể tải dữ liệu in phiếu xuất kho.");
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
        var normalizedDonViTien = NormalizeDonViTien(model.Don_Vi_Tien);
        var normalizedGhiChu = NormalizeNullableText(model.Ghi_Chu);
        var normalizedDetails = NormalizeDetails(model.Chi_Tiets, normalizedDonViTien);

        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
            await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

            var activeDuplicatedSoPhieu = await dbContext.XuatKhos.AnyAsync(
                x => x.Is_Active && x.So_Phieu_Xuat_Kho.ToUpper() == normalizedSoPhieu.ToUpper(),
                cancellationToken);
            if (activeDuplicatedSoPhieu)
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

            var stockValidation = await ValidateXuatMutationStockAsync(
                dbContext,
                model.Kho_ID,
                productIds,
                excludeXuatKhoId: null,
                excludeXuatKhoRawDataId: null,
                replacementXuatMovements: normalizedDetails.Select(x => new ReplacementXuatMovement(
                    x.San_Pham_ID,
                    model.Ngay_Xuat_Kho.Date,
                    x.SL_Xuat,
                    int.MaxValue,
                    x.San_Pham_ID)).ToList(),
                operationName: "Không thể thêm phiếu xuất kho",
                cancellationToken);
            if (!stockValidation.Success)
            {
                return stockValidation;
            }

            var inactiveVoucher = await dbContext.XuatKhos
                .Include(x => x.Xuat_Kho_Raw_Datas)
                .FirstOrDefaultAsync(
                    x => !x.Is_Active && x.So_Phieu_Xuat_Kho.ToUpper() == normalizedSoPhieu.ToUpper(),
                    cancellationToken);
            if (inactiveVoucher is not null)
            {
                inactiveVoucher.So_Phieu_Xuat_Kho = normalizedSoPhieu;
                inactiveVoucher.Kho_ID = model.Kho_ID;
                inactiveVoucher.Ngay_Xuat_Kho = model.Ngay_Xuat_Kho.Date;
                inactiveVoucher.Don_Vi_Tien = normalizedDonViTien;
                inactiveVoucher.Ghi_Chu = normalizedGhiChu;
                inactiveVoucher.Is_Active = true;

                foreach (var line in inactiveVoucher.Xuat_Kho_Raw_Datas)
                {
                    line.Is_Active = false;
                }

                dbContext.XuatKhoRawDatas.AddRange(normalizedDetails.Select(x => new XuatKhoRawData
                {
                    Xuat_Kho_ID = inactiveVoucher.Xuat_Kho_ID,
                    San_Pham_ID = x.San_Pham_ID,
                    SL_Xuat = x.SL_Xuat,
                    Don_Gia_Xuat = x.Don_Gia_Xuat,
                    Is_Active = true
                }));

                await dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return ServiceResult.Ok("Khôi phục phiếu xuất kho thành công.");
            }

            var entity = new XuatKho
            {
                So_Phieu_Xuat_Kho = normalizedSoPhieu,
                Kho_ID = model.Kho_ID,
                Ngay_Xuat_Kho = model.Ngay_Xuat_Kho.Date,
                Don_Vi_Tien = normalizedDonViTien,
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
            await transaction.CommitAsync(cancellationToken);

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
        var normalizedDonViTien = NormalizeDonViTien(model.Don_Vi_Tien);
        var normalizedGhiChu = NormalizeNullableText(model.Ghi_Chu);

        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
            await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

            var entity = await dbContext.XuatKhos
                .Include(x => x.Xuat_Kho_Raw_Datas)
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

            var soLuongCanXuatTheoSanPham = entity.Xuat_Kho_Raw_Datas
                .Where(x => x.Is_Active)
                .GroupBy(x => x.San_Pham_ID)
                .Select(x => new
                {
                    San_Pham_ID = x.Key,
                    SoLuong = x.Sum(y => y.SL_Xuat)
                })
                .ToList();

            if (soLuongCanXuatTheoSanPham.Count > 0)
            {
                var replacementXuatMovements = entity.Xuat_Kho_Raw_Datas
                    .Where(x => x.Is_Active)
                    .Select(x => new ReplacementXuatMovement(
                        x.San_Pham_ID,
                        model.Ngay_Xuat_Kho.Date,
                        x.SL_Xuat,
                        entity.Xuat_Kho_ID,
                        x.Xuat_Kho_Raw_Data_ID))
                    .ToList();

                var stockValidation = await ValidateXuatMutationStockAsync(
                    dbContext,
                    model.Kho_ID,
                    soLuongCanXuatTheoSanPham.Select(x => x.San_Pham_ID).ToList(),
                    excludeXuatKhoId: entity.Xuat_Kho_ID,
                    excludeXuatKhoRawDataId: null,
                    replacementXuatMovements,
                    "Không thể hiệu chỉnh phiếu xuất kho",
                    cancellationToken);
                if (!stockValidation.Success)
                {
                    return stockValidation;
                }
            }

            var currencyChanged = !string.Equals(entity.Don_Vi_Tien, normalizedDonViTien, StringComparison.Ordinal);

            entity.So_Phieu_Xuat_Kho = normalizedSoPhieu;
            entity.Kho_ID = model.Kho_ID;
            entity.Ngay_Xuat_Kho = model.Ngay_Xuat_Kho.Date;
            entity.Don_Vi_Tien = normalizedDonViTien;
            entity.Ghi_Chu = normalizedGhiChu;

            if (currencyChanged)
            {
                foreach (var detail in entity.Xuat_Kho_Raw_Datas.Where(x => x.Is_Active))
                {
                    detail.Don_Gia_Xuat = DonViTienOptions.RoundAmount(detail.Don_Gia_Xuat, normalizedDonViTien);
                }
            }

            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
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

        if (!IsWholeQuantity(model.SL_Xuat))
        {
            return ServiceResult.Fail("Số lượng xuất phải là số nguyên.");
        }

        if (model.SL_Xuat > BusinessValidationRules.MaxQuantity)
        {
            return ServiceResult.Fail("Số lượng xuất vượt quá giới hạn cho phép của hệ thống.");
        }

        if (model.Don_Gia_Xuat <= 0)
        {
            return ServiceResult.Fail("Đơn giá xuất phải lớn hơn 0.");
        }

        if (model.Don_Gia_Xuat > BusinessValidationRules.MaxAmount)
        {
            return ServiceResult.Fail("Đơn giá xuất vượt quá giới hạn cho phép của hệ thống.");
        }

        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
            await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

            var header = await dbContext.XuatKhos.FirstOrDefaultAsync(
                x => x.Xuat_Kho_ID == xuatKhoId && x.Is_Active,
                cancellationToken);
            if (header is null)
            {
                return ServiceResult.Fail("Không tìm thấy phiếu xuất kho.");
            }

            if (!BusinessValidationRules.HasValidAmountScale(model.Don_Gia_Xuat, header.Don_Vi_Tien))
            {
                return ServiceResult.Fail(BuildAmountScaleMessage("Đơn giá xuất", header.Don_Vi_Tien));
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

            var inactiveLine = await dbContext.XuatKhoRawDatas.FirstOrDefaultAsync(
                x => x.Xuat_Kho_ID == xuatKhoId
                    && x.San_Pham_ID == model.San_Pham_ID
                    && !x.Is_Active,
                cancellationToken);

            var soLuongXuat = NormalizeQuantity(model.SL_Xuat);
            var stockValidation = await ValidateXuatMutationStockAsync(
                dbContext,
                header.Kho_ID,
                [model.San_Pham_ID],
                excludeXuatKhoId: null,
                excludeXuatKhoRawDataId: null,
                replacementXuatMovements:
                [
                    new ReplacementXuatMovement(
                        model.San_Pham_ID,
                        header.Ngay_Xuat_Kho,
                        soLuongXuat,
                        xuatKhoId,
                        inactiveLine?.Xuat_Kho_Raw_Data_ID ?? int.MaxValue)
                ],
                operationName: "Không thể thêm dòng chi tiết xuất kho",
                cancellationToken);
            if (!stockValidation.Success)
            {
                return stockValidation;
            }

            if (inactiveLine is not null)
            {
                inactiveLine.SL_Xuat = soLuongXuat;
                inactiveLine.Don_Gia_Xuat = DonViTienOptions.RoundAmount(model.Don_Gia_Xuat, header.Don_Vi_Tien);
                inactiveLine.Is_Active = true;

                await dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return ServiceResult.Ok("Khôi phục dòng chi tiết thành công.");
            }

            dbContext.XuatKhoRawDatas.Add(new XuatKhoRawData
            {
                Xuat_Kho_ID = xuatKhoId,
                San_Pham_ID = model.San_Pham_ID,
                SL_Xuat = soLuongXuat,
                Don_Gia_Xuat = DonViTienOptions.RoundAmount(model.Don_Gia_Xuat, header.Don_Vi_Tien),
                Is_Active = true
            });

            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
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

        if (!IsWholeQuantity(model.SL_Xuat))
        {
            return ServiceResult.Fail("Số lượng xuất phải là số nguyên.");
        }

        if (model.SL_Xuat > BusinessValidationRules.MaxQuantity)
        {
            return ServiceResult.Fail("Số lượng xuất vượt quá giới hạn cho phép của hệ thống.");
        }

        if (model.Don_Gia_Xuat <= 0)
        {
            return ServiceResult.Fail("Đơn giá xuất phải lớn hơn 0.");
        }

        if (model.Don_Gia_Xuat > BusinessValidationRules.MaxAmount)
        {
            return ServiceResult.Fail("Đơn giá xuất vượt quá giới hạn cho phép của hệ thống.");
        }

        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
            await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

            var line = await dbContext.XuatKhoRawDatas
                .Include(x => x.Xuat_Kho)
                .Include(x => x.San_Pham)
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

            if (!BusinessValidationRules.HasValidAmountScale(model.Don_Gia_Xuat, line.Xuat_Kho.Don_Vi_Tien))
            {
                return ServiceResult.Fail(BuildAmountScaleMessage("Đơn giá xuất", line.Xuat_Kho.Don_Vi_Tien));
            }

            var soLuongXuat = NormalizeQuantity(model.SL_Xuat);
            var stockValidation = await ValidateXuatMutationStockAsync(
                dbContext,
                line.Xuat_Kho.Kho_ID,
                [line.San_Pham_ID],
                excludeXuatKhoId: null,
                excludeXuatKhoRawDataId: line.Xuat_Kho_Raw_Data_ID,
                replacementXuatMovements:
                [
                    new ReplacementXuatMovement(
                        line.San_Pham_ID,
                        line.Xuat_Kho.Ngay_Xuat_Kho,
                        soLuongXuat,
                        line.Xuat_Kho_ID,
                        line.Xuat_Kho_Raw_Data_ID)
                ],
                operationName: "Không thể cập nhật dòng chi tiết xuất kho",
                cancellationToken);
            if (!stockValidation.Success)
            {
                return stockValidation;
            }

            line.SL_Xuat = soLuongXuat;
            line.Don_Gia_Xuat = DonViTienOptions.RoundAmount(model.Don_Gia_Xuat, line.Xuat_Kho.Don_Vi_Tien);

            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
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

            var activeDetailCount = await dbContext.XuatKhoRawDatas.CountAsync(
                x => x.Xuat_Kho_ID == line.Xuat_Kho_ID && x.Is_Active,
                cancellationToken);
            if (activeDetailCount <= 1)
            {
                return ServiceResult.Fail("Phiếu xuất phải có ít nhất 1 dòng chi tiết, không thể xóa dòng cuối cùng.");
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
            return ServiceResult.Ok("Đã xóa khỏi danh sách.");
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

        if (!BusinessValidationRules.IsValidCode(normalizedSoPhieu))
        {
            return ServiceResult.Fail("Số phiếu xuất chỉ gồm chữ in hoa, số và các ký tự . _ / -.");
        }

        if (model.Kho_ID <= 0)
        {
            return ServiceResult.Fail("Kho không được để trống.");
        }

        if (model.Ngay_Xuat_Kho == default)
        {
            return ServiceResult.Fail("Ngày xuất kho không được để trống.");
        }

        if (!BusinessValidationRules.IsValidVoucherDate(model.Ngay_Xuat_Kho))
        {
            return ServiceResult.Fail(
                $"Ngày xuất kho phải trong khoảng {BusinessValidationRules.MinVoucherDate:dd/MM/yyyy} đến {BusinessValidationRules.MaxVoucherDate:dd/MM/yyyy}.");
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

            if (!IsWholeQuantity(line.SL_Xuat))
            {
                return ServiceResult.Fail($"Dòng {lineNo}: Số lượng xuất phải là số nguyên.");
            }

            if (line.SL_Xuat > BusinessValidationRules.MaxQuantity)
            {
                return ServiceResult.Fail($"Dòng {lineNo}: Số lượng xuất vượt quá giới hạn cho phép của hệ thống.");
            }

            if (line.Don_Gia_Xuat <= 0)
            {
                return ServiceResult.Fail($"Dòng {lineNo}: Đơn giá xuất phải lớn hơn 0.");
            }

            if (line.Don_Gia_Xuat > BusinessValidationRules.MaxAmount)
            {
                return ServiceResult.Fail($"Dòng {lineNo}: Đơn giá xuất vượt quá giới hạn cho phép của hệ thống.");
            }

            if (!BusinessValidationRules.HasValidAmountScale(line.Don_Gia_Xuat, model.Don_Vi_Tien))
            {
                return ServiceResult.Fail($"Dòng {lineNo}: {BuildAmountScaleMessage("Đơn giá xuất", model.Don_Vi_Tien)}");
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

        if (!BusinessValidationRules.IsValidCode(normalizedSoPhieu))
        {
            return ServiceResult.Fail("Số phiếu xuất chỉ gồm chữ in hoa, số và các ký tự . _ / -.");
        }

        if (model.Kho_ID <= 0)
        {
            return ServiceResult.Fail("Kho không được để trống.");
        }

        if (model.Ngay_Xuat_Kho == default)
        {
            return ServiceResult.Fail("Ngày xuất kho không được để trống.");
        }

        if (!BusinessValidationRules.IsValidVoucherDate(model.Ngay_Xuat_Kho))
        {
            return ServiceResult.Fail(
                $"Ngày xuất kho phải trong khoảng {BusinessValidationRules.MinVoucherDate:dd/MM/yyyy} đến {BusinessValidationRules.MaxVoucherDate:dd/MM/yyyy}.");
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

    private static List<XuatKhoRawDataUpsertVm> NormalizeDetails(
        IEnumerable<XuatKhoRawDataUpsertVm> details,
        string donViTien)
    {
        return details
            .Select(x => new XuatKhoRawDataUpsertVm
            {
                San_Pham_ID = x.San_Pham_ID,
                SL_Xuat = NormalizeQuantity(x.SL_Xuat),
                Don_Gia_Xuat = DonViTienOptions.RoundAmount(x.Don_Gia_Xuat, donViTien)
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

    private async Task<ServiceResult> ValidateXuatMutationStockAsync(
        AppDbContext dbContext,
        int khoId,
        IReadOnlyCollection<int> sanPhamIds,
        int? excludeXuatKhoId,
        int? excludeXuatKhoRawDataId,
        IReadOnlyCollection<ReplacementXuatMovement>? replacementXuatMovements,
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

        var nhapMovements = await dbContext.NhapKhoRawDatas
            .AsNoTracking()
            .Where(x => x.Is_Active
                        && normalizedSanPhamIds.Contains(x.San_Pham_ID)
                        && x.Nhap_Kho != null
                        && x.Nhap_Kho.Is_Active
                        && x.Nhap_Kho.Kho_ID == khoId)
            .Select(x => new StockLedgerMovement(
                x.San_Pham_ID,
                x.Nhap_Kho!.Ngay_Nhap_Kho,
                x.SL_Nhap,
                true,
                x.Nhap_Kho_ID,
                x.Nhap_Kho_Raw_Data_ID))
            .ToListAsync(cancellationToken);

        var xuatQuery = dbContext.XuatKhoRawDatas
            .AsNoTracking()
            .Where(x => x.Is_Active
                        && normalizedSanPhamIds.Contains(x.San_Pham_ID)
                        && x.Xuat_Kho != null
                        && x.Xuat_Kho.Is_Active
                        && x.Xuat_Kho.Kho_ID == khoId);

        if (excludeXuatKhoId.HasValue)
        {
            xuatQuery = xuatQuery.Where(x => x.Xuat_Kho_ID != excludeXuatKhoId.Value);
        }

        if (excludeXuatKhoRawDataId.HasValue)
        {
            xuatQuery = xuatQuery.Where(x => x.Xuat_Kho_Raw_Data_ID != excludeXuatKhoRawDataId.Value);
        }

        var xuatMovements = await xuatQuery
            .Select(x => new StockLedgerMovement(
                x.San_Pham_ID,
                x.Xuat_Kho!.Ngay_Xuat_Kho,
                -x.SL_Xuat,
                false,
                x.Xuat_Kho_ID,
                x.Xuat_Kho_Raw_Data_ID))
            .ToListAsync(cancellationToken);

        var allMovements = new List<StockLedgerMovement>(nhapMovements.Count + xuatMovements.Count + (replacementXuatMovements?.Count ?? 0));
        allMovements.AddRange(nhapMovements);
        allMovements.AddRange(xuatMovements);

        if (replacementXuatMovements is not null)
        {
            allMovements.AddRange(replacementXuatMovements.Select(x => new StockLedgerMovement(
                x.SanPhamId,
                x.NgayXuat,
                -x.SoLuongXuat,
                false,
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

    private sealed record ReplacementXuatMovement(
        int SanPhamId,
        DateTime NgayXuat,
        decimal SoLuongXuat,
        int ChungTuId,
        int LineId);

    private sealed record NegativeStockIssue(
        int SanPhamId,
        DateTime NgayPhatSinh,
        decimal TonSauPhatSinh);

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

    private static string? NormalizeNullableText(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value.Trim();
    }
}
