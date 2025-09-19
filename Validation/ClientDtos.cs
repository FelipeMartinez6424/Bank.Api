namespace Bank.Api.Validation;

public record ClientCreateDto(string Name, string Gender, int Age, string Identification,
                              string Address, string Phone, string ClientCode, string Password);
public record ClientDto(int Id, string Name, string ClientCode, bool IsActive);
