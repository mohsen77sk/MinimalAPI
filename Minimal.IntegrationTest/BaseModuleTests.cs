using System.Net.Http.Headers;
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
}