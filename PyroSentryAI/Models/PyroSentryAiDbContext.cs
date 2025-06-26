using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PyroSentryAI.Models;

public partial class PyroSentryAiDbContext : DbContext
{
    public PyroSentryAiDbContext()
    {
    }

    public PyroSentryAiDbContext(DbContextOptions<PyroSentryAiDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TblCamera> TblCameras { get; set; }

    public virtual DbSet<TblDeletedLog> TblDeletedLogs { get; set; }

    public virtual DbSet<TblLog> TblLogs { get; set; }

    public virtual DbSet<TblRole> TblRoles { get; set; }

    public virtual DbSet<TblSetting> TblSettings { get; set; }

    public virtual DbSet<TblUser> TblUsers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)

        => optionsBuilder.UseSqlServer("Server=MELIH;Database=PyroSentryAI_DB;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblCamera>(entity =>
        {
            entity.HasKey(e => e.CameraId).HasName("PK__tbl_Came__F971E0E8F9E7D92B");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<TblDeletedLog>(entity =>
        {
            entity.Property(e => e.DeletedAt).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<TblLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__tbl_Logs__5E5499A8F2FC6DF3");

            entity.ToTable("tbl_Logs", tb => tb.HasTrigger("tr_LogDelete"));

            entity.Property(e => e.Status).HasDefaultValue("New");
            entity.Property(e => e.Timestamp).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Camera).WithMany(p => p.TblLogs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_Logs__Camera__4222D4EF");
        });

        modelBuilder.Entity<TblRole>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__tbl_Role__8AFACE3AA915FDE3");
        });

        modelBuilder.Entity<TblUser>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__tbl_User__1788CCAC416950DE");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Role).WithMany(p => p.TblUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_Users__RoleI__3B75D760");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
