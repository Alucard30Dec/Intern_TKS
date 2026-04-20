using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BlazorApp1.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNhaCungCapCatalogForBai4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbl_DM_NCC",
                columns: table => new
                {
                    NCC_ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Ma_NCC = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Ten_NCC = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Ghi_Chu = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Is_Active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_DM_NCC", x => x.NCC_ID);
                });

            migrationBuilder.CreateIndex(
                name: "UQ_tbl_DM_NCC_Ten_NCC",
                table: "tbl_DM_NCC",
                column: "Ten_NCC",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_DM_NCC");
        }
    }
}
