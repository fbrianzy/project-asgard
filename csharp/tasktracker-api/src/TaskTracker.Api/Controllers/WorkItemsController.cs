using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Api.Data;
using TaskTracker.Api.Domain;
using TaskTracker.Api.Dtos;

namespace TaskTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkItemsController : ControllerBase
{
    private readonly AppDb _db;
    public WorkItemsController(AppDb db) => _db = db;

    // GET api/workitems?projectId=1&status=InProgress&sort=dueDate:asc&page=1&pageSize=10
    [HttpGet]
    public async Task<IActionResult> Query([FromQuery] int? projectId, [FromQuery] string? status,
        [FromQuery] string? sort = "priority:asc", [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var q = _db.WorkItems.AsNoTracking().AsQueryable();

        if (projectId.HasValue) q = q.Where(w => w.ProjectId == projectId.Value);
        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<WorkStatus>(status, true, out var s))
            q = q.Where(w => w.Status == s);

        // sorting
        q = sort?.ToLower() switch
        {
            "duedate:asc"  => q.OrderBy(w => w.DueDate),
            "duedate:desc" => q.OrderByDescending(w => w.DueDate),
            "priority:desc"=> q.OrderByDescending(w => w.Priority),
            _              => q.OrderBy(w => w.Priority)
        };

        var total = await q.CountAsync();
        var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return Ok(new PagedResult<object>(items, total, page, pageSize));
    }

    [HttpPost]
    public async Task<IActionResult> Create(WorkItemCreateDto dto)
    {
        if (!Enum.TryParse<WorkStatus>(dto.Status, true, out var s)) return BadRequest("Invalid status.");
        if (string.IsNullOrWhiteSpace(dto.Title)) return BadRequest("Title is required.");

        var exists = await _db.Projects.AnyAsync(p => p.Id == dto.ProjectId);
        if (!exists) return BadRequest("Project not found.");

        var w = new WorkItem
        {
            ProjectId = dto.ProjectId,
            Title = dto.Title.Trim(),
            Notes = dto.Notes,
            Status = s,
            Priority = dto.Priority,
            DueDate = dto.DueDate
        };
        _db.WorkItems.Add(w);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = w.Id }, w);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var w = await _db.WorkItems.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return w is null ? NotFound() : Ok(w);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, WorkItemUpdateDto dto)
    {
        var w = await _db.WorkItems.FindAsync(id);
        if (w is null) return NotFound();

        if (dto.Title is not null && !string.IsNullOrWhiteSpace(dto.Title)) w.Title = dto.Title.Trim();
        if (dto.Notes is not null) w.Notes = dto.Notes;
        if (dto.Status is not null && Enum.TryParse<WorkStatus>(dto.Status, true, out var s)) w.Status = s;
        if (dto.Priority.HasValue) w.Priority = dto.Priority.Value;
        if (dto.DueDate != default) w.DueDate = dto.DueDate;

        await _db.SaveChangesAsync();
        return Ok(w);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var w = await _db.WorkItems.FindAsync(id);
        if (w is null) return NotFound();
        _db.Remove(w); await _db.SaveChangesAsync();
        return NoContent();
    }
}
