using AutoMapper;
using Minimal.Api.Features.LoanTransactions.Commands;
using Minimal.Api.Features.LoanTransactions.Models;
using Minimal.Api.Models;
using Minimal.Domain;

namespace Minimal.Api.Features.Loans.Profiles;

public class LoanTransactionsProfile : Profile
{
    public LoanTransactionsProfile()
    {
        CreateMap<Document, LoanTransactionGetDto>()
            .ForMember(dto => dto.Amount, opt => opt.MapFrom(src => src.DocumentItems
                .Where(x => x.AccountDetail.Code != "11010001").Sum(x => x.Credit)
            ))
            .ForMember(dto => dto.Editable, opt => opt.MapFrom(src =>
                (src.DocumentType.Code != "21" || src.DocumentItems.Any(x => x.AccountDetail.IsActive == false)) ? false : true
            ));
        CreateMap<PageList<Document>, PageList<LoanTransactionGetDto>>();
    }
}