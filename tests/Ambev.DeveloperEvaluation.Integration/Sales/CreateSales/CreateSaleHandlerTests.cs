using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.ORM;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Sales.CreateSales;

public class CreateSaleHandlerTests : IClassFixture<CreateSaleHandlerTestsFixture>
{
    private readonly CreateSaleHandlerTestsFixture _fixture;

    public CreateSaleHandlerTests(CreateSaleHandlerTestsFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact(DisplayName = "Should create sale successfully and return CreateSaleResult")]
    public async Task Given_ValidSales_When_Create_Then_ShouldReturnCreateSaleResult()
    {
        // Arrange
        using var scope = _fixture.CreateScope();

        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
        var eventPublisher = scope.ServiceProvider.GetRequiredService<IEventPublisher>();
        var dbContext = scope.ServiceProvider.GetRequiredService<DefaultContext>();

        var handler = new CreateSaleHandler(unitOfWork, mapper, eventPublisher);
        var command = _fixture.CreateSaleCommand();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBe(Guid.Empty);

        var saleFromDb = await dbContext.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == result.Id);

        saleFromDb.Should().NotBeNull();
        saleFromDb!.Items.Should().HaveCount(command.Items.Count);
    }
    
    [Fact(DisplayName = "Should create sale item with 20 quantity and apply 20% discount")]
    public async Task Given_SaleItemWithQuantity20_When_Create_Then_ShouldApply20PercentDiscount()
    {
        // Arrange
        using var scope = _fixture.CreateScope();

        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
        var eventPublisher = scope.ServiceProvider.GetRequiredService<IEventPublisher>();
        var dbContext = scope.ServiceProvider.GetRequiredService<DefaultContext>();

        var handler = new CreateSaleHandler(unitOfWork, mapper, eventPublisher);

        var command = _fixture.CreateSaleCommand();
        var item = command.Items.First();
        item.Quantity = 20;

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        var saleFromDb = await dbContext.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == result.Id);

        var dbItem = saleFromDb!.Items.First(x => x.ProductId == item.ProductId);
        
        // 20% discount
        var expectedDiscount = dbItem.UnitPrice * dbItem.Quantity * 0.20m;

        dbItem.Discount.Should().Be(expectedDiscount);
    }
    
    [Fact(DisplayName = "Should create sale item with quantity between 4 and 9 and apply 10% discount")]
    public async Task Given_SaleItemWithQuantityBetween4And9_When_Create_Then_ShouldApply10PercentDiscount()
    {
        // Arrange
        using var scope = _fixture.CreateScope();

        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
        var eventPublisher = scope.ServiceProvider.GetRequiredService<IEventPublisher>();
        var dbContext = scope.ServiceProvider.GetRequiredService<DefaultContext>();

        var handler = new CreateSaleHandler(unitOfWork, mapper, eventPublisher);

        var command = _fixture.CreateSaleCommand();
        var item = command.Items.First();
        item.Quantity = 6; 

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        var saleFromDb = await dbContext.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == result.Id);

        var dbItem = saleFromDb!.Items.First(x => x.ProductId == item.ProductId);

        // 10% discount
        var expectedDiscount = dbItem.UnitPrice * dbItem.Quantity * 0.10m;

        dbItem.Discount.Should().Be(expectedDiscount);
    }
    
    [Fact(DisplayName = "Should throw validation exception when sale item quantity exceeds 20")]
    public async Task Given_SaleItemQuantityExceeds20_When_Create_Then_ShouldThrowValidationException()
    {
        // Arrange
        using var scope = _fixture.CreateScope();

        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();

        var eventPublisher = scope.ServiceProvider.GetRequiredService<IEventPublisher>();

        var handler = new CreateSaleHandler(unitOfWork, mapper, eventPublisher);

        var command = _fixture.CreateSaleCommand();
        command.Items.First().Quantity = 25;

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<FluentValidation.ValidationException>()
            .WithMessage("*must be 20 or less*");
    }
    
    [Fact(DisplayName = "Should throw validation exception when product name is missing")]
    public async Task Given_SaleItemMissingProductName_When_Create_Then_ShouldThrowValidationException()
    {
        // Arrange
        using var scope = _fixture.CreateScope();

        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();

        var eventPublisher = scope.ServiceProvider.GetRequiredService<IEventPublisher>();

        var handler = new CreateSaleHandler(unitOfWork, mapper, eventPublisher);

        var command = _fixture.CreateSaleCommand();
        command.Items.First().ProductName = "";

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<FluentValidation.ValidationException>()
            .WithMessage("*ProductName*");
    }
}
        
    
