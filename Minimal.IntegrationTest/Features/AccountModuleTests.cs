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

    [Fact]
    public async Task CreateAccount()
    {
        await ClearDbForAccountTest();

        var person = await CreateNewPerson();
        var accountType = await GetRowFromDbAsync<AccountType>(x => x.Code == "2101");

        var newAccount = new CreateAccount
        {
            PersonId = new List<int>(new int[] { person.Id }),
            AccountTypeId = accountType.Id,
            CreateDate = DateTimeOffset.Now,
            InitCredit = 1000,
            Note = "Test note"
        };
        var response = await _httpClient.PostAsJsonAsync("/api/account", newAccount);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseResult = await response.Content.ReadFromJsonAsync<AccountGetDto>();

        Assert.NotNull(responseResult?.Id);
        Assert.NotNull(responseResult?.Code);
        Assert.True(responseResult?.Persons.Any(x => x.Id == person.Id));
        Assert.Equal(accountType.Id, responseResult?.AccountTypeId);
        Assert.Equal(accountType.Name, responseResult?.AccountTypeName);
        Assert.Equal(newAccount.CreateDate, responseResult?.CreateDate);
        Assert.Equal(newAccount.Note, responseResult?.Note);
        Assert.True(responseResult?.IsActive);

        var responseBalance = await _httpClient.GetAsync("/api/account/" + responseResult?.Id + "/balance");

        Assert.Equal(HttpStatusCode.OK, responseBalance.StatusCode);
        var responseBalanceResult = await responseBalance.Content.ReadFromJsonAsync<AccountBalanceGetDto>();

        Assert.Equal(newAccount.InitCredit, responseBalanceResult?.Balance);

        await ClearDbForAccountTest();
    }

    [Fact]
    public async Task UpdateAccount()
    {
        await ClearDbForAccountTest();

        var account = await CreateNewAccount();
        var newPerson = await CreateNewPerson();

        var updateAccount = new UpdateAccount
        {
            Id = account.Id,
            PersonId = new List<int>(new int[] { newPerson.Id }),
            Note = "Test update note"
        };
        var response = await _httpClient.PutAsJsonAsync("/api/account", updateAccount);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseResult = await response.Content.ReadFromJsonAsync<AccountGetDto>();

        Assert.NotNull(responseResult?.Id);
        Assert.NotNull(responseResult?.Code);
        Assert.False(responseResult?.Persons.Any(x => x.Id == account.People.First().Id));
        Assert.True(responseResult?.Persons.Any(x => x.Id == newPerson.Id));
        Assert.Equal(updateAccount.Note, responseResult?.Note);
        Assert.True(responseResult?.IsActive);

        await ClearDbForAccountTest();
    }

    [Fact]
    public async Task CloseAccount()
    {
        await ClearDbForAccountTest();

        var account = await CreateNewAccount();

        var closeAccount = new CloseAccount
        {
            Id = account.Id,
            CloseDate = DateTimeOffset.Now
        };
        var response = await _httpClient.PostAsJsonAsync("/api/account/close", closeAccount);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseResult = await response.Content.ReadFromJsonAsync<AccountGetDto>();

        Assert.NotNull(responseResult?.Id);
        Assert.NotNull(responseResult?.CloseDate);
        Assert.Equal(closeAccount.CloseDate, responseResult?.CloseDate);
        Assert.False(responseResult?.IsActive);

        await ClearDbForAccountTest();
    }

    [Fact]
    public async Task GetAccount()
    {
        await ClearDbForAccountTest();

        var account = await CreateNewAccount();

        var response = await _httpClient.GetAsync("/api/account/" + account.Id);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseResult = await response.Content.ReadFromJsonAsync<AccountGetDto>();

        Assert.Equal(account.Id, responseResult?.Id);
        Assert.Equal(account.Code, responseResult?.Code);
        Assert.True(responseResult?.Persons.Any(x => x.Id == account.People.First().Id));
        Assert.Equal(account.AccountTypeId, responseResult?.AccountTypeId);
        Assert.Equal(account.AccountType.Name, responseResult?.AccountTypeName);
        Assert.Equal(account.CreateDate, responseResult?.CreateDate);
        Assert.Equal(account.CloseDate, responseResult?.CloseDate);
        Assert.Equal(account.Note, responseResult?.Note);
        Assert.Equal(account.IsActive, responseResult?.IsActive);

        await ClearDbForAccountTest();
    }

    private async Task ClearDbForAccountTest()
    {
        await ClearEntityFromDbAsync<Document>();
        await ClearEntityFromDbAsync<AccountDetail>();
        await ClearEntityFromDbAsync<Account>();
        await ClearEntityFromDbAsync<Person>();
    }
}