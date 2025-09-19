using Bank.Api.Validation;

namespace Bank.Api.Services;

public interface IMovementService
{
    Task<MovementDto> RegisterAsync(MovementCreateDto dto);
}
