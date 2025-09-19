using Bank.Api.Services;
using Bank.Api.Validation;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly IClientService _svc;
    public ClientsController(IClientService svc) => _svc = svc;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _svc.GetAllAsync());

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ClientCreateDto dto)
    {
        var created = await _svc.CreateAsync(dto);
        return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
    }
}
