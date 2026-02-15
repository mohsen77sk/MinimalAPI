using Riok.Mapperly.Abstractions;
using Minimal.Api.Features.People.Commands;
using Minimal.Api.Features.People.Models;
using Minimal.Api.Models;
using Minimal.Domain;

namespace Minimal.Api.Features.People.Profiles;

[Mapper(IgnoreObsoleteMembersStrategy = IgnoreObsoleteMembersStrategy.Both)]
public partial class PeopleMapper
{
    public partial Person MapToPerson(CreatePerson source);

    public partial PersonGetDto MapToPersonGetDto(Person source);

    [MapProperty(nameof(Person.FullName), nameof(LookupDto.Name))]
    public partial LookupDto MapToLookupDto(Person source);

    public PageList<PersonGetDto> MapToPageList(PageList<Person> source) =>
        new PageList<PersonGetDto>(
            source.Items.Select(MapToPersonGetDto).ToList(),
            source.Total,
            source.Page,
            source.PageSize);
}
