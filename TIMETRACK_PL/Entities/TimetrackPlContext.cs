using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TIMETRACK_PL.Entities;

public partial class TimetrackPlContext : DbContext
{
    public TimetrackPlContext()
    {
    }

    public TimetrackPlContext(DbContextOptions<TimetrackPlContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Employeepunchclockinterval> Employeepunchclockintervals { get; set; }

    public virtual DbSet<Interval> Intervals { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<Task> Tasks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS; Database=TIMETRACK_PL; Integrated Security=True; TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employeepunchclockinterval>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EMPLOYEE__3213E83F3F9BF6DA");

            entity.ToTable("EMPLOYEEPUNCHCLOCKINTERVALS");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.EndTimeActual).HasColumnType("datetime");
            entity.Property(e => e.EndTimePunchClockSoftware).HasColumnType("datetime");
            entity.Property(e => e.StartTimeActual).HasColumnType("datetime");
            entity.Property(e => e.StartTimePunchClockSoftware).HasColumnType("datetime");
        });

        modelBuilder.Entity<Interval>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__INTERVAL__3213E83F01A3EF8A");

            entity.ToTable("INTERVALS");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.EndTimeActual).HasColumnType("datetime");
            entity.Property(e => e.EndTimeRounded).HasColumnType("datetime");
            entity.Property(e => e.StartTimeActual).HasColumnType("datetime");
            entity.Property(e => e.StartTimeRounded).HasColumnType("datetime");
            entity.Property(e => e.TaskId).HasColumnName("Task_id");

            entity.HasOne(d => d.Task).WithMany(p => p.Intervals)
                .HasForeignKey(d => d.TaskId)
                .HasConstraintName("FK_ParentTaskChildInterval");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PROJECTS__3213E83F5F28F94D");

            entity.ToTable("PROJECTS");

            entity.HasIndex(e => e.Number, "UQ__PROJECTS__78A1A19D3AE3CE4B").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Number)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Task>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TASKS__3213E83F9E3F6874");

            entity.ToTable("TASKS");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ProjectId).HasColumnName("Project_id");

            entity.HasOne(d => d.Project).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("FK_ParentProjectChildTask");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
