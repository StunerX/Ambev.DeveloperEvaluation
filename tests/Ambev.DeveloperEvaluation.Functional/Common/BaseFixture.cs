using Ambev.DeveloperEvaluation.WebApi;
using Ambev.DevelopersEvaluation.Tests.Shared;
using Bogus;
using Xunit;

namespace Ambev.DeveloperEvaluation.Functional.Common;

public class BaseFixture : DatabaseFixture, IAsyncLifetime
{
    public static readonly Faker Faker = new();
    private CustomWebApplicationFactory<Program> WebApplicationFactory { get; set; }
    public ApiClient ApiClient { get; set; }
    
    public async Task InitializeAsync()
    {
        await base.InitializeAsync();
        
        WebApplicationFactory = new CustomWebApplicationFactory<Program>(ConnectionString);
        ApiClient = new ApiClient(WebApplicationFactory.CreateClient());
    }
    
    public new async Task DisposeAsync()
    {
        await base.DisposeAsync();
    }
}