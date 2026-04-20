using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BlazorApp1.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddKhoCatalogAndRequireUniqueSupplierCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                UPDATE "tbl_DM_NCC"
                SET "Ma_NCC" = UPPER(BTRIM("Ma_NCC"))
                WHERE "Ma_NCC" IS NOT NULL;

                WITH duplicate_codes AS (
                    SELECT
                        "NCC_ID",
                        ROW_NUMBER() OVER (PARTITION BY "Ma_NCC" ORDER BY "NCC_ID") AS rn
                    FROM "tbl_DM_NCC"
                    WHERE "Ma_NCC" IS NOT NULL
                      AND BTRIM("Ma_NCC") <> ''
                )
                UPDATE "tbl_DM_NCC" n
                SET "Ma_NCC" = CONCAT('AUTO_NCC_DUP_', n."NCC_ID")
                FROM duplicate_codes d
                WHERE n."NCC_ID" = d."NCC_ID"
                  AND d.rn > 1;

                UPDATE "tbl_DM_NCC"
                SET "Ma_NCC" = CONCAT('AUTO_NCC_', "NCC_ID")
                WHERE "Ma_NCC" IS NULL
                   OR BTRIM("Ma_NCC") = '';
                """);

            migrationBuilder.AlterColumn<string>(
                name: "Ma_NCC",
                table: "tbl_DM_NCC",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "tbl_DM_Kho",
                columns: table => new
                {
                    Kho_ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Ten_Kho = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Ghi_Chu = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Is_Active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_DM_Kho", x => x.Kho_ID);
                });

            migrationBuilder.CreateIndex(
                name: "UQ_tbl_DM_NCC_Ma_NCC",
                table: "tbl_DM_NCC",
                column: "Ma_NCC",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_tbl_DM_Kho_Ten_Kho",
                table: "tbl_DM_Kho",
                column: "Ten_Kho",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_DM_Kho");

            migrationBuilder.DropIndex(
                name: "UQ_tbl_DM_NCC_Ma_NCC",
                table: "tbl_DM_NCC");

            migrationBuilder.AlterColumn<string>(
                name: "Ma_NCC",
                table: "tbl_DM_NCC",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);
        }
    }
}
