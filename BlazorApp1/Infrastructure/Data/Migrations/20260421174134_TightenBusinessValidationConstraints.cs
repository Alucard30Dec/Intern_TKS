using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorApp1.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class TightenBusinessValidationConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE "tbl_XNK_Nhap_Kho"
                SET "Don_Vi_Tien" = UPPER(BTRIM("Don_Vi_Tien"))
                WHERE "Don_Vi_Tien" IS NOT NULL;

                UPDATE "tbl_XNK_Nhap_Kho"
                SET "Don_Vi_Tien" = 'VND'
                WHERE "Don_Vi_Tien" IS NULL
                   OR BTRIM("Don_Vi_Tien") = ''
                   OR "Don_Vi_Tien" NOT IN ('VND', 'USD');

                UPDATE "tbl_XNK_Nhap_Kho"
                SET "Ngay_Nhap_Kho" = DATE '2000-01-01'
                WHERE "Ngay_Nhap_Kho" < DATE '2000-01-01';
                """);

            migrationBuilder.Sql("""
                UPDATE "tbl_XNK_Xuat_Kho"
                SET "Don_Vi_Tien" = UPPER(BTRIM("Don_Vi_Tien"))
                WHERE "Don_Vi_Tien" IS NOT NULL;

                UPDATE "tbl_XNK_Xuat_Kho"
                SET "Don_Vi_Tien" = 'VND'
                WHERE "Don_Vi_Tien" IS NULL
                   OR BTRIM("Don_Vi_Tien") = ''
                   OR "Don_Vi_Tien" NOT IN ('VND', 'USD');

                UPDATE "tbl_XNK_Xuat_Kho"
                SET "Ngay_Xuat_Kho" = DATE '2000-01-01'
                WHERE "Ngay_Xuat_Kho" < DATE '2000-01-01';
                """);

            migrationBuilder.Sql("""
                UPDATE "tbl_DM_Nhap_Kho_Raw_Data"
                SET "Is_Active" = FALSE
                WHERE "Is_Active" = TRUE
                  AND ("SL_Nhap" <= 0 OR "Don_Gia_Nhap" <= 0);

                WITH duplicated AS (
                    SELECT "Nhap_Kho_Raw_Data_ID",
                           ROW_NUMBER() OVER (
                               PARTITION BY "Nhap_Kho_ID", "San_Pham_ID"
                               ORDER BY "Nhap_Kho_Raw_Data_ID"
                           ) AS rn
                    FROM "tbl_DM_Nhap_Kho_Raw_Data"
                    WHERE "Is_Active" = TRUE
                )
                UPDATE "tbl_DM_Nhap_Kho_Raw_Data" d
                SET "Is_Active" = FALSE
                FROM duplicated x
                WHERE d."Nhap_Kho_Raw_Data_ID" = x."Nhap_Kho_Raw_Data_ID"
                  AND x.rn > 1;
                """);

            migrationBuilder.Sql("""
                UPDATE "tbl_DM_Xuat_Kho_Raw_Data"
                SET "Is_Active" = FALSE
                WHERE "Is_Active" = TRUE
                  AND ("SL_Xuat" <= 0 OR "Don_Gia_Xuat" <= 0);

                WITH duplicated AS (
                    SELECT "Xuat_Kho_Raw_Data_ID",
                           ROW_NUMBER() OVER (
                               PARTITION BY "Xuat_Kho_ID", "San_Pham_ID"
                               ORDER BY "Xuat_Kho_Raw_Data_ID"
                           ) AS rn
                    FROM "tbl_DM_Xuat_Kho_Raw_Data"
                    WHERE "Is_Active" = TRUE
                )
                UPDATE "tbl_DM_Xuat_Kho_Raw_Data" d
                SET "Is_Active" = FALSE
                FROM duplicated x
                WHERE d."Xuat_Kho_Raw_Data_ID" = x."Xuat_Kho_Raw_Data_ID"
                  AND x.rn > 1;
                """);

            migrationBuilder.AddCheckConstraint(
                name: "CK_tbl_XNK_Xuat_Kho_Don_Vi_Tien_Valid",
                table: "tbl_XNK_Xuat_Kho",
                sql: "\"Don_Vi_Tien\" IN ('VND','USD')");

            migrationBuilder.AddCheckConstraint(
                name: "CK_tbl_XNK_Xuat_Kho_Ngay_Xuat_Kho_MinDate",
                table: "tbl_XNK_Xuat_Kho",
                sql: "\"Ngay_Xuat_Kho\" >= DATE '2000-01-01'");

            migrationBuilder.AddCheckConstraint(
                name: "CK_tbl_XNK_Nhap_Kho_Don_Vi_Tien_Valid",
                table: "tbl_XNK_Nhap_Kho",
                sql: "\"Don_Vi_Tien\" IN ('VND','USD')");

            migrationBuilder.AddCheckConstraint(
                name: "CK_tbl_XNK_Nhap_Kho_Ngay_Nhap_Kho_MinDate",
                table: "tbl_XNK_Nhap_Kho",
                sql: "\"Ngay_Nhap_Kho\" >= DATE '2000-01-01'");

            migrationBuilder.CreateIndex(
                name: "UQ_tbl_DM_Xuat_Kho_Raw_Data_Xuat_Kho_ID_San_Pham_ID_Active",
                table: "tbl_DM_Xuat_Kho_Raw_Data",
                columns: new[] { "Xuat_Kho_ID", "San_Pham_ID", "Is_Active" },
                unique: true,
                filter: "\"Is_Active\" = TRUE");

            migrationBuilder.AddCheckConstraint(
                name: "CK_tbl_DM_Xuat_Kho_Raw_Data_Don_Gia_Xuat_Positive",
                table: "tbl_DM_Xuat_Kho_Raw_Data",
                sql: "\"Don_Gia_Xuat\" > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_tbl_DM_Xuat_Kho_Raw_Data_SL_Xuat_Positive",
                table: "tbl_DM_Xuat_Kho_Raw_Data",
                sql: "\"SL_Xuat\" > 0");

            migrationBuilder.CreateIndex(
                name: "UQ_tbl_DM_Nhap_Kho_Raw_Data_Nhap_Kho_ID_San_Pham_ID_Active",
                table: "tbl_DM_Nhap_Kho_Raw_Data",
                columns: new[] { "Nhap_Kho_ID", "San_Pham_ID", "Is_Active" },
                unique: true,
                filter: "\"Is_Active\" = TRUE");

            migrationBuilder.AddCheckConstraint(
                name: "CK_tbl_DM_Nhap_Kho_Raw_Data_Don_Gia_Nhap_Positive",
                table: "tbl_DM_Nhap_Kho_Raw_Data",
                sql: "\"Don_Gia_Nhap\" > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_tbl_DM_Nhap_Kho_Raw_Data_SL_Nhap_Positive",
                table: "tbl_DM_Nhap_Kho_Raw_Data",
                sql: "\"SL_Nhap\" > 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_tbl_XNK_Xuat_Kho_Don_Vi_Tien_Valid",
                table: "tbl_XNK_Xuat_Kho");

            migrationBuilder.DropCheckConstraint(
                name: "CK_tbl_XNK_Xuat_Kho_Ngay_Xuat_Kho_MinDate",
                table: "tbl_XNK_Xuat_Kho");

            migrationBuilder.DropCheckConstraint(
                name: "CK_tbl_XNK_Nhap_Kho_Don_Vi_Tien_Valid",
                table: "tbl_XNK_Nhap_Kho");

            migrationBuilder.DropCheckConstraint(
                name: "CK_tbl_XNK_Nhap_Kho_Ngay_Nhap_Kho_MinDate",
                table: "tbl_XNK_Nhap_Kho");

            migrationBuilder.DropIndex(
                name: "UQ_tbl_DM_Xuat_Kho_Raw_Data_Xuat_Kho_ID_San_Pham_ID_Active",
                table: "tbl_DM_Xuat_Kho_Raw_Data");

            migrationBuilder.DropCheckConstraint(
                name: "CK_tbl_DM_Xuat_Kho_Raw_Data_Don_Gia_Xuat_Positive",
                table: "tbl_DM_Xuat_Kho_Raw_Data");

            migrationBuilder.DropCheckConstraint(
                name: "CK_tbl_DM_Xuat_Kho_Raw_Data_SL_Xuat_Positive",
                table: "tbl_DM_Xuat_Kho_Raw_Data");

            migrationBuilder.DropIndex(
                name: "UQ_tbl_DM_Nhap_Kho_Raw_Data_Nhap_Kho_ID_San_Pham_ID_Active",
                table: "tbl_DM_Nhap_Kho_Raw_Data");

            migrationBuilder.DropCheckConstraint(
                name: "CK_tbl_DM_Nhap_Kho_Raw_Data_Don_Gia_Nhap_Positive",
                table: "tbl_DM_Nhap_Kho_Raw_Data");

            migrationBuilder.DropCheckConstraint(
                name: "CK_tbl_DM_Nhap_Kho_Raw_Data_SL_Nhap_Positive",
                table: "tbl_DM_Nhap_Kho_Raw_Data");
        }
    }
}
