using AutoMapper;
using Minimal.Api.Features.AccountTransactions.Commands;
using Minimal.Api.Features.AccountTransactions.Models;
using Minimal.Api.Models;
using Minimal.Domain;

namespace Minimal.Api.Features.Accounts.Profiles;

public class AccountTransactionsProfile : Profile
{
    public AccountTransactionsProfile()
    {
        CreateMap<Document, AccountTransactionGetDto>()
            .ForMember(dto => dto.Credit, opt => opt.MapFrom(src => src.DocumentItems
                .Where(x => x.AccountDetail.Code != "11010001").Sum(x => x.Credit)
            ))
            .ForMember(dto => dto.Debit, opt => opt.MapFrom(src => src.DocumentItems
                .Where(x => x.AccountDetail.Code != "11010001").Sum(x => x.Debit)
            ))
            .ForMember(dto => dto.Editable, opt => opt.MapFrom(src => src.DocumentItems
                .Any(x => x.AccountDetail.IsActive == false) ? false : true
            ));
        CreateMap<PageList<Document>, PageList<AccountTransactionGetDto>>();
    }
}