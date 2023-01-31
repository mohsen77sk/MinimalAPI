using MediatR;
using Minimal.Api.Models;

namespace Minimal.Api.Features.LoanTypes.Queries;

public class GetLookupLoanType : IRequest<List<LookupDto>>
{
}