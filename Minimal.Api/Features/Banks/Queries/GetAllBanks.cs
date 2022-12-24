using MediatR;
using Minimal.Api.Features.Banks.Models;

namespace Minimal.Api.Features.Banks.Queries;

public class GetAllBank : IRequest<List<BankGetDto>>
{
}