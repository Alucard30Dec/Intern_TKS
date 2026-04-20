using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BlazorApp1.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddKhoUserPermissionForBai6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbl_DM_Kho_User",
                columns: table => new
                {
                    Kho_User_ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Ma_Dang_Nhap = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Kho_ID = table.Column<int>(type: "integer", nullable: false),
                    Is_Active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_DM_Kho_User", x => x.Kho_User_ID);
                    table.ForeignKey(
                        name: "FK_tbl_DM_Kho_User_tbl_DM_Kho_Kho_ID",
                        column: x => x.Kho_ID,
                        principalTable: "tbl_DM_Kho",
                        principalColumn: "Kho_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbl_DM_Kho_User_Kho_ID",
                table: "tbl_DM_Kho_User",
                column: "Kho_ID");

            migrationBuilder.CreateIndex(
                name: "UQ_tbl_DM_Kho_User_Ma_Dang_Nhap_Kho_ID",
                table: "tbl_DM_Kho_User",
                columns: new[] { "Ma_Dang_Nhap", "Kho_ID" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_DM_Kho_User");
        }
    }
}
