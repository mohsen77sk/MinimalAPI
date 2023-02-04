using MediatR;
using Minimal.Api.Features.Loans.Models;

namespace Minimal.Api.Features.Loans.Commands;

public class UpdateLoan : IRequest<LoanGetDto>
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public string Note { get; set; } = string.Empty;
}