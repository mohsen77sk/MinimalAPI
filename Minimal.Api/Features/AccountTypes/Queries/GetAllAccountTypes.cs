using MediatR;
using Minimal.Api.Features.AccountTypes.Models;

namespace Minimal.Api.Features.AccountTypes.Queries;

public class GetAllAccountType : IRequest<List<AccountTypeGetDto>>
{
}