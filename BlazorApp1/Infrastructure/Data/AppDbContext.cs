using BlazorApp1.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp1.Infrastructure.Data;

/// <summary>
/// DbContext quan ly schema va truy cap du lieu cho bai 1.
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
            entity.ToTable("tbl_dm_don_vi_tinh");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .HasColumnName("don_vi_tinh_id");

            entity.Property(x => x.TenDonViTinh)
                .HasColumnName("ten_don_vi_tinh")
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.GhiChu)
                .HasColumnName("ghi_chu")
                .HasMaxLength(255);

            entity.HasIndex(x => x.TenDonViTinh)
                .IsUnique();
        });

        modelBuilder.Entity<LoaiSanPham>(entity =>
        {
            entity.ToTable("tbl_dm_loai_san_pham");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id)
                .HasColumnName("loai_san_pham_id");

            entity.Property(x => x.MaLsp)
                .HasColumnName("ma_lsp")
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(x => x.TenLsp)
                .HasColumnName("ten_lsp")
                .HasMaxLength(120)
                .IsRequired();

            entity.Property(x => x.GhiChu)
                .HasColumnName("ghi_chu")
                .HasMaxLength(255);

            entity.HasIndex(x => x.MaLsp)
                .IsUnique();

            entity.HasIndex(x => x.TenLsp)
                .IsUnique();
        });
    }
}
