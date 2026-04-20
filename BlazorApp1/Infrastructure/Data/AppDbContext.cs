using BlazorApp1.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp1.Infrastructure.Data;

/// <summary>
/// DbContext quan ly schema va truy cap du lieu cho bai 1, bai 2, bai 3, bai 4 va bai 5.
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
    /// Bang danh muc san pham (bai 3).
    /// </summary>
    public DbSet<SanPham> SanPhams => Set<SanPham>();

    /// <summary>
    /// Bang danh muc nha cung cap (bai 4).
    /// </summary>
    public DbSet<NhaCungCap> NhaCungCaps => Set<NhaCungCap>();

    /// <summary>
    /// Bang danh muc kho (bai 5).
    /// </summary>
    public DbSet<Kho> Khos => Set<Kho>();

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

            entity.Property(x => x.Is_Active)
                .HasColumnName("Is_Active")
                .HasDefaultValue(true)
                .IsRequired();

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

            entity.Property(x => x.Is_Active)
                .HasColumnName("Is_Active")
                .HasDefaultValue(true)
                .IsRequired();

            entity.HasIndex(x => x.Ma_LSP)
                .HasDatabaseName("UQ_tbl_DM_Loai_San_Pham_Ma_LSP")
                .IsUnique();

            entity.HasIndex(x => x.Ten_LSP)
                .HasDatabaseName("UQ_tbl_DM_Loai_San_Pham_Ten_LSP")
                .IsUnique();
        });

        modelBuilder.Entity<SanPham>(entity =>
        {
            entity.ToTable("tbl_DM_San_Pham");

            entity.HasKey(x => x.San_Pham_ID);

            entity.Property(x => x.San_Pham_ID)
                .HasColumnName("San_Pham_ID");

            entity.Property(x => x.Ma_San_Pham)
                .HasColumnName("Ma_San_Pham")
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(x => x.Ten_San_Pham)
                .HasColumnName("Ten_San_Pham")
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.Loai_San_Pham_ID)
                .HasColumnName("Loai_San_Pham_ID")
                .IsRequired();

            entity.Property(x => x.Don_Vi_Tinh_ID)
                .HasColumnName("Don_Vi_Tinh_ID")
                .IsRequired();

            entity.Property(x => x.Ghi_Chu)
                .HasColumnName("Ghi_Chu")
                .HasMaxLength(255);

            entity.Property(x => x.Is_Active)
                .HasColumnName("Is_Active")
                .HasDefaultValue(true)
                .IsRequired();

            entity.HasIndex(x => x.Ma_San_Pham)
                .HasDatabaseName("UQ_tbl_DM_San_Pham_Ma_San_Pham")
                .IsUnique();

            entity.HasIndex(x => x.Loai_San_Pham_ID)
                .HasDatabaseName("IX_tbl_DM_San_Pham_Loai_San_Pham_ID");

            entity.HasIndex(x => x.Don_Vi_Tinh_ID)
                .HasDatabaseName("IX_tbl_DM_San_Pham_Don_Vi_Tinh_ID");

            entity.HasOne(x => x.Loai_San_Pham)
                .WithMany()
                .HasForeignKey(x => x.Loai_San_Pham_ID)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_tbl_DM_San_Pham_tbl_DM_Loai_San_Pham_Loai_San_Pham_ID");

            entity.HasOne(x => x.Don_Vi_Tinh)
                .WithMany()
                .HasForeignKey(x => x.Don_Vi_Tinh_ID)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_tbl_DM_San_Pham_tbl_DM_Don_Vi_Tinh_Don_Vi_Tinh_ID");
        });

        modelBuilder.Entity<NhaCungCap>(entity =>
        {
            entity.ToTable("tbl_DM_NCC");

            entity.HasKey(x => x.NCC_ID);

            entity.Property(x => x.NCC_ID)
                .HasColumnName("NCC_ID");

            entity.Property(x => x.Ma_NCC)
                .HasColumnName("Ma_NCC")
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(x => x.Ten_NCC)
                .HasColumnName("Ten_NCC")
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(x => x.Ghi_Chu)
                .HasColumnName("Ghi_Chu")
                .HasMaxLength(255);

            entity.Property(x => x.Is_Active)
                .HasColumnName("Is_Active")
                .HasDefaultValue(true)
                .IsRequired();

            entity.HasIndex(x => x.Ten_NCC)
                .HasDatabaseName("UQ_tbl_DM_NCC_Ten_NCC")
                .IsUnique();

            entity.HasIndex(x => x.Ma_NCC)
                .HasDatabaseName("UQ_tbl_DM_NCC_Ma_NCC")
                .IsUnique();
        });

        modelBuilder.Entity<Kho>(entity =>
        {
            entity.ToTable("tbl_DM_Kho");

            entity.HasKey(x => x.Kho_ID);

            entity.Property(x => x.Kho_ID)
                .HasColumnName("Kho_ID");

            entity.Property(x => x.Ten_Kho)
                .HasColumnName("Ten_Kho")
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(x => x.Ghi_Chu)
                .HasColumnName("Ghi_Chu")
                .HasMaxLength(255);

            entity.Property(x => x.Is_Active)
                .HasColumnName("Is_Active")
                .HasDefaultValue(true)
                .IsRequired();

            entity.HasIndex(x => x.Ten_Kho)
                .HasDatabaseName("UQ_tbl_DM_Kho_Ten_Kho")
                .IsUnique();
        });
    }
}
