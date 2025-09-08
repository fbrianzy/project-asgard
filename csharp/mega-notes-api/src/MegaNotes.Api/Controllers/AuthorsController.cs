using Microsoft.AspNetCore.Mvc;
using MegaNotes.Api.Dtos;
using MegaNotes.Api.Services;

namespace MegaNotes.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorsController : ControllerBase
{
    private readonly AuthorService _svc;
    public AuthorsController(AuthorService svc) => _svc = svc;

    [HttpGet]
    public ActionResult<IEnumerable<AuthorDto>> Get() => Ok(_svc.GetAll());

    [HttpPost]
    public ActionResult<AuthorDto> Create(AuthorCreateDto dto)
        => Created("", _svc.Create(dto));
}
