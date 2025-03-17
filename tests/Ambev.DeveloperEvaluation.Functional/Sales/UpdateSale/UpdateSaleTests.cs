using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using FluentValidation.Results;
using Xunit;

namespace Ambev.DeveloperEvaluation.Functional.Sales.UpdateSale;

public class UpdateSaleTests : IClassFixture<UpdateSaleTestsFixture>
{
    private readonly UpdateSaleTestsFixture _fixture;

    public UpdateSaleTests(UpdateSaleTestsFixture fixture)
    {
        _fixture = fixture;
    }
    
     [Fact(DisplayName = "Should update sale successfully and return 204 NoContent")]
    public async Task Given_ValidSales_When_Update_Then_ShouldReturnNoContent()
    {
        // Arrange
        var dbContext = await _fixture.CreateE2EDatabaseAsync();
        var httpClient = _fixture.ApiClient;

        var sale =  _fixture.GenerateValidSale();
        dbContext.Sales.Add(sale);
        await dbContext.SaveChangesAsync();    
        
        var request = _fixture.GenerateUpdateRequest(sale.Id);

        // Act
        var response = await httpClient.PutAsync($"/api/sales/{sale.Id}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact(DisplayName = "Should return NotFound when sale does not exist")]
    public async Task Given_InvalidSaleId_When_Update_Then_ShouldReturnNotFound()
    {
        var httpClient = _fixture.ApiClient;

        var nonExistingId = Guid.NewGuid();
        var request = _fixture.GenerateUpdateRequest(nonExistingId);

        var response = await httpClient.PutAsync($"/api/sales/{nonExistingId}", request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact(DisplayName = "Should return BadRequest when customer name is empty")]
    public async Task Given_MissingCustomerName_When_Update_Then_ShouldReturnBadRequest()
    {
        await _fixture.CreateE2EDatabaseAsync();
        var httpClient = _fixture.ApiClient;
       
        var saleId = Guid.NewGuid();
       
        var request = _fixture.GenerateUpdateRequest(saleId);
        request.CustomerName = string.Empty;

        var response = await httpClient.PutAsync($"/api/sales/{saleId}", request);
        
        var content = await response.Content.ReadAsStringAsync();

        await ValidateBadRequestAsync(response, "CustomerName", "Customer name is required.");
    }

    [Fact(DisplayName = "Should return BadRequest when there are no items in the sale")]
    public async Task Given_NoItems_When_Update_Then_ShouldReturnBadRequest()
    {
        await _fixture.CreateE2EDatabaseAsync();
        var httpClient = _fixture.ApiClient;

        var saleId = Guid.NewGuid();
       
        var request = _fixture.GenerateUpdateRequest(saleId);
        request.Items.Clear();

        var response = await httpClient.PutAsync($"/api/sales/{saleId}", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var validationErrors = await response.Content.ReadFromJsonAsync<List<ValidationFailure>>();
        validationErrors.Should().NotBeNull();
        validationErrors!.First().ErrorMessage.Should().Be("At least one sale item is required.");
    }

    [Fact(DisplayName = "Should return BadRequest when productId is empty")]
    public async Task Given_EmptyProductId_When_Update_Then_ShouldReturnBadRequest()
    {
        await _fixture.CreateE2EDatabaseAsync();
        var httpClient = _fixture.ApiClient;

        var saleId = Guid.NewGuid();
       
        var request = _fixture.GenerateUpdateRequest(saleId);
        request.Items.First().ProductId = Guid.Empty;

        var response = await httpClient.PutAsync($"/api/sales/{saleId}", request);

        await ValidateBadRequestAsync(response, "Items[0].ProductId", "ProductId is required.");
    }

    [Fact(DisplayName = "Should return BadRequest when productName is empty")]
    public async Task Given_EmptyProductName_When_Update_Then_ShouldReturnBadRequest()
    {
        await _fixture.CreateE2EDatabaseAsync();
        var httpClient = _fixture.ApiClient;

        var saleId = Guid.NewGuid();
       
        var request = _fixture.GenerateUpdateRequest(saleId);
        request.Items.First().ProductName = string.Empty;

        var response = await httpClient.PutAsync($"/api/sales/{saleId}", request);

        await ValidateBadRequestAsync(response, "Items[0].ProductName", "Product name is required.");
    }

    [Fact(DisplayName = "Should return BadRequest when unitPrice is zero or negative")]
    public async Task Given_InvalidUnitPrice_When_Update_Then_ShouldReturnBadRequest()
    {
        await _fixture.CreateE2EDatabaseAsync();
        var httpClient = _fixture.ApiClient;

        var saleId = Guid.NewGuid();
       
        var request = _fixture.GenerateUpdateRequest(saleId);
        request.Items.First().UnitPrice = 0; // ou negativo para testar outro cenário

        var response = await httpClient.PutAsync($"/api/sales/{saleId}", request);

        await ValidateBadRequestAsync(response, "Items[0].UnitPrice", "Unit price must be greater than 0.");
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
}