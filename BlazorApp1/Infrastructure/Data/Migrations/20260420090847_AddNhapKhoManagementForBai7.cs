using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BlazorApp1.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNhapKhoManagementForBai7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbl_DM_Nhap_Kho",
                columns: table => new
                {
                    Nhap_Kho_ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    So_Phieu_Nhap_Kho = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Kho_ID = table.Column<int>(type: "integer", nullable: false),
                    NCC_ID = table.Column<int>(type: "integer", nullable: false),
                    Ngay_Nhap_Kho = table.Column<DateTime>(type: "date", nullable: false),
                    Ghi_Chu = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Is_Active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_DM_Nhap_Kho", x => x.Nhap_Kho_ID);
                    table.ForeignKey(
                        name: "FK_tbl_DM_Nhap_Kho_tbl_DM_Kho_Kho_ID",
                        column: x => x.Kho_ID,
                        principalTable: "tbl_DM_Kho",
                        principalColumn: "Kho_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbl_DM_Nhap_Kho_tbl_DM_NCC_NCC_ID",
                        column: x => x.NCC_ID,
                        principalTable: "tbl_DM_NCC",
                        principalColumn: "NCC_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbl_DM_Nhap_Kho_Raw_Data",
                columns: table => new
                {
                    Nhap_Kho_Raw_Data_ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nhap_Kho_ID = table.Column<int>(type: "integer", nullable: false),
                    San_Pham_ID = table.Column<int>(type: "integer", nullable: false),
                    SL_Nhap = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Don_Gia_Nhap = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Is_Active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_DM_Nhap_Kho_Raw_Data", x => x.Nhap_Kho_Raw_Data_ID);
                    table.ForeignKey(
                        name: "FK_tbl_DM_Nhap_Kho_Raw_Data_tbl_DM_Nhap_Kho_Nhap_Kho_ID",
                        column: x => x.Nhap_Kho_ID,
                        principalTable: "tbl_DM_Nhap_Kho",
                        principalColumn: "Nhap_Kho_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbl_DM_Nhap_Kho_Raw_Data_tbl_DM_San_Pham_San_Pham_ID",
                        column: x => x.San_Pham_ID,
                        principalTable: "tbl_DM_San_Pham",
                        principalColumn: "San_Pham_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbl_DM_Nhap_Kho_Kho_ID",
                table: "tbl_DM_Nhap_Kho",
                column: "Kho_ID");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_DM_Nhap_Kho_NCC_ID",
                table: "tbl_DM_Nhap_Kho",
                column: "NCC_ID");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_DM_Nhap_Kho_Ngay_Nhap_Kho",
                table: "tbl_DM_Nhap_Kho",
                column: "Ngay_Nhap_Kho");

            migrationBuilder.CreateIndex(
                name: "UQ_tbl_DM_Nhap_Kho_So_Phieu_Nhap_Kho",
                table: "tbl_DM_Nhap_Kho",
                column: "So_Phieu_Nhap_Kho",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tbl_DM_Nhap_Kho_Raw_Data_Nhap_Kho_ID",
                table: "tbl_DM_Nhap_Kho_Raw_Data",
                column: "Nhap_Kho_ID");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_DM_Nhap_Kho_Raw_Data_San_Pham_ID",
                table: "tbl_DM_Nhap_Kho_Raw_Data",
                column: "San_Pham_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_DM_Nhap_Kho_Raw_Data");

            migrationBuilder.DropTable(
                name: "tbl_DM_Nhap_Kho");
        }
    }
}
