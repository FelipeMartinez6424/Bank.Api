using Bank.Api.Validation;

public interface IClientService
{
    Task<IEnumerable<ClientDto>> GetAllAsync(string? q = null, string? identification = null);
    Task<ClientDetailDto?> GetByIdAsync(int id);
    Task<ClientDto> CreateAsync(ClientCreateDto dto);
    Task<bool> UpdateAsync(int id, ClientUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}



