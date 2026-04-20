using BlazorApp1.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp1.Infrastructure.Data;

/// <summary>
/// DbContext quan ly schema va truy cap du lieu cho bai 1 va bai 2.
/// </summary>
public sealed class AppDbContext : DbContext
{
    /// <summary>
    /// Khoi tao db context voi cau hinh tu DI.
    /// </summary>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Bang danh muc don vi tinh (bai 1).
    /// </summary>
    public DbSet<DonViTinh> DonViTinhs => Set<DonViTinh>();

    /// <summary>
    /// Bang danh muc loai san pham (bai 2).
    /// </summary>
    public DbSet<LoaiSanPham> LoaiSanPhams => Set<LoaiSanPham>();

    /// <summary>
    /// Cau hinh mapping bang/cot/index de giu ten schema dung theo de bai SQL.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<DonViTinh>(entity =>
        {
            entity.ToTable("tbl_DM_Don_Vi_Tinh");

            entity.HasKey(x => x.Don_Vi_Tinh_ID);

            entity.Property(x => x.Don_Vi_Tinh_ID)
                .HasColumnName("Don_Vi_Tinh_ID");

            entity.Property(x => x.Ten_Don_Vi_Tinh)
                .HasColumnName("Ten_Don_Vi_Tinh")
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.Ghi_Chu)
                .HasColumnName("Ghi_Chu")
                .HasMaxLength(255);

            entity.HasIndex(x => x.Ten_Don_Vi_Tinh)
                .HasDatabaseName("UQ_tbl_DM_Don_Vi_Tinh_Ten_Don_Vi_Tinh")
                .IsUnique();
        });

        modelBuilder.Entity<LoaiSanPham>(entity =>
        {
            entity.ToTable("tbl_DM_Loai_San_Pham");

            entity.HasKey(x => x.Loai_San_Pham_ID);

            entity.Property(x => x.Loai_San_Pham_ID)
                .HasColumnName("Loai_San_Pham_ID");

            entity.Property(x => x.Ma_LSP)
                .HasColumnName("Ma_LSP")
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(x => x.Ten_LSP)
                .HasColumnName("Ten_LSP")
                .HasMaxLength(120)
                .IsRequired();

            entity.Property(x => x.Ghi_Chu)
                .HasColumnName("Ghi_Chu")
                .HasMaxLength(255);

            entity.HasIndex(x => x.Ma_LSP)
                .HasDatabaseName("UQ_tbl_DM_Loai_San_Pham_Ma_LSP")
                .IsUnique();

            entity.HasIndex(x => x.Ten_LSP)
                .HasDatabaseName("UQ_tbl_DM_Loai_San_Pham_Ten_LSP")
                .IsUnique();
        });
    }
}
