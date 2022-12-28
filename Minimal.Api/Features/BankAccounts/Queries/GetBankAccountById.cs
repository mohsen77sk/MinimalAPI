using MediatR;
using Minimal.Api.Features.BankAccounts.Models;

namespace Minimal.Api.Features.BankAccounts.Queries;

public class GetBankAccountById : IRequest<BankAccountGetDto>
{
    public int BankAccountId { get; set; }
}