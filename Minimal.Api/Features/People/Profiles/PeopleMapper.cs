using Riok.Mapperly.Abstractions;
using Minimal.Api.Features.People.Commands;
using Minimal.Api.Features.People.Models;
using Minimal.Api.Models;
using Minimal.Domain;

namespace Minimal.Api.Features.People.Profiles;

[Mapper]
public partial class PeopleMapper
{
    [MapperIgnoreTarget(nameof(Person.Id))]
    [MapperIgnoreTarget(nameof(Person.Code))]
    [MapperIgnoreTarget(nameof(Person.IsActive))]
    [MapperIgnoreTarget(nameof(Person.User))]
    [MapperIgnoreTarget(nameof(Person.BankAccounts))]
    [MapperIgnoreTarget(nameof(Person.Accounts))]
    public partial Person MapToPerson(CreatePerson source);

    [MapperIgnoreSource(nameof(Person.FullName))]
    [MapperIgnoreSource(nameof(Person.User))]
    [MapperIgnoreSource(nameof(Person.BankAccounts))]
    [MapperIgnoreSource(nameof(Person.Accounts))]
    public partial PersonGetDto MapToPersonGetDto(Person source);

    [MapProperty(nameof(Person.FullName), nameof(LookupDto.Name))]
    [MapperIgnoreSource(nameof(Person.FirstName))]
    [MapperIgnoreSource(nameof(Person.LastName))]
    [MapperIgnoreSource(nameof(Person.NationalCode))]
    [MapperIgnoreSource(nameof(Person.Birthday))]
    [MapperIgnoreSource(nameof(Person.Gender))]
    [MapperIgnoreSource(nameof(Person.Note))]
    [MapperIgnoreSource(nameof(Person.IsActive))]
    [MapperIgnoreSource(nameof(Person.User))]
    [MapperIgnoreSource(nameof(Person.BankAccounts))]
    [MapperIgnoreSource(nameof(Person.Accounts))]
    public partial LookupDto MapToLookupDto(Person source);

    public PageList<PersonGetDto> MapToPageList(PageList<Person> source) =>
        new PageList<PersonGetDto>(
            source.Items.Select(MapToPersonGetDto).ToList(),
            source.Total,
            source.Page,
            source.PageSize);
}
