using Bank.Api.Validation;

public interface IAccountService
{
    Task<IEnumerable<AccountDto>> GetAllAsync(int? clientId = null, string? accountNumber = null);
    Task<AccountDto?> GetByIdAsync(int id);
    Task<AccountDto> CreateAsync(AccountCreateDto dto);
    Task<bool> UpdateAsync(int id, AccountUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}

