using System.Net;
using System.Net.Http.Json;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSales;
using FluentAssertions;
using FluentValidation.Results;
using Xunit;

namespace Ambev.DeveloperEvaluation.Functional.Sales.CreateSales;

public class CreateSalesTests : IClassFixture<CreateSalesTestsFixture>
{
    private readonly CreateSalesTestsFixture _fixture;

    public CreateSalesTests(CreateSalesTestsFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact(DisplayName = "Should create sale successfully and return 201 Created")]
    public async Task Given_ValidSales_When_Create_Then_ShouldReturnCreated()
    {
        await _fixture.CreateE2EDatabaseAsync();
        var httpClient = _fixture.ApiClient;

        var request = _fixture.CreateSaleRequest();

        var response = await httpClient.PostAsync("/api/sales", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<ApiResponseWithData<CreateSaleResponse>>();
        result!.Data.Should().NotBeNull();
        result.Data!.Id.Should().NotBe(Guid.Empty);
    }

    [Fact(DisplayName = "Should return BadRequest when quantity exceeds 20")]
    public async Task Given_InvalidQuantity_When_Create_Then_ShouldReturnBadRequest()
    {
        var httpClient = _fixture.ApiClient;

        var request = _fixture.CreateSaleRequest();
        request.Items.First().Quantity = 25; 

        var response = await httpClient.PostAsync("/api/sales", request);

        await ValidateBadRequestAsync(response, "Items[0].Quantity","Quantity must be 20 or less.");
    }

    [Fact(DisplayName = "Should return BadRequest when quantity is zero")]
    public async Task Given_ZeroQuantity_When_Create_Then_ShouldReturnBadRequest()
    {
        var httpClient = _fixture.ApiClient;

        var request = _fixture.CreateSaleRequest();
        request.Items.First().Quantity = 0; 

        var response = await httpClient.PostAsync("/api/sales", request);
        await ValidateBadRequestAsync(response, "Items[0].Quantity","Quantity must be greater than 0.");
    }

    [Fact(DisplayName = "Should return BadRequest when customer name is empty")]
    public async Task Given_MissingCustomerName_When_Create_Then_ShouldReturnBadRequest()
    {
        var httpClient = _fixture.ApiClient;

        var request = _fixture.CreateSaleRequest();
        request.CustomerName = ""; 

        var response = await httpClient.PostAsync("/api/sales", request);

        await ValidateBadRequestAsync(response, "CustomerName","Customer name is required.");
    }

    [Fact(DisplayName = "Should return BadRequest when there are no items in the sale")]
    public async Task Given_NoItems_When_Create_Then_ShouldReturnBadRequest()
    {
        var httpClient = _fixture.ApiClient;

        var request = _fixture.CreateSaleRequest();
        request.Items.Clear();

        var response = await httpClient.PostAsync("/api/sales", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var validationErrors = await response.Content.ReadFromJsonAsync<List<ValidationFailure>>();
        validationErrors.Should().NotBeNull();
        validationErrors!.First().ErrorMessage.Should().Be("At least one sale item is required.");
        
    }
    
    [Fact(DisplayName = "Should return BadRequest when productId is empty")]
    public async Task Given_EmptyProductId_When_Create_Then_ShouldReturnBadRequest()
    {
        await _fixture.CreateE2EDatabaseAsync();
        var httpClient = _fixture.ApiClient;

        var request = _fixture.CreateSaleRequest();
        request.Items.First().ProductId = Guid.Empty;

        var response = await httpClient.PostAsync("/api/sales", request);
        await ValidateBadRequestAsync(response, "Items[0].ProductId", "ProductId is required.");
    }
    
    private static async Task ValidateBadRequestAsync(HttpResponseMessage response, string expectedProperty, string expectedMessage)
    {
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var validationErrors = await response.Content.ReadFromJsonAsync<List<ValidationFailure>>();

        validationErrors.Should().NotBeNull();
        validationErrors.Should().ContainSingle();

        var error = validationErrors!.First();
        error.PropertyName.Should().Be(expectedProperty);
        error.ErrorMessage.Should().Be(expectedMessage);
    }
    
    [Fact(DisplayName = "Should return BadRequest when productName is empty")]
    public async Task Given_EmptyProductName_When_Create_Then_ShouldReturnBadRequest()
    {
        var httpClient = _fixture.ApiClient;

        var request = _fixture.CreateSaleRequest();
        request.Items.First().ProductName = string.Empty;

        var response = await httpClient.PostAsync("/api/sales", request);

        await ValidateBadRequestAsync(response, "Items[0].ProductName", "Product name is required.");
    }
    
    [Fact(DisplayName = "Should return BadRequest when unitPrice is zero or negative")]
    public async Task Given_InvalidUnitPrice_When_Create_Then_ShouldReturnBadRequest()
    {
        var httpClient = _fixture.ApiClient;

        var request = _fixture.CreateSaleRequest();
        request.Items.First().UnitPrice = 0; // ou negativo para testar outro cenário

        var response = await httpClient.PostAsync("/api/sales", request);

        await ValidateBadRequestAsync(response, "Items[0].UnitPrice", "Unit price must be greater than 0.");
    }
}