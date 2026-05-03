using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ProjectManagement.Models;

public partial class ProjectManagementContext : IdentityDbContext<ApplicationUser>
{
    public ProjectManagementContext()
    {
    }

    public ProjectManagementContext(DbContextOptions<ProjectManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TbActivityLog> TbActivityLogs { get; set; }

    public virtual DbSet<TbAttachment> TbAttachments { get; set; }

    public virtual DbSet<TbComment> TbComments { get; set; }

    public virtual DbSet<TbNotification> TbNotifications { get; set; }

    public virtual DbSet<TbProject> TbProjects { get; set; }

    public virtual DbSet<TbProjectMember> TbProjectMembers { get; set; }

    public virtual DbSet<TbTask> TbTasks { get; set; }

    public virtual DbSet<TbTaskHistory> TbTaskHistories { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TbActivityLog>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Action).HasMaxLength(50);
            entity.Property(e => e.EntityType).HasMaxLength(50);
            entity.Property(e => e.EntityId).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(500);

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId);
        });

        modelBuilder.Entity<TbAttachment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Attachme__3214EC073AE3B018");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.FileUrl).HasMaxLength(200);

            entity.HasOne(d => d.Task).WithMany()
                .HasForeignKey(d => d.TaskId)
                .HasConstraintName("FK__Attachmen__TaskI__4CA06362");

            entity.HasOne(d => d.UploadedByNavigation).WithMany()
                .HasForeignKey(d => d.UploadedBy)
                .HasConstraintName("FK__Attachmen__Uploa__4D94879B");
        });

        modelBuilder.Entity<TbComment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Comments__3214EC07279B244E");

            entity.Property(e => e.Content).HasMaxLength(200);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Task).WithMany()
                .HasForeignKey(d => d.TaskId)
                .HasConstraintName("FK__Comments__TaskId__4AB81AF0");

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Comments__UserId__4BAC3F29");
        });

        modelBuilder.Entity<TbNotification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Notifica__3214EC07D6006DDB");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Message).HasMaxLength(200);
            entity.Property(e => e.Type).HasMaxLength(50);

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Notificat__UserI__4E88ABD4");
        });

        modelBuilder.Entity<TbProject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Projects__3214EC077FDC379D");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.CreatedByNavigation).WithMany()
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Projects__Create__44FF419A");
        });

        modelBuilder.Entity<TbProjectMember>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProjectM__3214EC07F20E807A");

            entity.Property(e => e.JoinedAt).HasColumnType("datetime");
            entity.Property(e => e.Role).HasMaxLength(50);

            entity.HasOne(d => d.Project).WithMany(pm => pm.TbProjectMembers)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("FK__ProjectMe__Proje__45F365D3");

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__ProjectMe__UserI__46E78A0C");
        });

        modelBuilder.Entity<TbTask>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tasks__3214EC07B727A843");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.Priority).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.AssignedToNavigation).WithMany()
                .HasForeignKey(d => d.AssignedTo)
                .HasConstraintName("FK__Tasks__AssignedT__48CFD27E");

            entity.HasOne(d => d.CreatedByNavigation).WithMany()
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Tasks__CreatedBy__49C3F6B7");

            entity.HasOne(d => d.Project).WithMany(p => p.TbTasks)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("FK__Tasks__ProjectId__47DBAE45");
        });

        modelBuilder.Entity<TbTaskHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TaskHist__3214EC075D06874D");

            entity.ToTable("TbTaskHistory");

            entity.Property(e => e.ChangedAt).HasColumnType("datetime");
            entity.Property(e => e.FieldChanged).HasMaxLength(50);
            entity.Property(e => e.NewValue).HasMaxLength(50);
            entity.Property(e => e.OldValue).HasMaxLength(50);

            entity.HasOne(d => d.ChangedByNavigation).WithMany()
                .HasForeignKey(d => d.ChangedBy)
                .HasConstraintName("FK__TaskHisto__Chang__5070F446");

            entity.HasOne(d => d.Task).WithMany()
                .HasForeignKey(d => d.TaskId)
                .HasConstraintName("FK__TaskHisto__TaskI__4F7CD00D")
                .OnDelete(DeleteBehavior.Cascade);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
