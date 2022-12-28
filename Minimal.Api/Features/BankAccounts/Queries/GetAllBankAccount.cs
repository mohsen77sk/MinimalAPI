using MediatR;
using Minimal.Api.Features.BankAccounts.Models;
using Minimal.Api.Models;

namespace Minimal.Api.Features.BankAccounts.Queries;

public class GetAllBankAccount : PagingData, IRequest<PageList<BankAccountGetDto>>
{
}