using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorApp1.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AlignTableColumnNamesWithSpec : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.tables
                        WHERE table_schema = 'public' AND table_name = 'tbl_dm_don_vi_tinh'
                    ) THEN
                        ALTER TABLE tbl_dm_don_vi_tinh RENAME TO "tbl_DM_Don_Vi_Tinh";
                    END IF;

                    IF EXISTS (
                        SELECT 1 FROM information_schema.tables
                        WHERE table_schema = 'public' AND table_name = 'tbl_dm_loai_san_pham'
                    ) THEN
                        ALTER TABLE tbl_dm_loai_san_pham RENAME TO "tbl_DM_Loai_San_Pham";
                    END IF;
                END $$;
                """);

            migrationBuilder.Sql(
                """
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_schema = 'public' AND table_name = 'tbl_DM_Don_Vi_Tinh' AND column_name = 'don_vi_tinh_id'
                    ) THEN
                        ALTER TABLE "tbl_DM_Don_Vi_Tinh" RENAME COLUMN don_vi_tinh_id TO "Don_Vi_Tinh_ID";
                    END IF;

                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_schema = 'public' AND table_name = 'tbl_DM_Don_Vi_Tinh' AND column_name = 'ten_don_vi_tinh'
                    ) THEN
                        ALTER TABLE "tbl_DM_Don_Vi_Tinh" RENAME COLUMN ten_don_vi_tinh TO "Ten_Don_Vi_Tinh";
                    END IF;

                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_schema = 'public' AND table_name = 'tbl_DM_Don_Vi_Tinh' AND column_name = 'ghi_chu'
                    ) THEN
                        ALTER TABLE "tbl_DM_Don_Vi_Tinh" RENAME COLUMN ghi_chu TO "Ghi_Chu";
                    END IF;
                END $$;
                """);

            migrationBuilder.Sql(
                """
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_schema = 'public' AND table_name = 'tbl_DM_Loai_San_Pham' AND column_name = 'loai_san_pham_id'
                    ) THEN
                        ALTER TABLE "tbl_DM_Loai_San_Pham" RENAME COLUMN loai_san_pham_id TO "Loai_San_Pham_ID";
                    END IF;

                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_schema = 'public' AND table_name = 'tbl_DM_Loai_San_Pham' AND column_name = 'ma_lsp'
                    ) THEN
                        ALTER TABLE "tbl_DM_Loai_San_Pham" RENAME COLUMN ma_lsp TO "Ma_LSP";
                    END IF;

                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_schema = 'public' AND table_name = 'tbl_DM_Loai_San_Pham' AND column_name = 'ten_lsp'
                    ) THEN
                        ALTER TABLE "tbl_DM_Loai_San_Pham" RENAME COLUMN ten_lsp TO "Ten_LSP";
                    END IF;

                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_schema = 'public' AND table_name = 'tbl_DM_Loai_San_Pham' AND column_name = 'ghi_chu'
                    ) THEN
                        ALTER TABLE "tbl_DM_Loai_San_Pham" RENAME COLUMN ghi_chu TO "Ghi_Chu";
                    END IF;
                END $$;
                """);

            migrationBuilder.Sql(
                """
                ALTER TABLE "tbl_DM_Don_Vi_Tinh" DROP CONSTRAINT IF EXISTS uq_tbl_dm_don_vi_tinh_ten;
                DROP INDEX IF EXISTS "IX_tbl_dm_don_vi_tinh_ten_don_vi_tinh";
                DROP INDEX IF EXISTS "UQ_tbl_DM_Don_Vi_Tinh_Ten_Don_Vi_Tinh";
                CREATE UNIQUE INDEX "UQ_tbl_DM_Don_Vi_Tinh_Ten_Don_Vi_Tinh"
                    ON "tbl_DM_Don_Vi_Tinh" ("Ten_Don_Vi_Tinh");
                """);

            migrationBuilder.Sql(
                """
                DROP INDEX IF EXISTS "IX_tbl_dm_loai_san_pham_ma_lsp";
                DROP INDEX IF EXISTS "IX_tbl_dm_loai_san_pham_ten_lsp";
                DROP INDEX IF EXISTS "UQ_tbl_DM_Loai_San_Pham_Ma_LSP";
                DROP INDEX IF EXISTS "UQ_tbl_DM_Loai_San_Pham_Ten_LSP";

                CREATE UNIQUE INDEX "UQ_tbl_DM_Loai_San_Pham_Ma_LSP"
                    ON "tbl_DM_Loai_San_Pham" ("Ma_LSP");

                CREATE UNIQUE INDEX "UQ_tbl_DM_Loai_San_Pham_Ten_LSP"
                    ON "tbl_DM_Loai_San_Pham" ("Ten_LSP");
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DROP INDEX IF EXISTS "UQ_tbl_DM_Don_Vi_Tinh_Ten_Don_Vi_Tinh";
                DROP INDEX IF EXISTS "UQ_tbl_DM_Loai_San_Pham_Ma_LSP";
                DROP INDEX IF EXISTS "UQ_tbl_DM_Loai_San_Pham_Ten_LSP";
                """);
        }
    }
}
