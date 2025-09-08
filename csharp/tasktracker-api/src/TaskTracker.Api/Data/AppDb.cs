using Microsoft.EntityFrameworkCore;
using TaskTracker.Api.Domain;

namespace TaskTracker.Api.Data;

public class AppDb : DbContext
{
    public AppDb(DbContextOptions<AppDb> options) : base(options) { }

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<WorkItem> WorkItems => Set<WorkItem>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Project>()
            .HasMany(p => p.WorkItems)
            .WithOne(w => w.Project!)
            .HasForeignKey(w => w.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        b.Entity<WorkItem>()
            .Property(w => w.Status)
            .HasConversion<string>(); // enum as string
    }
}
