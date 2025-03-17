using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Functional.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSales;
using Bogus;

namespace Ambev.DeveloperEvaluation.Functional.Sales.CreateSales;

public class CreateSalesTestsFixture : BaseFixture
{
    public CreateSaleRequest CreateSaleRequest()
    {
        var saleFaker = new Faker<CreateSaleRequest>()
            .RuleFor(x => x.Number, f => $"SALE-{f.Random.Number(1000, 9999)}")
            .RuleFor(x => x.CustomerId, f => f.Random.Guid())
            .RuleFor(x => x.CustomerName, f => f.Name.FullName())
            .RuleFor(x => x.BranchId, f => f.Random.Guid())
            .RuleFor(x => x.BranchName, f => f.Name.FullName())
            .Generate();
        
        saleFaker.Items = new Faker<CreateSaleItem>()
            .RuleFor(x => x.ProductId, f => f.Random.Guid())
            .RuleFor(x => x.ProductName, f => f.Name.FullName())
            .RuleFor(x => x.Quantity, f => f.Random.Int(1, 20))
            .RuleFor(x => x.UnitPrice, f => Math.Round(f.Random.Decimal(10m, 1000m), 2))
            .Generate(Faker.Random.Int(1, 5));

        return saleFaker;
    }

}