namespace TaskTracker.Api.Dtos;

public record WorkItemCreateDto(int ProjectId, string Title, string? Notes, string Status, int Priority, DateTime? DueDate);
public record WorkItemUpdateDto(string? Title, string? Notes, string? Status, int? Priority, DateTime? DueDate);
public record PagedResult<T>(IEnumerable<T> Items, int Total, int Page, int PageSize);
