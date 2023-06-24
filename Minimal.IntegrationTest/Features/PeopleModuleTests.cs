using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Minimal.Domain;
using Minimal.DataAccess;
using Minimal.Api.Models;
using Minimal.IntegrationTest.Helpers;
using Minimal.Api.Features.People.Commands;
using Minimal.Api.Features.People.Models;

namespace Minimal.IntegrationTest.Features;

public class PeopleModuleTests : BaseModuleTests
{
    public PeopleModuleTests(TestWebApplicationFactory<Program> factory) : base(factory)
    {
    }

    public static IEnumerable<object[]> InvalidPeople => new List<object[]>
    {
        new object[] { new Person { FirstName = "", LastName = "" }, "'First Name' must not be empty." },
        new object[] { new Person { FirstName = "", LastName = "" }, "'Last Name' must not be empty." },
        new object[] { new Person { FirstName = "a", LastName = "" }, "'First Name' must be between 2 and 25 characters. You entered 1 characters." },
        new object[] { new Person { FirstName = "", LastName = "b" }, "'Last Name' must be between 2 and 25 characters. You entered 1 characters." },
        new object[] { new Person { FirstName = "aaa", LastName = "bbb" }, "'Gender' has a range of values which does not include '0'." },
        new object[] { new Person { FirstName = "aaa", LastName = "bbb", Gender = 0 }, "'Gender' has a range of values which does not include '0'." },
        new object[] { new Person { FirstName = "aaa", LastName = "bbb", Gender = 1, NationalCode = "1" }, "'National Code' must be 10 characters in length. You entered 1 characters." },
    };

    [Theory]
    [MemberData(nameof(InvalidPeople))]
    public async Task PostPersonWithValidationProblems(Person person, string errorMessage)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/person", person);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problemResult = await response.Content.ReadFromJsonAsync<ValidationError>();

        Assert.NotNull(problemResult?.Errors);
        Assert.True(problemResult?.Errors.Any(x => x.Value.Any(x => x == errorMessage)));
    }

    [Fact]
    public async Task CreatePerson()
    {
        var newPerson = new CreatePerson
        {
            FirstName = "First name",
            LastName = "Last name",
            NationalCode = "1234512345",
            Gender = GenderEnum.Male,
            DateOfBirth = new DateTime(1993, 12, 23),
            Note = "Test note"
        };
        var response = await _httpClient.PostAsJsonAsync("/api/person", newPerson);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseResult = await response.Content.ReadFromJsonAsync<PersonGetDto>();

        Assert.NotNull(responseResult?.Id);
        Assert.NotNull(responseResult?.Code);
        Assert.Equal(newPerson.FirstName, responseResult?.FirstName);
        Assert.Equal(newPerson.LastName, responseResult?.LastName);
        Assert.Equal(newPerson.Gender, responseResult?.Gender);
        Assert.Equal(newPerson.NationalCode, responseResult?.NationalCode);
        Assert.Equal(newPerson.DateOfBirth, responseResult?.DateOfBirth);
        Assert.Equal(newPerson.Note, responseResult?.Note);

        await deleteRowFromDb<Person>(responseResult?.Id ?? 0);
    }

    [Fact]
    public async Task UpdatePerson()
    {
        var person = await addRowToDb<Person>(new Person
        {
            FirstName = "First name",
            LastName = "Last name",
            NationalCode = "1234512345",
            Gender = 1,
            DateOfBirth = new DateTime(1993, 12, 23),
            Note = "Test note"
        });

        var updatePerson = new UpdatePerson
        {
            Id = person.Id,
            FirstName = "Name",
            LastName = "Name",
            NationalCode = "",
            Gender = GenderEnum.Female,
            DateOfBirth = new DateTime(1993, 12, 23),
            Note = "note",
        };
        var response = await _httpClient.PutAsJsonAsync("/api/person", updatePerson);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseResult = await response.Content.ReadFromJsonAsync<PersonGetDto>();

        Assert.Equal(updatePerson.Id, responseResult?.Id);
        Assert.Equal(updatePerson.FirstName, responseResult?.FirstName);
        Assert.Equal(updatePerson.LastName, responseResult?.LastName);
        Assert.Equal(updatePerson.Gender, responseResult?.Gender);
        Assert.Equal(updatePerson.NationalCode, responseResult?.NationalCode);
        Assert.Equal(updatePerson.DateOfBirth, responseResult?.DateOfBirth);
        Assert.Equal(updatePerson.Note, responseResult?.Note);

        await deleteRowFromDb<Person>(person.Id);
    }

    [Fact]
    public async Task UpdateStatusPerson()
    {
        var person = await addRowToDb<Person>(new Person
        {
            FirstName = "First name",
            LastName = "Last name",
            NationalCode = "1234512345",
            Gender = 1,
            DateOfBirth = new DateTime(1993, 12, 23),
            Note = "Test note"
        });

        var updatePerson = new UpdateStatusPerson
        {
            Id = person.Id,
            IsActive = false
        };
        var response = await _httpClient.PatchAsJsonAsync("/api/person", updatePerson);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseResult = await response.Content.ReadFromJsonAsync<PersonGetDto>();

        Assert.Equal(updatePerson.Id, responseResult?.Id);
        Assert.Equal(updatePerson.IsActive, responseResult?.IsActive);

        await deleteRowFromDb<Person>(person.Id);
    }
}