using AutoMapper;
using Minimal.Api.Features.People.Commands;
using Minimal.Api.Features.People.Models;
using Minimal.Api.Models;
using Minimal.Domain;

namespace Minimal.Api.Features.People.Profiles;

public class PeopleProfile : Profile
{
    public PeopleProfile()
    {
        CreateMap<CreatePerson, Person>().ReverseMap();
        CreateMap<Person, LookupDto>()
            .ForMember(dto => dto.Name, opt => opt.MapFrom(src => src.FullName));
        CreateMap<Person, PersonGetDto>();
        CreateMap<PageList<Person>, PageList<PersonGetDto>>();
    }
}