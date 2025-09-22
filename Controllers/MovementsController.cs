using Bank.Api.Infrastructure;
using Bank.Api.Services;
using Bank.Api.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class MovementsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IMovementService _svc;

    public MovementsController(AppDbContext db, IMovementService svc)
    {
        _db = db;
        _svc = svc;
    }
    
   

    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] MovementCreateDto dto)
        => Ok(await _svc.RegisterAsync(dto));

    
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
        => (await _svc.GetByIdAsync(id)) is { } dto ? Ok(dto) : NotFound();


    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int? accountId, [FromQuery] int? clientId,
                                      [FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        var q = _db.Movements
            .AsNoTracking()
            .Include(m => m.Account)
            .AsQueryable();

        if (accountId.HasValue)
            q = q.Where(m => m.AccountId == accountId.Value);

        if (clientId.HasValue)
            q = q.Where(m => m.Account.ClientId == clientId.Value);

        if (from.HasValue)
        {
            var f = from.Value.Date;
            q = q.Where(m => m.OccurredAt >= f);
        }

        if (to.HasValue)
        {
            var te = to.Value.Date.AddDays(1); 
            q = q.Where(m => m.OccurredAt < te);
        }

        var items = await q
            .OrderByDescending(m => m.Id)
            .Select(m => new {
                m.Id,
                m.OccurredAt,
                AccountId = m.AccountId,
                AccountNumber = m.Account.AccountNumber,
                m.MovementType,     
                m.Amount,
                m.AvailableBalanceAfter
            })
            .ToListAsync();

        return Ok(items);
    }



    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
        => await _svc.DeleteAsync(id) ? NoContent() : NotFound();
}

