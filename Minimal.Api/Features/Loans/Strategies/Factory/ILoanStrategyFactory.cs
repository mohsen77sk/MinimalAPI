using Minimal.Domain;

namespace Minimal.Api.Features.Loans.Strategies;

public interface ILoanStrategyFactory
{
    ILoanStrategy GetStrategy(LoanStrategyEnum strategy);
}