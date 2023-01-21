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
                .Where(x => x.AccountDetail.Code.Equals("11010001")).Select(x => x.Credit).SingleOrDefault()
            ))
            .ForMember(dto => dto.Debit, opt => opt.MapFrom(src => src.DocumentItems
                .Where(x => x.AccountDetail.Code.Equals("11010001")).Select(x => x.Debit).SingleOrDefault()
            ))
            .ForMember(dto => dto.Editable, opt => opt.MapFrom(src =>
                new[] { "12", "13" }.Contains(src.DocumentType.Code) ? true : false
            ));
        CreateMap<PageList<Document>, PageList<AccountTransactionGetDto>>();
    }
}