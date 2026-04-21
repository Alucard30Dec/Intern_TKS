using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorApp1.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrencyUnitForNhapXuat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE "tbl_XNK_Xuat_Kho"
                ADD COLUMN IF NOT EXISTS "Don_Vi_Tien" character varying(3) NOT NULL DEFAULT 'VND';

                UPDATE "tbl_XNK_Xuat_Kho"
                SET "Don_Vi_Tien" = 'VND'
                WHERE "Don_Vi_Tien" IS NULL OR BTRIM("Don_Vi_Tien") = '';
                """);

            migrationBuilder.Sql("""
                ALTER TABLE "tbl_XNK_Nhap_Kho"
                ADD COLUMN IF NOT EXISTS "Don_Vi_Tien" character varying(3) NOT NULL DEFAULT 'VND';

                UPDATE "tbl_XNK_Nhap_Kho"
                SET "Don_Vi_Tien" = 'VND'
                WHERE "Don_Vi_Tien" IS NULL OR BTRIM("Don_Vi_Tien") = '';
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE "tbl_XNK_Xuat_Kho"
                DROP COLUMN IF EXISTS "Don_Vi_Tien";
                """);

            migrationBuilder.Sql("""
                ALTER TABLE "tbl_XNK_Nhap_Kho"
                DROP COLUMN IF EXISTS "Don_Vi_Tien";
                """);
        }
    }
}
