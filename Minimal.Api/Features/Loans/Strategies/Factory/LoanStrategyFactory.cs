using Minimal.Domain;

namespace Minimal.Api.Features.Loans.Strategies;

public class LoanStrategyFactory : ILoanStrategyFactory
{
    private readonly IServiceProvider _provider;

    public LoanStrategyFactory(IServiceProvider provider)
    {
        _provider = provider;
    }

    public ILoanStrategy GetStrategy(LoanStrategyEnum strategy)
    {
        return strategy switch
        {
            LoanStrategyEnum.Standard => _provider.GetRequiredService<StandardLoanStrategy>(),

            LoanStrategyEnum.GharzolHasaneh => _provider.GetRequiredService<GharzolHasanehLoanStrategy>(),

            _ => throw new NotImplementedException($"Strategy {strategy} not implemented")
        };
    }
}