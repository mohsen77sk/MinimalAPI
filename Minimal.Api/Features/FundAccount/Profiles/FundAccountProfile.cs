using AutoMapper;
using Minimal.Api.Features.FundAccount.Models;
using Minimal.Api.Models;
using Minimal.Domain;

namespace Minimal.Api.Features.FundAccount.Profiles;

public class FundAccountProfile : Profile
{
    public FundAccountProfile()
    {
        CreateMap<DocumentArticle, FundAccountTransactionGetDto>()
            .ForMember(dto => dto.Date, opt => opt.MapFrom(src => src.Document.Date))
            .ForMember(dto => dto.Credit, opt => opt.MapFrom(src => src.Debit))
            .ForMember(dto => dto.debit, opt => opt.MapFrom(src => src.Credit));
        CreateMap<PageList<DocumentArticle>, PageList<FundAccountTransactionGetDto>>();
    }
}