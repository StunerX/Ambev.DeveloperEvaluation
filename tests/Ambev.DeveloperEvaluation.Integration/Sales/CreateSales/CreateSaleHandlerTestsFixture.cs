using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Integration.Common;
using Ambev.DeveloperEvaluation.ORM;
using AutoMapper;
using Bogus;

namespace Ambev.DeveloperEvaluation.Integration.Sales.CreateSales;

public class CreateSaleHandlerTestsFixture : BaseFixture
{

    public CreateSaleCommand CreateSaleCommand()
    {
        var saleFaker = new Faker<CreateSaleCommand>()
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