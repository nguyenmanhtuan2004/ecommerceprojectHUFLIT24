using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EcommerceMVC.Data
{
    public partial class Hshop2023Context : DbContext
    {
        public Hshop2023Context()
        {
        }

        public Hshop2023Context(DbContextOptions<Hshop2023Context> options)
            : base(options)
        {
        }

        public virtual DbSet<AspNetRole> AspNetRoles { get; set; } = null!;
        public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; } = null!;
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; } = null!;
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; } = null!;
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; } = null!;
        public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; } = null!;
        public virtual DbSet<ChiTietHd> ChiTietHds { get; set; } = null!;
        public virtual DbSet<HangHoa> HangHoas { get; set; } = null!;
        public virtual DbSet<HoaDon> HoaDons { get; set; } = null!;
        public virtual DbSet<KhachHang> KhachHangs { get; set; } = null!;
        public virtual DbSet<Loai> Loais { get; set; } = null!;
        public virtual DbSet<NhaCungCap> NhaCungCaps { get; set; } = null!;
        public virtual DbSet<NhanVien> NhanViens { get; set; } = null!;
        public virtual DbSet<TrangThai> TrangThais { get; set; } = null!;
        public virtual DbSet<VChiTietHoaDon> VChiTietHoaDons { get; set; } = null!;
        public virtual DbSet<YeuThich> YeuThiches { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=DESKTOP-KOO7FB7\\SQLEXPRESS;Database=Hshop2023;Integrated Security=True;Trust Server Certificate=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AspNetRole>(entity =>
            {
                entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedName] IS NOT NULL)");

                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.NormalizedName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetRoleClaim>(entity =>
            {
                entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetRoleClaims)
                    .HasForeignKey(d => d.RoleId);
            });

            modelBuilder.Entity<AspNetUser>(entity =>
            {
                entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

                entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedUserName] IS NOT NULL)");

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

                entity.Property(e => e.UserName).HasMaxLength(256);

                entity.HasMany(d => d.Roles)
                    .WithMany(p => p.Users)
                    .UsingEntity<Dictionary<string, object>>(
                        "AspNetUserRole",
                        l => l.HasOne<AspNetRole>().WithMany().HasForeignKey("RoleId"),
                        r => r.HasOne<AspNetUser>().WithMany().HasForeignKey("UserId"),
                        j =>
                        {
                            j.HasKey("UserId", "RoleId");

                            j.ToTable("AspNetUserRoles");

                            j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                        });
            });

            modelBuilder.Entity<AspNetUserClaim>(entity =>
            {
                entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserClaims)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserLogin>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

                entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserLogins)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserToken>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserTokens)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<ChiTietHd>(entity =>
            {
                entity.HasKey(e => e.MaCt)
                    .HasName("PK_OrderDetails");

                entity.ToTable("ChiTietHD");

                entity.Property(e => e.MaCt).HasColumnName("MaCT");

                entity.Property(e => e.MaHd).HasColumnName("MaHD");

                entity.Property(e => e.MaHh).HasColumnName("MaHH");

                entity.Property(e => e.SoLuong).HasDefaultValueSql("((1))");

                entity.HasOne(d => d.MaHdNavigation)
                    .WithMany(p => p.ChiTietHds)
                    .HasForeignKey(d => d.MaHd)
                    .HasConstraintName("FK_OrderDetails_Orders");

                entity.HasOne(d => d.MaHhNavigation)
                    .WithMany(p => p.ChiTietHds)
                    .HasForeignKey(d => d.MaHh)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderDetails_Products");
            });

            modelBuilder.Entity<HangHoa>(entity =>
            {
                entity.HasKey(e => e.MaHh)
                    .HasName("PK_Products");

                entity.ToTable("HangHoa");

                entity.Property(e => e.MaHh).HasColumnName("MaHH");

                entity.Property(e => e.DonGia).HasDefaultValueSql("((0))");

                entity.Property(e => e.Hinh).HasMaxLength(50);

                entity.Property(e => e.MaNcc)
                    .HasMaxLength(50)
                    .HasColumnName("MaNCC");

                entity.Property(e => e.MoTaDonVi).HasMaxLength(50);

                entity.Property(e => e.NgaySx)
                    .HasColumnType("datetime")
                    .HasColumnName("NgaySX")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.TenAlias).HasMaxLength(50);

                entity.Property(e => e.TenHh)
                    .HasMaxLength(50)
                    .HasColumnName("TenHH");

                entity.HasOne(d => d.MaLoaiNavigation)
                    .WithMany(p => p.HangHoas)
                    .HasForeignKey(d => d.MaLoai)
                    .HasConstraintName("FK_Products_Categories");

                entity.HasOne(d => d.MaNccNavigation)
                    .WithMany(p => p.HangHoas)
                    .HasForeignKey(d => d.MaNcc)
                    .HasConstraintName("FK_Products_Suppliers");
            });

            modelBuilder.Entity<HoaDon>(entity =>
            {
                entity.HasKey(e => e.MaHd)
                    .HasName("PK_Orders");

                entity.ToTable("HoaDon");

                entity.Property(e => e.MaHd).HasColumnName("MaHD");

                entity.Property(e => e.CachThanhToan)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("(N'Cash')");

                entity.Property(e => e.CachVanChuyen)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("(N'Airline')");

                entity.Property(e => e.DiaChi).HasMaxLength(60);

                entity.Property(e => e.DienThoai).HasMaxLength(24);

                entity.Property(e => e.GhiChu).HasMaxLength(50);

                entity.Property(e => e.HoTen).HasMaxLength(50);

                entity.Property(e => e.MaKh)
                    .HasMaxLength(20)
                    .HasColumnName("MaKH");

                entity.Property(e => e.MaNv)
                    .HasMaxLength(50)
                    .HasColumnName("MaNV");

                entity.Property(e => e.NgayCan)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.NgayDat)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.NgayGiao)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(((1)/(1))/(1900))");

                entity.HasOne(d => d.MaKhNavigation)
                    .WithMany(p => p.HoaDons)
                    .HasForeignKey(d => d.MaKh)
                    .HasConstraintName("FK_Orders_Customers");

                entity.HasOne(d => d.MaNvNavigation)
                    .WithMany(p => p.HoaDons)
                    .HasForeignKey(d => d.MaNv)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_HoaDon_NhanVien");

                entity.HasOne(d => d.MaTrangThaiNavigation)
                    .WithMany(p => p.HoaDons)
                    .HasForeignKey(d => d.MaTrangThai)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_HoaDon_TrangThai");
            });

            modelBuilder.Entity<KhachHang>(entity =>
            {
                entity.HasKey(e => e.MaKh)
                    .HasName("PK_Customers");

                entity.ToTable("KhachHang");

                entity.Property(e => e.MaKh)
                    .HasMaxLength(20)
                    .HasColumnName("MaKH");

                entity.Property(e => e.DiaChi).HasMaxLength(60);

                entity.Property(e => e.DienThoai).HasMaxLength(24);

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.Hinh)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("(N'Photo.gif')");

                entity.Property(e => e.HoTen).HasMaxLength(50);

                entity.Property(e => e.MatKhau).HasMaxLength(50);

                entity.Property(e => e.NgaySinh)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.RandomKey)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Loai>(entity =>
            {
                entity.HasKey(e => e.MaLoai)
                    .HasName("PK_Categories");

                entity.ToTable("Loai");

                entity.Property(e => e.Hinh).HasMaxLength(50);

                entity.Property(e => e.TenLoai).HasMaxLength(50);

                entity.Property(e => e.TenLoaiAlias).HasMaxLength(50);
            });

            modelBuilder.Entity<NhaCungCap>(entity =>
            {
                entity.HasKey(e => e.MaNcc)
                    .HasName("PK_Suppliers");

                entity.ToTable("NhaCungCap");

                entity.Property(e => e.MaNcc)
                    .HasMaxLength(50)
                    .HasColumnName("MaNCC");

                entity.Property(e => e.DiaChi).HasMaxLength(50);

                entity.Property(e => e.DienThoai).HasMaxLength(50);

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.Logo).HasMaxLength(50);

                entity.Property(e => e.NguoiLienLac).HasMaxLength(50);

                entity.Property(e => e.TenCongTy).HasMaxLength(50);
            });

            modelBuilder.Entity<NhanVien>(entity =>
            {
                entity.HasKey(e => e.MaNv);

                entity.ToTable("NhanVien");

                entity.Property(e => e.MaNv)
                    .HasMaxLength(50)
                    .HasColumnName("MaNV");

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.HoTen).HasMaxLength(50);

                entity.Property(e => e.MatKhau).HasMaxLength(50);
            });

            modelBuilder.Entity<TrangThai>(entity =>
            {
                entity.HasKey(e => e.MaTrangThai);

                entity.ToTable("TrangThai");

                entity.Property(e => e.MaTrangThai).ValueGeneratedNever();

                entity.Property(e => e.MoTa).HasMaxLength(500);

                entity.Property(e => e.TenTrangThai).HasMaxLength(50);
            });

            modelBuilder.Entity<VChiTietHoaDon>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vChiTietHoaDon");

                entity.Property(e => e.MaCt).HasColumnName("MaCT");

                entity.Property(e => e.MaHd).HasColumnName("MaHD");

                entity.Property(e => e.MaHh).HasColumnName("MaHH");

                entity.Property(e => e.TenHh)
                    .HasMaxLength(50)
                    .HasColumnName("TenHH");
            });

            modelBuilder.Entity<YeuThich>(entity =>
            {
                entity.HasKey(e => e.MaYt)
                    .HasName("PK_Favorites");

                entity.ToTable("YeuThich");

                entity.Property(e => e.MaYt).HasColumnName("MaYT");

                entity.Property(e => e.MaHh).HasColumnName("MaHH");

                entity.Property(e => e.MaKh)
                    .HasMaxLength(20)
                    .HasColumnName("MaKH");

                entity.Property(e => e.MoTa).HasMaxLength(255);

                entity.Property(e => e.NgayChon).HasColumnType("datetime");

                entity.HasOne(d => d.MaHhNavigation)
                    .WithMany(p => p.YeuThiches)
                    .HasForeignKey(d => d.MaHh)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_YeuThich_HangHoa");

                entity.HasOne(d => d.MaKhNavigation)
                    .WithMany(p => p.YeuThiches)
                    .HasForeignKey(d => d.MaKh)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Favorites_Customers");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
