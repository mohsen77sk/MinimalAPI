using MediatR;
using Minimal.Api.Features.BankAccounts.Models;

namespace Minimal.Api.Features.BankAccounts.Commands;

public class UpdateStatusBankAccount : IRequest<BankAccountGetDto>
{
    public int Id { get; set; }
    public bool IsActive { get; set; }
}