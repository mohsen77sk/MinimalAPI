using AutoMapper;
using Minimal.Api.Features.AccountTypes.Models;
using Minimal.Domain;

namespace Minimal.Api.Features.AccountTypes.Profiles;

public class AccountTypeProfile : Profile
{
    public AccountTypeProfile()
    {
        CreateMap<AccountType, AccountTypeGetDto>();
    }
}