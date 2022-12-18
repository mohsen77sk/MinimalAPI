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
        CreateMap<Person, PersonGetDto>();
        CreateMap<Person, PeopleGetDto>();
        CreateMap<PageList<Person>, PageList<PeopleGetDto>>();
    }
}