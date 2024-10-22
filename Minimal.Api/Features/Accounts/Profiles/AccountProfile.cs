using AutoMapper;
using Minimal.Api.Features.Accounts.Commands;
using Minimal.Api.Features.Accounts.Models;
using Minimal.Api.Models;
using Minimal.Domain;

namespace Minimal.Api.Features.Accounts.Profiles;

public class AccountProfile : Profile
{
    public AccountProfile()
    {
        CreateMap<CreateAccount, Account>().ReverseMap();
        CreateMap<Account, LookupDto>()
            .ForMember(dto => dto.Name, opt => opt.MapFrom(src => src.AccountType.Name + " - " + string.Join(", ", src.People.Select(x => x.FullName))));
        CreateMap<Account, AccountGetDto>()
            .ForMember(dto => dto.AccountTypeName, opt => opt.MapFrom(src => src.AccountType.Name))
            .ForMember(dto => dto.Persons, opt => opt.MapFrom(src => src.People.Select(x =>
                new LookupDto { Id = x.Id, Code = x.Code, Name = x.FullName }
            )));
        CreateMap<PageList<Account>, PageList<AccountGetDto>>();
    }
}