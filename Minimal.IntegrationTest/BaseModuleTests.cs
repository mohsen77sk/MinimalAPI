using System.Linq.Expressions;
using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Minimal.Api.Common.TokenService;
using Minimal.DataAccess;
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

    protected async Task<T> getRowFromDbAsync<T>(Expression<Func<T, bool>> where) where T : class
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

    protected async Task<T> addRowToDbAsync<T>(T entity) where T : class
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

    protected async Task deleteRowFromDbAsync<T>(int id) where T : class
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

    protected async Task clearEntityFromDbAsync<T>() where T : class
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

    protected void clearEntityFromDb<T>() where T : class
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
                    db.SaveChanges();
                }
            }
        }
    }
}