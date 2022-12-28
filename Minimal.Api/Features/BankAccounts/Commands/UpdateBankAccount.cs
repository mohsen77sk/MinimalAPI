using MediatR;
using Minimal.Api.Features.BankAccounts.Models;

namespace Minimal.Api.Features.BankAccounts.Commands;

public class UpdateBankAccount : IRequest<BankAccountGetDto>
{
    public int Id { get; set; }
    public int PersonId { get; set; }
    public int BankId { get; set; }
    public string BranchCode { get; set; } = string.Empty;
    public string BranchName { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string CardNumber { get; set; } = string.Empty;
    public string Iban { get; set; } = string.Empty;
}