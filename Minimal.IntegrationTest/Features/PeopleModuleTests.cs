using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Minimal.Domain;
using Minimal.DataAccess;
using Minimal.Api.Models;
using Minimal.IntegrationTest.Helpers;

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
}