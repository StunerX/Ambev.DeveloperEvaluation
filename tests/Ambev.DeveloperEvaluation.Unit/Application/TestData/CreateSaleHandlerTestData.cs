using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain;

public static class CreateSaleHandlerTestData
{
    private static readonly Faker<CreateSaleCommand> CreateSaleCommandFaker = new Faker<CreateSaleCommand>()
        .RuleFor(x => x.Number, f => $"SALE-{f.Random.Number(1000, 9999)}")
        .RuleFor(x => x.CustomerId, f => f.Random.Guid())
        .RuleFor(x => x.CustomerName, f => f.Name.FullName())
        .RuleFor(x => x.BranchId, f => f.Random.Guid())
        .RuleFor(x => x.BranchName, f => f.Company.CompanyName())
        .RuleFor(x => x.Items, f => new Faker<CreateSaleItem>()
            .RuleFor(i => i.ProductId, f => f.Random.Guid())
            .RuleFor(i => i.ProductName, f => f.Commerce.ProductName())
            .RuleFor(i => i.Quantity, f => f.Random.Int(1, 10))
            .RuleFor(i => i.UnitPrice, f => f.Random.Decimal(10m, 500m))
            .Generate(2));

    public static CreateSaleCommand GenerateValidCommand() => CreateSaleCommandFaker.Generate();

    public static CreateSaleCommand GenerateInvalidCommand()
    {
        return new CreateSaleCommand
        {
            Number = "",
            CustomerId = Guid.Empty,
            CustomerName = "",
            BranchId = Guid.Empty,
            BranchName = "",
            Items = new List<CreateSaleItem>()
        };
    }
}