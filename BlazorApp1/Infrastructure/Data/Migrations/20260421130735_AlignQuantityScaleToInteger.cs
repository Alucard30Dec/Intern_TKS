using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorApp1.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AlignQuantityScaleToInteger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE "tbl_DM_Xuat_Kho_Raw_Data"
                SET "SL_Xuat" = ROUND("SL_Xuat", 0)
                WHERE "SL_Xuat" <> ROUND("SL_Xuat", 0);
                """);

            migrationBuilder.Sql("""
                UPDATE "tbl_DM_Nhap_Kho_Raw_Data"
                SET "SL_Nhap" = ROUND("SL_Nhap", 0)
                WHERE "SL_Nhap" <> ROUND("SL_Nhap", 0);
                """);

            migrationBuilder.AlterColumn<decimal>(
                name: "SL_Xuat",
                table: "tbl_DM_Xuat_Kho_Raw_Data",
                type: "numeric(18,0)",
                precision: 18,
                scale: 0,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "SL_Nhap",
                table: "tbl_DM_Nhap_Kho_Raw_Data",
                type: "numeric(18,0)",
                precision: 18,
                scale: 0,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "SL_Xuat",
                table: "tbl_DM_Xuat_Kho_Raw_Data",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,0)",
                oldPrecision: 18,
                oldScale: 0);

            migrationBuilder.AlterColumn<decimal>(
                name: "SL_Nhap",
                table: "tbl_DM_Nhap_Kho_Raw_Data",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,0)",
                oldPrecision: 18,
                oldScale: 0);
        }
    }
}
