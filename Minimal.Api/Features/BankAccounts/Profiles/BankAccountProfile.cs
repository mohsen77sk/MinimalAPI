using AutoMapper;
using Minimal.Api.Features.BankAccounts.Commands;
using Minimal.Api.Features.BankAccounts.Models;
using Minimal.Api.Models;
using Minimal.Domain;

namespace Minimal.Api.Features.BankAccounts.Profiles;

public class BankAccountProfile : Profile
{
    public BankAccountProfile()
    {
        CreateMap<CreateBankAccount, BankAccount>().ReverseMap();
        CreateMap<BankAccount, BankAccountGetDto>()
            .ForMember(dto => dto.PersonName, opt => opt.MapFrom(src => src.Person.FullName))
            .ForMember(dto => dto.BankName, opt => opt.MapFrom(src => src.Bank.Name));
        CreateMap<PageList<BankAccount>, PageList<BankAccountGetDto>>();
    }
}