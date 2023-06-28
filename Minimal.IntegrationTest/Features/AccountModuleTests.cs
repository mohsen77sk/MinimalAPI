using System.Net;
using System.Net.Http.Json;
using Minimal.Domain;
using Minimal.Api.Models;
using Minimal.IntegrationTest.Helpers;
using Minimal.Api.Features.Accounts.Commands;
using Minimal.Api.Features.Accounts.Models;

namespace Minimal.IntegrationTest.Features;

public class AccountModuleTests : BaseModuleTests
{
    public AccountModuleTests(TestWebApplicationFactory<Program> factory) : base(factory)
    {
    }

    public static IEnumerable<object[]> InvalidAccountsForCreate => new List<object[]>
    {
        new object[] { new CreateAccount { PersonId = new List<int>(), AccountTypeId = 0 }, "'Person Id' must not be empty." },
        new object[] { new CreateAccount { PersonId = new List<int>(0), AccountTypeId = 0 }, "'Account Type Id' must not be empty." },
        new object[] { new CreateAccount { PersonId = new List<int>(0), AccountTypeId = 0, InitCredit = 0 }, "'Init Credit' must be greater than or equal to '1000'." },
    };

    public static IEnumerable<object[]> InvalidAccountsForUpdate => new List<object[]>
    {
        new object[] { new UpdateAccount { Id = 0, PersonId = new List<int>() }, "'Id' must not be empty." },
        new object[] { new UpdateAccount { Id = 0, PersonId = new List<int>() }, "'Person Id' must not be empty." },
    };

    [Theory]
    [MemberData(nameof(InvalidAccountsForCreate))]
    public async Task CreateAccountWithValidationProblems(CreateAccount account, string errorMessage)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/account", account);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problemResult = await response.Content.ReadFromJsonAsync<ValidationError>();

        Assert.NotNull(problemResult?.Errors);
        Assert.True(problemResult?.Errors.Any(x => x.Value.Any(x => x == errorMessage)));
    }

    [Theory]
    [MemberData(nameof(InvalidAccountsForUpdate))]
    public async Task UpdateAccountWithValidationProblems(UpdateAccount account, string errorMessage)
    {
        var response = await _httpClient.PutAsJsonAsync("/api/account", account);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problemResult = await response.Content.ReadFromJsonAsync<ValidationError>();

        Assert.NotNull(problemResult?.Errors);
        Assert.True(problemResult?.Errors.Any(x => x.Value.Any(x => x == errorMessage)));
    }
}