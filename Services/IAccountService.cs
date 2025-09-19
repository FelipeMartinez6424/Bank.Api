using Bank.Api.Validation;

namespace Bank.Api.Services;

public interface IAccountService
{
    Task<AccountDto> CreateAsync(AccountCreateDto dto);
    Task<IEnumerable<AccountDto>> GetAllAsync();
    Task<AccountDto?> GetByIdAsync(int id);
    Task<bool> UpdateAsync(int id, AccountUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}
