using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorApp1.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddActiveStatusForCatalogSoftDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Is_Active",
                table: "tbl_DM_San_Pham",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "Is_Active",
                table: "tbl_DM_Loai_San_Pham",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "Is_Active",
                table: "tbl_DM_Don_Vi_Tinh",
                type: "boolean",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Is_Active",
                table: "tbl_DM_San_Pham");

            migrationBuilder.DropColumn(
                name: "Is_Active",
                table: "tbl_DM_Loai_San_Pham");

            migrationBuilder.DropColumn(
                name: "Is_Active",
                table: "tbl_DM_Don_Vi_Tinh");
        }
    }
}
