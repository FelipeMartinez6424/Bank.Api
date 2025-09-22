using Bank.Api.Validation;

namespace Bank.Api.Services;

public interface IMovementService
{
    Task<MovementDto> RegisterAsync(MovementCreateDto dto);
    Task<MovementDto?> GetByIdAsync(int id);
    Task<IEnumerable<MovementDto>> GetAllAsync(int? accountId = null, DateTime? from = null, DateTime? to = null);
    Task<bool> DeleteAsync(int id);
}

