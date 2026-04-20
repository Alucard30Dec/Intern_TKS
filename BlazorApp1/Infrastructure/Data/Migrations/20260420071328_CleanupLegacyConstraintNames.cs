using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorApp1.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class CleanupLegacyConstraintNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DROP INDEX IF EXISTS uq_tbl_dm_loai_san_pham_ma;
                DROP INDEX IF EXISTS uq_tbl_dm_loai_san_pham_ten;
                DROP INDEX IF EXISTS uq_tbl_dm_don_vi_tinh_ten;

                ALTER TABLE "tbl_DM_Don_Vi_Tinh" DROP CONSTRAINT IF EXISTS uq_tbl_dm_don_vi_tinh_ten;

                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1
                        FROM pg_constraint
                        WHERE conname = 'tbl_dm_don_vi_tinh_pkey'
                          AND conrelid = '"tbl_DM_Don_Vi_Tinh"'::regclass
                    ) THEN
                        ALTER TABLE "tbl_DM_Don_Vi_Tinh"
                            RENAME CONSTRAINT tbl_dm_don_vi_tinh_pkey TO "PK_tbl_DM_Don_Vi_Tinh";
                    END IF;

                    IF EXISTS (
                        SELECT 1
                        FROM pg_constraint
                        WHERE conname = 'tbl_dm_loai_san_pham_pkey'
                          AND conrelid = '"tbl_DM_Loai_San_Pham"'::regclass
                    ) THEN
                        ALTER TABLE "tbl_DM_Loai_San_Pham"
                            RENAME CONSTRAINT tbl_dm_loai_san_pham_pkey TO "PK_tbl_DM_Loai_San_Pham";
                    END IF;
                END $$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1
                        FROM pg_constraint
                        WHERE conname = 'PK_tbl_DM_Don_Vi_Tinh'
                          AND conrelid = '"tbl_DM_Don_Vi_Tinh"'::regclass
                    ) THEN
                        ALTER TABLE "tbl_DM_Don_Vi_Tinh"
                            RENAME CONSTRAINT "PK_tbl_DM_Don_Vi_Tinh" TO tbl_dm_don_vi_tinh_pkey;
                    END IF;

                    IF EXISTS (
                        SELECT 1
                        FROM pg_constraint
                        WHERE conname = 'PK_tbl_DM_Loai_San_Pham'
                          AND conrelid = '"tbl_DM_Loai_San_Pham"'::regclass
                    ) THEN
                        ALTER TABLE "tbl_DM_Loai_San_Pham"
                            RENAME CONSTRAINT "PK_tbl_DM_Loai_San_Pham" TO tbl_dm_loai_san_pham_pkey;
                    END IF;
                END $$;
                """);
        }
    }
}
