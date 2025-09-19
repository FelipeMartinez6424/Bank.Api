using Bank.Api.Services;
using Bank.Api.Validation;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class MovementsController : ControllerBase
{
    private readonly IMovementService _svc;
    public MovementsController(IMovementService svc) => _svc = svc;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] MovementCreateDto dto)
        => Ok(await _svc.RegisterAsync(dto));
}
