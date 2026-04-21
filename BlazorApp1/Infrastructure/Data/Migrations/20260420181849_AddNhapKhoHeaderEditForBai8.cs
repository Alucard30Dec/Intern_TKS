using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorApp1.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNhapKhoHeaderEditForBai8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbl_DM_Nhap_Kho_tbl_DM_Kho_Kho_ID",
                table: "tbl_DM_Nhap_Kho");

            migrationBuilder.DropForeignKey(
                name: "FK_tbl_DM_Nhap_Kho_tbl_DM_NCC_NCC_ID",
                table: "tbl_DM_Nhap_Kho");

            migrationBuilder.DropForeignKey(
                name: "FK_tbl_DM_Nhap_Kho_Raw_Data_tbl_DM_Nhap_Kho_Nhap_Kho_ID",
                table: "tbl_DM_Nhap_Kho_Raw_Data");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tbl_DM_Nhap_Kho",
                table: "tbl_DM_Nhap_Kho");

            migrationBuilder.RenameTable(
                name: "tbl_DM_Nhap_Kho",
                newName: "tbl_XNK_Nhap_Kho");

            migrationBuilder.RenameIndex(
                name: "UQ_tbl_DM_Nhap_Kho_So_Phieu_Nhap_Kho",
                table: "tbl_XNK_Nhap_Kho",
                newName: "UQ_tbl_XNK_Nhap_Kho_So_Phieu_Nhap_Kho");

            migrationBuilder.RenameIndex(
                name: "IX_tbl_DM_Nhap_Kho_Ngay_Nhap_Kho",
                table: "tbl_XNK_Nhap_Kho",
                newName: "IX_tbl_XNK_Nhap_Kho_Ngay_Nhap_Kho");

            migrationBuilder.RenameIndex(
                name: "IX_tbl_DM_Nhap_Kho_NCC_ID",
                table: "tbl_XNK_Nhap_Kho",
                newName: "IX_tbl_XNK_Nhap_Kho_NCC_ID");

            migrationBuilder.RenameIndex(
                name: "IX_tbl_DM_Nhap_Kho_Kho_ID",
                table: "tbl_XNK_Nhap_Kho",
                newName: "IX_tbl_XNK_Nhap_Kho_Kho_ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tbl_XNK_Nhap_Kho",
                table: "tbl_XNK_Nhap_Kho",
                column: "Nhap_Kho_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_DM_Nhap_Kho_Raw_Data_tbl_XNK_Nhap_Kho_Nhap_Kho_ID",
                table: "tbl_DM_Nhap_Kho_Raw_Data",
                column: "Nhap_Kho_ID",
                principalTable: "tbl_XNK_Nhap_Kho",
                principalColumn: "Nhap_Kho_ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_XNK_Nhap_Kho_tbl_DM_Kho_Kho_ID",
                table: "tbl_XNK_Nhap_Kho",
                column: "Kho_ID",
                principalTable: "tbl_DM_Kho",
                principalColumn: "Kho_ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_XNK_Nhap_Kho_tbl_DM_NCC_NCC_ID",
                table: "tbl_XNK_Nhap_Kho",
                column: "NCC_ID",
                principalTable: "tbl_DM_NCC",
                principalColumn: "NCC_ID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbl_DM_Nhap_Kho_Raw_Data_tbl_XNK_Nhap_Kho_Nhap_Kho_ID",
                table: "tbl_DM_Nhap_Kho_Raw_Data");

            migrationBuilder.DropForeignKey(
                name: "FK_tbl_XNK_Nhap_Kho_tbl_DM_Kho_Kho_ID",
                table: "tbl_XNK_Nhap_Kho");

            migrationBuilder.DropForeignKey(
                name: "FK_tbl_XNK_Nhap_Kho_tbl_DM_NCC_NCC_ID",
                table: "tbl_XNK_Nhap_Kho");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tbl_XNK_Nhap_Kho",
                table: "tbl_XNK_Nhap_Kho");

            migrationBuilder.RenameTable(
                name: "tbl_XNK_Nhap_Kho",
                newName: "tbl_DM_Nhap_Kho");

            migrationBuilder.RenameIndex(
                name: "UQ_tbl_XNK_Nhap_Kho_So_Phieu_Nhap_Kho",
                table: "tbl_DM_Nhap_Kho",
                newName: "UQ_tbl_DM_Nhap_Kho_So_Phieu_Nhap_Kho");

            migrationBuilder.RenameIndex(
                name: "IX_tbl_XNK_Nhap_Kho_Ngay_Nhap_Kho",
                table: "tbl_DM_Nhap_Kho",
                newName: "IX_tbl_DM_Nhap_Kho_Ngay_Nhap_Kho");

            migrationBuilder.RenameIndex(
                name: "IX_tbl_XNK_Nhap_Kho_NCC_ID",
                table: "tbl_DM_Nhap_Kho",
                newName: "IX_tbl_DM_Nhap_Kho_NCC_ID");

            migrationBuilder.RenameIndex(
                name: "IX_tbl_XNK_Nhap_Kho_Kho_ID",
                table: "tbl_DM_Nhap_Kho",
                newName: "IX_tbl_DM_Nhap_Kho_Kho_ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tbl_DM_Nhap_Kho",
                table: "tbl_DM_Nhap_Kho",
                column: "Nhap_Kho_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_DM_Nhap_Kho_tbl_DM_Kho_Kho_ID",
                table: "tbl_DM_Nhap_Kho",
                column: "Kho_ID",
                principalTable: "tbl_DM_Kho",
                principalColumn: "Kho_ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_DM_Nhap_Kho_tbl_DM_NCC_NCC_ID",
                table: "tbl_DM_Nhap_Kho",
                column: "NCC_ID",
                principalTable: "tbl_DM_NCC",
                principalColumn: "NCC_ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_DM_Nhap_Kho_Raw_Data_tbl_DM_Nhap_Kho_Nhap_Kho_ID",
                table: "tbl_DM_Nhap_Kho_Raw_Data",
                column: "Nhap_Kho_ID",
                principalTable: "tbl_DM_Nhap_Kho",
                principalColumn: "Nhap_Kho_ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
