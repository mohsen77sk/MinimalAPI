namespace Minimal.Api.Features.Accounts.Models;

public class AccountBalanceGetDto
{
    public int Id { get; init; }

    public decimal Balance { get; set; }
}