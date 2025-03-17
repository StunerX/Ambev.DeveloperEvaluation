using System.Net;
using System.Net.Http.Json;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetPagedSales;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Ambev.DeveloperEvaluation.Functional.Sales.GetPagedSales;

public class GetPagedSalesTests : IClassFixture<GetPagedSalesTestsFixture>
{
    private readonly GetPagedSalesTestsFixture _fixture;

    public GetPagedSalesTests(GetPagedSalesTestsFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact(DisplayName = "Should return paginated sales list with success")]
    public async Task Given_ValidRequest_When_GetSales_Then_ShouldReturnPaginatedList()
    {
        // Arrange
        var dbContext = await _fixture.CreateE2EDatabaseAsync();
        var httpClient = _fixture.ApiClient;

        // Seedando o banco com vendas
        var sales = _fixture.GenerateSales(25).ToList();
        await dbContext.Sales.AddRangeAsync(sales);
        await dbContext.SaveChangesAsync();

        var query = new GetPagedSalesRequest
        {
            Page = 1,
            PageSize = 10
        };

        // Act
        var response = await httpClient.GetAsync($"/api/sales?page={query.Page}&pageSize={query.PageSize}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<PagedResponse<GetPagedSalesResponse>>();
        result.Should().NotBeNull();
        result!.Data.Should().NotBeNullOrEmpty();
        result.Data.Count().Should().BeGreaterThan(0);
        result.CurrentPage.Should().Be(query.Page);
        result.TotalPages.Should().BeGreaterThan(0);
    }
    
    [Fact(DisplayName = "Should return empty list when there are no sales")]
    public async Task Given_NoSales_When_GetSales_Then_ShouldReturnEmptyList()
    {
        // Arrange
        var dbContext = await _fixture.CreateE2EDatabaseAsync();
        await dbContext.Sales.ExecuteDeleteAsync(); 
        
        var httpClient = _fixture.ApiClient;

        var query = new GetPagedSalesRequest
        {
            Page = 1,
            PageSize = 10
        };

        // Act
        var response = await httpClient.GetAsync($"/api/sales?page={query.Page}&pageSize={query.PageSize}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PagedResponse<GetPagedSalesResponse>>();
        result.Should().NotBeNull();
        result!.Data.Should().BeEmpty();
        result.TotalPages.Should().Be(0);
        result.TotalItems.Should().Be(0);
    }
    
    [Fact(DisplayName = "Should return empty list when page number exceeds total pages")]
    public async Task Given_PageExceedsTotalPages_When_GetSales_Then_ShouldReturnEmptyList()
    {
        // Arrange
        var dbContext = await _fixture.CreateE2EDatabaseAsync();
        var httpClient = _fixture.ApiClient;

        var sales = _fixture.GenerateSales(10).ToList();
        await dbContext.Sales.AddRangeAsync(sales);
        await dbContext.SaveChangesAsync();

        var query = new GetPagedSalesRequest
        {
            Page = 5, 
            PageSize = 10
        };

        // Act
        var response = await httpClient.GetAsync($"/api/sales?page={query.Page}&pageSize={query.PageSize}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PagedResponse<GetPagedSalesResponse>>();
        result.Should().NotBeNull();
        result!.Data.Should().BeEmpty();
        result.CurrentPage.Should().Be(query.Page);
    }
    
    [Theory(DisplayName = "Should return BadRequest when page or pageSize are invalid")]
    [InlineData(0, 10)]
    [InlineData(1, 0)]
    [InlineData(-1, 10)]
    [InlineData(1, -10)]
    public async Task Given_InvalidPageOrPageSize_When_GetSales_Then_ShouldReturnBadRequest(int page, int pageSize)
    {
        // Arrange
        await _fixture.CreateE2EDatabaseAsync();
        var httpClient = _fixture.ApiClient;

        // Act
        var response = await httpClient.GetAsync($"/api/sales?page={page}&pageSize={pageSize}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}