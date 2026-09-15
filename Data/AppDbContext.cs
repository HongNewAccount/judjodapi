using Microsoft.EntityFrameworkCore;
using JudjodApi.Models;

namespace JudjodApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectOwner> ProjectOwners { get; set; }
    public DbSet<ProjectGroup> ProjectGroups { get; set; }
    public DbSet<ProjectGroupAssignment> ProjectGroupAssignments { get; set; }
    public DbSet<ActivityLog> ActivityLogs { get; set; }
    public DbSet<StickyNote> StickyNotes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
        modelBuilder.Entity<ProjectOwner>().HasOne(o => o.Project).WithMany(p => p.Owners).HasForeignKey(o => o.ProjectId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<ProjectOwner>().HasOne(o => o.User).WithMany().HasForeignKey(o => o.UserId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ProjectGroupAssignment>().HasOne(a => a.Project).WithMany(p => p.Groups).HasForeignKey(a => a.ProjectId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<ProjectGroupAssignment>().HasOne(a => a.Group).WithMany(g => g.ProjectAssignments).HasForeignKey(a => a.GroupId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<ActivityLog>().HasOne(a => a.Project).WithMany(p => p.ActivityLogs).HasForeignKey(a => a.ProjectId).OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<ActivityLog>().HasOne(a => a.User).WithMany().HasForeignKey(a => a.UserId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<StickyNote>().HasOne(n => n.User).WithMany().HasForeignKey(n => n.UserId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Project>().HasOne(p => p.CreatedByUser).WithMany().HasForeignKey(p => p.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
    }
}
