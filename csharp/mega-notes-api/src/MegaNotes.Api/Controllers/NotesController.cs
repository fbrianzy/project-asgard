using Microsoft.AspNetCore.Mvc;
using MegaNotes.Api.Dtos;
using MegaNotes.Api.Services;
using MegaNotes.Api.Utilities;

namespace MegaNotes.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotesController : ControllerBase
{
    private readonly NoteService _svc;
    public NotesController(NoteService svc) => _svc = svc;

    [HttpGet]
    public ActionResult<PagedResult<NoteDto>> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? q = null)
        => Ok(_svc.Search(page, pageSize, q));

    [HttpGet("{id:guid}")]
    public ActionResult<NoteDto> GetById(Guid id)
        => _svc.Get(id) is { } dto ? Ok(dto) : NotFound();

    [HttpPost]
    public ActionResult<NoteDto> Create(NoteCreateDto dto)
    {
        var created = _svc.Create(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public ActionResult<NoteDto> Update(Guid id, NoteUpdateDto dto)
        => _svc.Update(id, dto) is { } updated ? Ok(updated) : NotFound();

    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
        => _svc.Delete(id) ? NoContent() : NotFound();
}
