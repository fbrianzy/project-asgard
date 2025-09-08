namespace TaskTracker.Api.Domain;

public enum WorkStatus { Todo, InProgress, Blocked, Done }

public class WorkItem
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string? Notes { get; set; }
    public WorkStatus Status { get; set; } = WorkStatus.Todo;
    public int Priority { get; set; } = 3; // 1 high, 5 low
    public DateTime? DueDate { get; set; }

    // Relation
    public int ProjectId { get; set; }
    public Project? Project { get; set; }
}
