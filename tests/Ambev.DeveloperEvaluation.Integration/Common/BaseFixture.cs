using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Messaging.Events;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using Ambev.DevelopersEvaluation.Tests.Shared;
using AutoMapper;
using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Common;

public class BaseFixture : DatabaseFixture, IAsyncLifetime
{
    private IServiceScope _scope;
    
    public IServiceProvider Services { get; private set; }
    
    public static readonly Faker Faker = new();
    
    public DefaultContext DbContext => _scope.ServiceProvider.GetRequiredService<DefaultContext>();
    public IMapper Mapper => _scope.ServiceProvider.GetRequiredService<IMapper>();
    public IUnitOfWork UnitOfWork => _scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
    public IEventPublisher EventPublisher => _scope.ServiceProvider.GetRequiredService<IEventPublisher>();
 
    public async Task InitializeAsync()
    {
        // Sobe o banco no Docker + cria/migra
        await base.InitializeAsync();

        var dbContext = await CreateE2EDatabaseAsync();

        // Agora cria o ServiceProvider usando a ConnectionString dinâmica
        var services = new ServiceCollection();

        services.AddDbContext<DefaultContext>(options =>
        {
            options.UseNpgsql(dbContext.Database.GetConnectionString());
        });

        services.AddScoped<IUnitOfWork, UnitOfWork<DefaultContext>>();
        services.AddAutoMapper(typeof(CreateSaleProfile).Assembly);
        services.AddScoped<IEventPublisher, EventPublisher>();

        Services = services.BuildServiceProvider();

        // Cria o escopo inicial
        _scope = Services.CreateScope();

        // Roda migrations pra garantir que tá atualizado (redundância segura)
        var context = _scope.ServiceProvider.GetRequiredService<DefaultContext>();
        await context.Database.MigrateAsync();
    }
    
    public async Task DisposeAsync()
    {
        _scope.Dispose();
        await base.DisposeAsync();
    }

    public IServiceScope CreateScope()
    {
        _scope.Dispose();
        _scope = Services.CreateScope();
        return _scope;
    }
}