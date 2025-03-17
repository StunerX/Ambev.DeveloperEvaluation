using System.Net;
using System.Net.Http.Json;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Functional.Sales.GetSale;

public class GetSaleTests : IClassFixture<GetSaleTestsFixture>
{
    private readonly GetSaleTestsFixture _fixture;

    public GetSaleTests(GetSaleTestsFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = "Should return sale when sale exists (Functional)")]
    public async Task Given_ExistingSaleId_When_GetSaleById_Then_ShouldReturnSale()
    {
        var dbContext = await _fixture.CreateE2EDatabaseAsync();
        var httpClient = _fixture.ApiClient;

        // Arrange: Criar uma venda no banco real
        var sale =  _fixture.GenerateValidSale();
        dbContext.Sales.Add(sale);
        await dbContext.SaveChangesAsync();    

        // Act
        var response = await httpClient.GetAsync($"/api/sales/{sale.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ApiResponseWithData<GetSaleResponse>>();
        result!.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(sale.Id);
        result.Data.Number.Should().Be(sale.Number);
        result.Data.CustomerId.Should().Be(sale.CustomerId);
        result.Data.CustomerName.Should().Be(sale.CustomerName);
        result.Data.BranchId.Should().Be(sale.BranchId);
        result.Data.BranchName.Should().Be(sale.BranchName);
        result.Data.Items.Count.Should().Be(sale.Items.Count);
    }

    [Fact(DisplayName = "Should return 404 Not Found when sale does not exist (Functional)")]
    public async Task Given_NonExistingSaleId_When_GetSaleById_Then_ShouldReturnNotFound()
    {
        var httpClient = _fixture.ApiClient;

        // Act
        var response = await httpClient.GetAsync($"/api/sales/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}