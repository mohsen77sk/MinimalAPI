using AutoMapper;
using Minimal.Api.Features.AccountTypes.Models;
using Minimal.Api.Models;
using Minimal.Domain;

namespace Minimal.Api.Features.AccountTypes.Profiles;

public class AccountTypeProfile : Profile
{
    public AccountTypeProfile()
    {
        CreateMap<AccountType, LookupDto>();
        CreateMap<AccountType, AccountTypeGetDto>();
        CreateMap<PageList<AccountType>, PageList<AccountTypeGetDto>>();
    }
}