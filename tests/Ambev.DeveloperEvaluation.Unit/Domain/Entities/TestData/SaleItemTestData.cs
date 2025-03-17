using Ambev.DeveloperEvaluation.Domain.Entities;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;

public static class SaleItemTestData
{
    private static readonly Faker Faker = new();

    public static SaleItem GenerateValidSaleItem(int quantity = 5, decimal unitPrice = 100m)
    {
        var productId = Guid.NewGuid();
        var productName = Faker.Commerce.ProductName();

        return SaleItem.Create(productId, productName, quantity, unitPrice);
    }
}