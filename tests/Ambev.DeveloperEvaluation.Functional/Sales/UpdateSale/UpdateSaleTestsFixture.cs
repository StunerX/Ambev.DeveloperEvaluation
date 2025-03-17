using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Functional.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSales;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;
using Bogus;

namespace Ambev.DeveloperEvaluation.Functional.Sales.UpdateSale;

public class UpdateSaleTestsFixture : BaseFixture
{
    public Sale GenerateValidSale(int quantity = 0, decimal unitPrice = 0m)
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
    
    public UpdateSaleRequest GenerateUpdateRequest(Guid saleId)
    {
        var saleFaker = new Faker<UpdateSaleRequest>()
            .RuleFor(x => x.CustomerId, f => f.Random.Guid())
            .RuleFor(x => x.CustomerName, f => f.Name.FullName())
            .RuleFor(x => x.BranchId, f => f.Random.Guid())
            .RuleFor(x => x.BranchName, f => f.Name.FullName())
            .Generate();
        
        saleFaker.Items = new Faker<UpdateSaleItemRequest>()
            .RuleFor(x => x.ProductId, f => f.Random.Guid())
            .RuleFor(x => x.ProductName, f => f.Name.FullName())
            .RuleFor(x => x.Quantity, f => f.Random.Int(1, 20))
            .RuleFor(x => x.UnitPrice, f => Math.Round(f.Random.Decimal(10m, 1000m), 2))
            .Generate(Faker.Random.Int(1, 5));

        return saleFaker;
    }
}