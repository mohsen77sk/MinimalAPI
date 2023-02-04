using AutoMapper;
using Minimal.Api.Features.Loans.Commands;
using Minimal.Api.Features.Loans.Models;
using Minimal.Api.Models;
using Minimal.Domain;

namespace Minimal.Api.Features.Loans.Profiles;

public class LoanProfile : Profile
{
    public LoanProfile()
    {
        CreateMap<CreateLoan, Loan>().ReverseMap();
        CreateMap<Loan, LoanGetDto>()
            .ForMember(dto => dto.AccountCode, opt => opt.MapFrom(src => src.Account.Code))
            .ForMember(dto => dto.LoanTypeName, opt => opt.MapFrom(src => src.LoanType.Name));
        CreateMap<PageList<Loan>, PageList<LoanGetDto>>();
    }
}