using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BlazorApp1.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSanPhamCatalogForBai3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbl_DM_San_Pham",
                columns: table => new
                {
                    San_Pham_ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Ma_San_Pham = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Ten_San_Pham = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Loai_San_Pham_ID = table.Column<int>(type: "integer", nullable: false),
                    Don_Vi_Tinh_ID = table.Column<int>(type: "integer", nullable: false),
                    Ghi_Chu = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_DM_San_Pham", x => x.San_Pham_ID);
                    table.ForeignKey(
                        name: "FK_tbl_DM_San_Pham_tbl_DM_Don_Vi_Tinh_Don_Vi_Tinh_ID",
                        column: x => x.Don_Vi_Tinh_ID,
                        principalTable: "tbl_DM_Don_Vi_Tinh",
                        principalColumn: "Don_Vi_Tinh_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbl_DM_San_Pham_tbl_DM_Loai_San_Pham_Loai_San_Pham_ID",
                        column: x => x.Loai_San_Pham_ID,
                        principalTable: "tbl_DM_Loai_San_Pham",
                        principalColumn: "Loai_San_Pham_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbl_DM_San_Pham_Don_Vi_Tinh_ID",
                table: "tbl_DM_San_Pham",
                column: "Don_Vi_Tinh_ID");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_DM_San_Pham_Loai_San_Pham_ID",
                table: "tbl_DM_San_Pham",
                column: "Loai_San_Pham_ID");

            migrationBuilder.CreateIndex(
                name: "UQ_tbl_DM_San_Pham_Ma_San_Pham",
                table: "tbl_DM_San_Pham",
                column: "Ma_San_Pham",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_DM_San_Pham");
        }
    }
}
