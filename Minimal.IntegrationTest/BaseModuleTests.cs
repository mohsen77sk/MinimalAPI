using System.Linq.Expressions;
using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Minimal.Api.Common.TokenService;
using Minimal.DataAccess;
using Minimal.Domain;
using Minimal.IntegrationTest.Helpers;

namespace Minimal.IntegrationTest;

public class BaseModuleTests : IClassFixture<TestWebApplicationFactory<Program>>
{
    protected readonly TestWebApplicationFactory<Program> _factory;
    protected readonly HttpClient _httpClient;

    protected BaseModuleTests(TestWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _httpClient = factory.CreateClient();
        _httpClient.DefaultRequestHeaders.Authorization = GetToken();
    }

    protected AuthenticationHeaderValue GetToken()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
            var tokenService = scope.ServiceProvider.GetService<ITokenFactoryService>();
            var user = context?.Users.FirstOrDefault(x => x.UserName == "administrator");
            var token = tokenService?.CreateAccessTokenAsync(user, false).Result;
            return new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<Person> CreateNewPerson()
    {
        return await AddRowToDbAsync<Person>(new Person
        {
            FirstName = "First name",
            LastName = "Last name",
            NationalCode = "1234567890",
            Gender = 1,
            DateOfBirth = new DateTime(1990, 1, 1),
            Note = "Test create person",
            IsActive = true
        });
    }

    public async Task<Account> CreateNewAccount()
    {
        var accountType = await GetRowFromDbAsync<AccountType>(x => x.Code == "2101");
        var accountCategory = await GetRowFromDbAsync<AccountCategory>(x => x.Code == "2");

        var account = await AddRowToDbAsync<Account>(new Account
        {
            People = new List<Person>(new Person[] {
                new Person
                {
                    FirstName = "Person first name",
                    LastName = "Person last name",
                    NationalCode = "1234512345",
                    Gender = 1,
                    DateOfBirth = new DateTime(2000, 1, 1),
                    IsActive = true
                }
            }),
            AccountDetail = new AccountDetail
            {
                Title = "Account test",
                AccountCategoryId = accountCategory.Id,
                IsActive = true
            },
            AccountTypeId = accountType.Id,
            CreateDate = DateTimeOffset.Now,
            CloseDate = null,
            Note = "Test create account",
            IsActive = true
        });

        // Fill account type
        account.AccountType = accountType;

        return account;
    }

    protected async Task<T> GetRowFromDbAsync<T>(Expression<Func<T, bool>> where) where T : class
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetService<ApplicationDbContext>();
            if (db != null)
            {
                DbSet<T> dbSet = db.Set<T>();
                if (dbSet != null)
                {
                    var result = await dbSet.AsNoTracking().SingleAsync(where);
                    return result;
                }
            }
            throw new Exception();
        }
    }

    protected async Task<T> AddRowToDbAsync<T>(T entity) where T : class
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetService<ApplicationDbContext>();
            if (db != null)
            {
                DbSet<T> dbSet = db.Set<T>();
                if (dbSet != null)
                {
                    var result = dbSet.Add(entity);
                    await db.SaveChangesAsync();
                    return result.Entity;
                }
            }
            throw new Exception();
        }
    }

    protected async Task DeleteRowFromDbAsync<T>(int id) where T : class
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetService<ApplicationDbContext>();
            if (db != null)
            {
                DbSet<T> dbSet = db.Set<T>();
                if (dbSet != null)
                {
                    var row = dbSet.Find(id);
                    if (row != null)
                    {
                        dbSet.Remove(row);
                        await db.SaveChangesAsync();
                    }
                }
            }
        }
    }

    protected async Task ClearEntityFromDbAsync<T>() where T : class
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetService<ApplicationDbContext>();
            if (db != null)
            {
                DbSet<T> dbSet = db.Set<T>();
                if (dbSet != null && dbSet.Any())
                {
                    dbSet.RemoveRange(dbSet);
                    await db.SaveChangesAsync();
                }
            }
        }
    }
}