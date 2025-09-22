using Bank.Api.Validation;

namespace Bank.Api.Services;

public interface IClientService
{
    Task<ClientDto> CreateAsync(ClientCreateDto dto);
    Task<IEnumerable<ClientDto>> GetAllAsync();
    Task<ClientDetailDto?> GetByIdAsync(int id);
    Task<bool> UpdateAsync(int id, ClientUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}


