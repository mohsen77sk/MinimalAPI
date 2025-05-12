using AutoMapper;
using Minimal.Api.Features.AccountTransactions.Models;
using Minimal.Api.Models;
using Minimal.Domain;

namespace Minimal.Api.Features.Accounts.Profiles;

public class AccountTransactionsProfile : Profile
{
    public AccountTransactionsProfile()
    {
        CreateMap<Document, AccountTransactionGetDto>()
            .ForMember(dto => dto.Credit, opt => opt.MapFrom(src =>
                new[] { "10", "12" }.Contains(src.DocumentType.Code) ? src.DocumentItems.Sum(x => x.Credit) : 0
            ))
            .ForMember(dto => dto.Debit, opt => opt.MapFrom(src =>
                new[] { "11", "13" }.Contains(src.DocumentType.Code) ? src.DocumentItems.Sum(x => x.Debit) : 0
            ))
            .ForMember(dto => dto.Editable, opt => opt.MapFrom(src => src.DocumentItems
                .Any(x => x.AccountDetail.IsActive == false) ? false : true
            ));
        CreateMap<PageList<Document>, PageList<AccountTransactionGetDto>>();
    }
}