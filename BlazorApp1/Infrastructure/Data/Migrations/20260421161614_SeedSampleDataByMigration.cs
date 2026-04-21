using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorApp1.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedSampleDataByMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                INSERT INTO "tbl_DM_Don_Vi_Tinh" ("Ten_Don_Vi_Tinh", "Ghi_Chu", "Is_Active")
                VALUES
                    ('Cái', 'Đơn vị đếm cơ bản cho sản phẩm rời.', TRUE),
                    ('Bộ', 'Đơn vị cho bộ linh kiện hoặc bộ sản phẩm.', TRUE),
                    ('Chiếc', 'Đơn vị cho thiết bị đơn chiếc.', TRUE),
                    ('Thùng', 'Đơn vị cho vật tư đóng thùng.', TRUE),
                    ('Hộp', 'Đơn vị cho văn phòng phẩm và hàng tiêu hao.', TRUE),
                    ('Mét', 'Đơn vị chiều dài cho vật tư điện.', TRUE),
                    ('Tấm', 'Đơn vị cho gỗ/ván công nghiệp.', TRUE)
                ON CONFLICT DO NOTHING;

                INSERT INTO "tbl_DM_Loai_San_Pham" ("Ma_LSP", "Ten_LSP", "Ghi_Chu", "Is_Active")
                VALUES
                    ('LSP-NOI-THAT', 'Nội thất', 'Nhóm sản phẩm nội thất văn phòng.', TRUE),
                    ('LSP-VPP', 'Văn phòng phẩm', 'Nhóm văn phòng phẩm tiêu hao.', TRUE),
                    ('LSP-DIEN', 'Thiết bị điện', 'Nhóm vật tư và thiết bị điện.', TRUE),
                    ('LSP-MOC', 'Mộc xây dựng', 'Nhóm gỗ, sơn và vật tư mộc.', TRUE),
                    ('LSP-CONG-CU', 'Công cụ dụng cụ', 'Nhóm công cụ phục vụ thi công và bảo trì.', TRUE)
                ON CONFLICT DO NOTHING;

                INSERT INTO "tbl_DM_San_Pham" ("Ma_San_Pham", "Ten_San_Pham", "Loai_San_Pham_ID", "Don_Vi_Tinh_ID", "Ghi_Chu", "Is_Active")
                SELECT
                    src."Ma_San_Pham",
                    src."Ten_San_Pham",
                    lsp."Loai_San_Pham_ID",
                    dvt."Don_Vi_Tinh_ID",
                    src."Ghi_Chu",
                    TRUE
                FROM (
                    VALUES
                        ('SP-SF01', 'Ghế sofa đơn', 'LSP-NOI-THAT', 'Cái', NULL),
                        ('SP-BT02', 'Bàn trà kính', 'LSP-NOI-THAT', 'Cái', NULL),
                        ('SP-TK03', 'Tủ kệ trang trí', 'LSP-NOI-THAT', 'Cái', NULL),
                        ('SP-KS04', 'Kệ sách gỗ', 'LSP-NOI-THAT', 'Cái', NULL),
                        ('SP-BLV05', 'Bàn làm việc', 'LSP-NOI-THAT', 'Cái', NULL),
                        ('SP-GVP06', 'Ghế văn phòng', 'LSP-NOI-THAT', 'Cái', NULL),
                        ('SP-THS07', 'Tủ hồ sơ', 'LSP-NOI-THAT', 'Cái', NULL),
                        ('SP-GIAY-A4', 'Giấy in A4 70gsm', 'LSP-VPP', 'Hộp', NULL),
                        ('SP-BUT-BI', 'Bút bi xanh', 'LSP-VPP', 'Hộp', NULL),
                        ('SP-DEN-LED', 'Đèn LED panel 600x600', 'LSP-DIEN', 'Cái', NULL),
                        ('SP-CB-2P', 'Aptomat 2P 20A', 'LSP-DIEN', 'Cái', NULL),
                        ('SP-DAY-2X1', 'Dây điện 2x1.5', 'LSP-DIEN', 'Mét', NULL),
                        ('SP-VAN-OK', 'Ván ép Okal 18mm', 'LSP-MOC', 'Tấm', NULL),
                        ('SP-SON-TRANG', 'Sơn nước nội thất trắng 18L', 'LSP-MOC', 'Thùng', NULL),
                        ('SP-MAY-KHOAN', 'Máy khoan cầm tay', 'LSP-CONG-CU', 'Cái', NULL),
                        ('SP-MUI-KHOAN', 'Mũi khoan bộ 13 chi tiết', 'LSP-CONG-CU', 'Bộ', NULL),
                        ('SP-GANG-TAY', 'Găng tay bảo hộ', 'LSP-CONG-CU', 'Hộp', NULL)
                ) AS src("Ma_San_Pham", "Ten_San_Pham", "Ma_LSP", "Ten_Don_Vi_Tinh", "Ghi_Chu")
                INNER JOIN "tbl_DM_Loai_San_Pham" lsp ON lsp."Ma_LSP" = src."Ma_LSP"
                INNER JOIN "tbl_DM_Don_Vi_Tinh" dvt ON dvt."Ten_Don_Vi_Tinh" = src."Ten_Don_Vi_Tinh"
                ON CONFLICT DO NOTHING;

                INSERT INTO "tbl_DM_NCC" ("Ma_NCC", "Ten_NCC", "Ghi_Chu", "Is_Active")
                VALUES
                    ('NCC-MINHTHANH', 'Công Ty TNHH Minh Thành', NULL, TRUE),
                    ('NCC-HONGHAI', 'Hồng Hải Furniture Co.,Ltd', NULL, TRUE),
                    ('NCC-ANPHAT', 'Công Ty Cổ Phần An Phát', NULL, TRUE),
                    ('NCC-THAOLINH', 'Công Ty TNHH Thảo Linh', NULL, TRUE),
                    ('NCC-THIENLONG', 'Công Ty TNHH Thiên Long', NULL, TRUE),
                    ('NCC-PANASONICVN', 'Panasonic Việt Nam', NULL, TRUE)
                ON CONFLICT DO NOTHING;

                INSERT INTO "tbl_DM_Kho" ("Ten_Kho", "Ghi_Chu", "Is_Active")
                VALUES
                    ('KNQ Tân Uyên', 'Kho nhập xuất chính.', TRUE),
                    ('Kho Trung Tâm', 'Kho trung chuyển liên vùng.', TRUE),
                    ('Kho Chi Nhánh Q9', 'Kho phục vụ đơn hàng chi nhánh.', TRUE)
                ON CONFLICT DO NOTHING;

                INSERT INTO "tbl_DM_Kho_User" ("Ma_Dang_Nhap", "Kho_ID", "Is_Active")
                SELECT
                    src."Ma_Dang_Nhap",
                    k."Kho_ID",
                    TRUE
                FROM (
                    VALUES
                        ('ADMIN', 'KNQ Tân Uyên'),
                        ('ADMIN', 'Kho Trung Tâm'),
                        ('ADMIN', 'Kho Chi Nhánh Q9'),
                        ('KETOAN1', 'KNQ Tân Uyên'),
                        ('KETOAN1', 'Kho Trung Tâm'),
                        ('THUKHO1', 'KNQ Tân Uyên'),
                        ('THUKHO2', 'Kho Chi Nhánh Q9')
                ) AS src("Ma_Dang_Nhap", "Ten_Kho")
                INNER JOIN "tbl_DM_Kho" k ON k."Ten_Kho" = src."Ten_Kho"
                ON CONFLICT ("Ma_Dang_Nhap", "Kho_ID") DO NOTHING;

                INSERT INTO "tbl_XNK_Nhap_Kho" ("So_Phieu_Nhap_Kho", "Kho_ID", "NCC_ID", "Ngay_Nhap_Kho", "Don_Vi_Tien", "Ghi_Chu", "Is_Active")
                SELECT
                    src."So_Phieu_Nhap_Kho",
                    k."Kho_ID",
                    n."NCC_ID",
                    src."Ngay_Nhap_Kho",
                    src."Don_Vi_Tien",
                    src."Ghi_Chu",
                    TRUE
                FROM (
                    VALUES
                        ('PN-SEED-2025-001', 'KNQ Tân Uyên', 'NCC-HONGHAI', DATE '2025-10-05', 'VND', 'Nhập lô nội thất đợt 1.'),
                        ('PN-SEED-2025-002', 'KNQ Tân Uyên', 'NCC-MINHTHANH', DATE '2025-11-12', 'VND', 'Nhập ghế, tủ hồ sơ và văn phòng phẩm.'),
                        ('PN-SEED-2025-003', 'Kho Trung Tâm', 'NCC-ANPHAT', DATE '2025-12-03', 'VND', 'Nhập vật tư điện cho quý 4.'),
                        ('PN-SEED-2026-001', 'KNQ Tân Uyên', 'NCC-THAOLINH', DATE '2026-01-15', 'USD', 'Nhập công cụ dụng cụ theo giá USD.'),
                        ('PN-SEED-2026-002', 'Kho Trung Tâm', 'NCC-PANASONICVN', DATE '2026-02-10', 'USD', 'Nhập bổ sung thiết bị điện theo hợp đồng USD.'),
                        ('PN-SEED-2026-003', 'Kho Chi Nhánh Q9', 'NCC-MINHTHANH', DATE '2026-02-22', 'VND', 'Nhập vật tư mộc cho chi nhánh.'),
                        ('PN-SEED-2026-004', 'KNQ Tân Uyên', 'NCC-THIENLONG', DATE '2026-03-05', 'VND', 'Nhập thêm văn phòng phẩm quý 1.'),
                        ('PN-SEED-2026-005', 'KNQ Tân Uyên', 'NCC-ANPHAT', DATE '2026-03-25', 'USD', 'Nhập bổ sung công cụ trước kỳ cao điểm.')
                ) AS src("So_Phieu_Nhap_Kho", "Ten_Kho", "Ma_NCC", "Ngay_Nhap_Kho", "Don_Vi_Tien", "Ghi_Chu")
                INNER JOIN "tbl_DM_Kho" k ON k."Ten_Kho" = src."Ten_Kho"
                INNER JOIN "tbl_DM_NCC" n ON n."Ma_NCC" = src."Ma_NCC"
                ON CONFLICT DO NOTHING;

                INSERT INTO "tbl_DM_Nhap_Kho_Raw_Data" ("Nhap_Kho_ID", "San_Pham_ID", "SL_Nhap", "Don_Gia_Nhap", "Is_Active")
                SELECT
                    nk."Nhap_Kho_ID",
                    sp."San_Pham_ID",
                    src."SL_Nhap",
                    src."Don_Gia_Nhap",
                    TRUE
                FROM (
                    VALUES
                        ('PN-SEED-2025-001', 'SP-SF01', 10::numeric(18,0), 8500000::numeric(18,2)),
                        ('PN-SEED-2025-001', 'SP-BT02', 15::numeric(18,0), 4500000::numeric(18,2)),
                        ('PN-SEED-2025-001', 'SP-TK03', 8::numeric(18,0), 12500000::numeric(18,2)),
                        ('PN-SEED-2025-001', 'SP-KS04', 12::numeric(18,0), 6000000::numeric(18,2)),
                        ('PN-SEED-2025-001', 'SP-BLV05', 6::numeric(18,0), 11000000::numeric(18,2)),
                        ('PN-SEED-2025-002', 'SP-GVP06', 20::numeric(18,0), 7000000::numeric(18,2)),
                        ('PN-SEED-2025-002', 'SP-THS07', 10::numeric(18,0), 9500000::numeric(18,2)),
                        ('PN-SEED-2025-002', 'SP-GIAY-A4', 100::numeric(18,0), 68000::numeric(18,2)),
                        ('PN-SEED-2025-002', 'SP-BUT-BI', 60::numeric(18,0), 120000::numeric(18,2)),
                        ('PN-SEED-2025-003', 'SP-DEN-LED', 40::numeric(18,0), 580000::numeric(18,2)),
                        ('PN-SEED-2025-003', 'SP-CB-2P', 60::numeric(18,0), 135000::numeric(18,2)),
                        ('PN-SEED-2025-003', 'SP-DAY-2X1', 2000::numeric(18,0), 14500::numeric(18,2)),
                        ('PN-SEED-2026-001', 'SP-MAY-KHOAN', 25::numeric(18,0), 79.50::numeric(18,2)),
                        ('PN-SEED-2026-001', 'SP-MUI-KHOAN', 30::numeric(18,0), 18.25::numeric(18,2)),
                        ('PN-SEED-2026-001', 'SP-GANG-TAY', 120::numeric(18,0), 4.80::numeric(18,2)),
                        ('PN-SEED-2026-002', 'SP-DEN-LED', 35::numeric(18,0), 22.40::numeric(18,2)),
                        ('PN-SEED-2026-002', 'SP-CB-2P', 50::numeric(18,0), 5.35::numeric(18,2)),
                        ('PN-SEED-2026-003', 'SP-VAN-OK', 300::numeric(18,0), 285000::numeric(18,2)),
                        ('PN-SEED-2026-003', 'SP-SON-TRANG', 45::numeric(18,0), 1650000::numeric(18,2)),
                        ('PN-SEED-2026-004', 'SP-GIAY-A4', 150::numeric(18,0), 70000::numeric(18,2)),
                        ('PN-SEED-2026-004', 'SP-BUT-BI', 100::numeric(18,0), 125000::numeric(18,2)),
                        ('PN-SEED-2026-005', 'SP-MAY-KHOAN', 12::numeric(18,0), 81.75::numeric(18,2)),
                        ('PN-SEED-2026-005', 'SP-GANG-TAY', 80::numeric(18,0), 4.95::numeric(18,2))
                ) AS src("So_Phieu_Nhap_Kho", "Ma_San_Pham", "SL_Nhap", "Don_Gia_Nhap")
                INNER JOIN "tbl_XNK_Nhap_Kho" nk ON nk."So_Phieu_Nhap_Kho" = src."So_Phieu_Nhap_Kho"
                INNER JOIN "tbl_DM_San_Pham" sp ON sp."Ma_San_Pham" = src."Ma_San_Pham"
                WHERE NOT EXISTS (
                    SELECT 1
                    FROM "tbl_DM_Nhap_Kho_Raw_Data" d
                    WHERE d."Nhap_Kho_ID" = nk."Nhap_Kho_ID"
                      AND d."San_Pham_ID" = sp."San_Pham_ID"
                      AND d."Is_Active" = TRUE
                );

                INSERT INTO "tbl_XNK_Xuat_Kho" ("So_Phieu_Xuat_Kho", "Kho_ID", "Ngay_Xuat_Kho", "Don_Vi_Tien", "Ghi_Chu", "Is_Active")
                SELECT
                    src."So_Phieu_Xuat_Kho",
                    k."Kho_ID",
                    src."Ngay_Xuat_Kho",
                    src."Don_Vi_Tien",
                    src."Ghi_Chu",
                    TRUE
                FROM (
                    VALUES
                        ('PX-SEED-2025-001', 'KNQ Tân Uyên', DATE '2025-10-20', 'VND', 'Xuất nội thất cho dự án văn phòng A.'),
                        ('PX-SEED-2025-002', 'KNQ Tân Uyên', DATE '2025-12-01', 'VND', 'Xuất ghế, tủ hồ sơ và VPP cho phòng kế toán.'),
                        ('PX-SEED-2026-001', 'Kho Trung Tâm', DATE '2026-01-08', 'VND', 'Xuất vật tư điện cho công trình B.'),
                        ('PX-SEED-2026-002', 'KNQ Tân Uyên', DATE '2026-02-02', 'USD', 'Xuất công cụ cho đội bảo trì theo đơn USD.'),
                        ('PX-SEED-2026-003', 'Kho Chi Nhánh Q9', DATE '2026-03-01', 'VND', 'Xuất vật tư mộc cho hợp đồng nội thất C.'),
                        ('PX-SEED-2026-004', 'KNQ Tân Uyên', DATE '2026-03-28', 'USD', 'Xuất bổ sung đồ bảo hộ và công cụ.'),
                        ('PX-SEED-2026-005', 'KNQ Tân Uyên', DATE '2026-04-10', 'VND', 'Xuất văn phòng phẩm cho khối back-office.')
                ) AS src("So_Phieu_Xuat_Kho", "Ten_Kho", "Ngay_Xuat_Kho", "Don_Vi_Tien", "Ghi_Chu")
                INNER JOIN "tbl_DM_Kho" k ON k."Ten_Kho" = src."Ten_Kho"
                ON CONFLICT DO NOTHING;

                INSERT INTO "tbl_DM_Xuat_Kho_Raw_Data" ("Xuat_Kho_ID", "San_Pham_ID", "SL_Xuat", "Don_Gia_Xuat", "Is_Active")
                SELECT
                    xk."Xuat_Kho_ID",
                    sp."San_Pham_ID",
                    src."SL_Xuat",
                    src."Don_Gia_Xuat",
                    TRUE
                FROM (
                    VALUES
                        ('PX-SEED-2025-001', 'SP-SF01', 2::numeric(18,0), 9200000::numeric(18,2)),
                        ('PX-SEED-2025-001', 'SP-BT02', 4::numeric(18,0), 5100000::numeric(18,2)),
                        ('PX-SEED-2025-001', 'SP-KS04', 3::numeric(18,0), 6800000::numeric(18,2)),
                        ('PX-SEED-2025-002', 'SP-GVP06', 5::numeric(18,0), 7800000::numeric(18,2)),
                        ('PX-SEED-2025-002', 'SP-THS07', 2::numeric(18,0), 10300000::numeric(18,2)),
                        ('PX-SEED-2025-002', 'SP-GIAY-A4', 40::numeric(18,0), 85000::numeric(18,2)),
                        ('PX-SEED-2026-001', 'SP-DEN-LED', 12::numeric(18,0), 690000::numeric(18,2)),
                        ('PX-SEED-2026-001', 'SP-CB-2P', 20::numeric(18,0), 165000::numeric(18,2)),
                        ('PX-SEED-2026-001', 'SP-DAY-2X1', 600::numeric(18,0), 21000::numeric(18,2)),
                        ('PX-SEED-2026-002', 'SP-MAY-KHOAN', 8::numeric(18,0), 96.20::numeric(18,2)),
                        ('PX-SEED-2026-002', 'SP-MUI-KHOAN', 10::numeric(18,0), 24.40::numeric(18,2)),
                        ('PX-SEED-2026-003', 'SP-VAN-OK', 120::numeric(18,0), 360000::numeric(18,2)),
                        ('PX-SEED-2026-003', 'SP-SON-TRANG', 18::numeric(18,0), 1950000::numeric(18,2)),
                        ('PX-SEED-2026-004', 'SP-GANG-TAY', 70::numeric(18,0), 6.25::numeric(18,2)),
                        ('PX-SEED-2026-004', 'SP-MAY-KHOAN', 5::numeric(18,0), 99.90::numeric(18,2)),
                        ('PX-SEED-2026-005', 'SP-GIAY-A4', 90::numeric(18,0), 89000::numeric(18,2)),
                        ('PX-SEED-2026-005', 'SP-BUT-BI', 40::numeric(18,0), 150000::numeric(18,2))
                ) AS src("So_Phieu_Xuat_Kho", "Ma_San_Pham", "SL_Xuat", "Don_Gia_Xuat")
                INNER JOIN "tbl_XNK_Xuat_Kho" xk ON xk."So_Phieu_Xuat_Kho" = src."So_Phieu_Xuat_Kho"
                INNER JOIN "tbl_DM_San_Pham" sp ON sp."Ma_San_Pham" = src."Ma_San_Pham"
                WHERE NOT EXISTS (
                    SELECT 1
                    FROM "tbl_DM_Xuat_Kho_Raw_Data" d
                    WHERE d."Xuat_Kho_ID" = xk."Xuat_Kho_ID"
                      AND d."San_Pham_ID" = sp."San_Pham_ID"
                      AND d."Is_Active" = TRUE
                );
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DELETE FROM "tbl_DM_Xuat_Kho_Raw_Data" d
                USING "tbl_XNK_Xuat_Kho" x
                WHERE d."Xuat_Kho_ID" = x."Xuat_Kho_ID"
                  AND x."So_Phieu_Xuat_Kho" LIKE 'PX-SEED-%';

                DELETE FROM "tbl_XNK_Xuat_Kho"
                WHERE "So_Phieu_Xuat_Kho" LIKE 'PX-SEED-%';

                DELETE FROM "tbl_DM_Nhap_Kho_Raw_Data" d
                USING "tbl_XNK_Nhap_Kho" n
                WHERE d."Nhap_Kho_ID" = n."Nhap_Kho_ID"
                  AND n."So_Phieu_Nhap_Kho" LIKE 'PN-SEED-%';

                DELETE FROM "tbl_XNK_Nhap_Kho"
                WHERE "So_Phieu_Nhap_Kho" LIKE 'PN-SEED-%';

                DELETE FROM "tbl_DM_Kho_User" ku
                USING "tbl_DM_Kho" k
                WHERE ku."Kho_ID" = k."Kho_ID"
                  AND (
                        (ku."Ma_Dang_Nhap" = 'ADMIN' AND k."Ten_Kho" IN ('KNQ Tân Uyên', 'Kho Trung Tâm', 'Kho Chi Nhánh Q9'))
                     OR (ku."Ma_Dang_Nhap" = 'KETOAN1' AND k."Ten_Kho" IN ('KNQ Tân Uyên', 'Kho Trung Tâm'))
                     OR (ku."Ma_Dang_Nhap" = 'THUKHO1' AND k."Ten_Kho" = 'KNQ Tân Uyên')
                     OR (ku."Ma_Dang_Nhap" = 'THUKHO2' AND k."Ten_Kho" = 'Kho Chi Nhánh Q9')
                  );
                """);
        }
    }
}
