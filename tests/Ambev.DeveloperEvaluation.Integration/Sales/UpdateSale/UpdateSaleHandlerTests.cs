using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.ORM;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Sales.UpdateSale;

public class UpdateSaleHandlerTests : IClassFixture<UpdateSaleHandlerTestsFixture>
{
    private readonly UpdateSaleHandlerTestsFixture _fixture;

    public UpdateSaleHandlerTests(UpdateSaleHandlerTestsFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact(DisplayName = "Should update sale successfully")]
    public async Task Given_ValidUpdateSaleCommand_When_UpdatingSale_Then_SaleShouldBeUpdated()
    {
        // Arrange
        using var scope = _fixture.CreateScope();

        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
        var dbContext = scope.ServiceProvider.GetRequiredService<DefaultContext>();

        var existingSale = _fixture.GenerateValidSale();
        dbContext.Sales.Add(existingSale);
        await dbContext.SaveChangesAsync();

        var updatedCommand = _fixture.GenerateUpdateCommand(existingSale.Id);

        var handler = new UpdateSaleHandler(unitOfWork, mapper);

        // Act
        await handler.Handle(updatedCommand, CancellationToken.None);

        // Assert
        var updatedSale = await dbContext.Sales
            .Include(s => s.Items.Where(item => item.DeletedAt == null))
            .AsTracking()
            .FirstOrDefaultAsync(s => s.Id == existingSale.Id);
        
        updatedSale.Should().NotBeNull();
        updatedSale!.CustomerName.Should().Be(updatedCommand.CustomerName);
        updatedSale.BranchName.Should().Be(updatedCommand.BranchName);
        updatedSale.Items.Where(x => x.IsCancelled == false).Should().HaveCount(updatedCommand.Items.Count);

        foreach (var item in updatedCommand.Items)
        {
            var updatedItem = updatedSale.Items.FirstOrDefault(i => i.ProductId == item.ProductId && !i.IsCancelled);
            updatedItem.Should().NotBeNull();
            updatedItem!.Quantity.Should().Be(item.Quantity);
            updatedItem.UnitPrice.Should().Be(item.UnitPrice);
        }
    }
}