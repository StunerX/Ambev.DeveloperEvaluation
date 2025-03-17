using Ambev.DeveloperEvaluation.Domain.Entities;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;

public static class SaleTestData
{
    private static readonly Faker Faker = new();

    public static Sale GenerateValidSale(int quantity = 0, decimal unitPrice = 0m)
    {
        var faker = new Faker();

        var number = $"SALE-{faker.Random.Number(1000, 9999)}";
        var customerId = faker.Random.Guid();
        var customerName = faker.Name.FullName();
        var branchId = faker.Random.Guid();
        var branchName = faker.Company.CompanyName();
        
        var itemFaker = new Faker<SaleItem>()
            .CustomInstantiator(f => SaleItem.Create(
                f.Random.Guid(),
                f.Commerce.ProductName(),
                quantity == 0 ? f.Random.Int(1, 20) : quantity,
                unitPrice == 0m ? f.Random.Decimal(1, 1000) : unitPrice
            ));

        var items = itemFaker.Generate(2);

        return Sale.Create(number, customerId, customerName, branchId, branchName, items);
    }
}