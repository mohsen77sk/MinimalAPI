using AutoMapper;
using Minimal.Api.Features.Banks.Models;
using Minimal.Api.Models;
using Minimal.Domain;

namespace Minimal.Api.Features.Banks.Profiles;

public class BankProfile : Profile
{
    public BankProfile()
    {
        CreateMap<Bank, LookupDto>();
        CreateMap<Bank, BankGetDto>();
        CreateMap<PageList<Bank>, PageList<BankGetDto>>();
    }
}