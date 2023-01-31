using AutoMapper;
using Minimal.Api.Features.LoanTypes.Models;
using Minimal.Api.Models;
using Minimal.Domain;

namespace Minimal.Api.Features.LoanTypes.Profiles;

public class LoanTypeProfile : Profile
{
    public LoanTypeProfile()
    {
        CreateMap<LoanType, LookupDto>();
        CreateMap<LoanType, LoanTypeGetDto>();
        CreateMap<PageList<LoanType>, PageList<LoanTypeGetDto>>();
    }
}