using TaskTracker.Api.Domain;

namespace TaskTracker.Api.Data;

public static class Seed
{
    public static async Task InitializeAsync(AppDb db)
    {
        if (db.Projects.Any()) return;

        var p = new Project { Name = "Website Revamp", Description = "Landing page + dashboard" };
        p.WorkItems = new List<WorkItem>
        {
            new() { Title = "Design wireframe", Status = WorkStatus.Todo, Priority = 2, DueDate = DateTime.UtcNow.AddDays(7) },
            new() { Title = "Build API", Status = WorkStatus.InProgress, Priority = 1, DueDate = DateTime.UtcNow.AddDays(14) },
            new() { Title = "QA & Deploy", Status = WorkStatus.Blocked, Priority = 3, DueDate = DateTime.UtcNow.AddDays(21) }
        };

        db.Projects.Add(p);
        await db.SaveChangesAsync();
    }
}
