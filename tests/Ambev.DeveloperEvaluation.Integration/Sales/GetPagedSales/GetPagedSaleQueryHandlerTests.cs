using Ambev.DeveloperEvaluation.Application.Sales.GetPagedSales;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Integration.Sales.GetPaginatedSales;
using Ambev.DeveloperEvaluation.ORM;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Sales.GetPagedSales;

public class GetPagedSaleQueryHandlerTests : IClassFixture<GetPagedSaleQueryHandlerTestsFixture>
{
    private readonly GetPagedSaleQueryHandlerTestsFixture _fixture;
    
    public GetPagedSaleQueryHandlerTests(GetPagedSaleQueryHandlerTestsFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact(DisplayName = "Should return paginated sales successfully")]
    public async Task Given_ExistingSales_When_GetPaginatedSales_Then_ReturnPagedResult()
    {
        // Arrange
        using var scope = _fixture.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<DefaultContext>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();

        // Seedando o banco com vendas
        var sales = _fixture.GenerateSales(25).ToList();
        await dbContext.Sales.AddRangeAsync(sales);
        await dbContext.SaveChangesAsync();

        var handler = new GetPagedSalesQueryHandler(unitOfWork, mapper);

        var query = new GetPagedSalesQuery
        {
            Page = 1,
            PageSize = 10
        };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(query.PageSize);
        result.CurrentPage.Should().Be(query.Page);
        result.PageSize.Should().Be(query.PageSize);
        result.TotalItems.Should().Be(sales.Count);
        result.HasNextPage.Should().BeTrue();
        result.HasPreviousPage.Should().BeFalse();
    }
    
    [Fact(DisplayName = "Should return empty list when no sales exist")]
    public async Task Given_NoSales_When_GetPaginatedSales_Then_ReturnEmptyPagedResult()
    {
        // Arrange
        using var scope = _fixture.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<DefaultContext>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
        
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();

        var handler = new GetPagedSalesQueryHandler(unitOfWork, mapper);

        var query = new GetPagedSalesQuery
        {
            Page = 1,
            PageSize = 10
        };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.TotalItems.Should().Be(0);
        result.CurrentPage.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.HasNextPage.Should().BeFalse();
    }
}