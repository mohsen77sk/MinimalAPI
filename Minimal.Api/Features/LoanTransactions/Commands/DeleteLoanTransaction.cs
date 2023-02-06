using MediatR;

namespace Minimal.Api.Features.LoanTransactions.Commands;

public class DeleteLoanTransaction : IRequest<Unit>
{
    public int Id { get; set; }
}