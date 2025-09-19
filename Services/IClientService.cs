using Bank.Api.Validation;

namespace Bank.Api.Services;

public interface IClientService
{
    Task<ClientDto> CreateAsync(ClientCreateDto dto);
    Task<IEnumerable<ClientDto>> GetAllAsync();
}

