using Microsoft.AspNetCore.Mvc;
using MegaNotes.Api.Dtos;
using MegaNotes.Api.Services;

namespace MegaNotes.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagsController : ControllerBase
{
    private readonly TagService _svc;
    public TagsController(TagService svc) => _svc = svc;

    [HttpGet]
    public ActionResult<IEnumerable<TagDto>> Get() => Ok(_svc.GetAll());

    [HttpPost]
    public ActionResult<TagDto> Create(TagCreateDto dto)
        => Created("", _svc.Create(dto));
}
