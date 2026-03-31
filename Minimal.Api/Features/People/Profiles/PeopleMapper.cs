using Minimal.Api.Features.People.Commands;
using Minimal.Api.Features.People.Models;
using Minimal.Api.Models;
using Minimal.Domain;

namespace Minimal.Api.Features.People.Profiles;

public class PeopleMapper
{
    public Person MapToPerson(CreatePerson source) =>
        new Person
        {
            FirstName = source.FirstName,
            LastName = source.LastName,
            NationalCode = source.NationalCode,
            Gender = source.Gender,
            Birthday = source.Birthday,
            Note = source.Note,
        };

    public PersonGetDto MapToPersonGetDto(Person source) =>
        new PersonGetDto
        {
            Id = source.Id,
            Code = source.Code,
            FirstName = source.FirstName,
            LastName = source.LastName,
            NationalCode = source.NationalCode,
            Gender = source.Gender,
            Birthday = source.Birthday,
            Note = source.Note,
            IsActive = source.IsActive
        };

    public LookupDto MapToLookupDto(Person source) =>
        new LookupDto
        {
            Id = source.Id,
            Code = source.Code,
            Name = source.FullName
        };

    public PageList<PersonGetDto> MapToPageList(PageList<Person> source) =>
        new PageList<PersonGetDto>(
            source.Items.Select(MapToPersonGetDto).ToList(),
            source.Total,
            source.Page,
            source.PageSize);
}
