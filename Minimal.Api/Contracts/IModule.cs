namespace Minimal.Api.Contracts;

public interface IModule
{
    IEndpointRouteBuilder RegisterEndpoints(IEndpointRouteBuilder endpoints);
}