using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.ORM;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Sales.DeleteSale;

public class DeleteSaleHandlerTests : IClassFixture<DeleteSaleHandlerTestsFixture>
{
    private readonly DeleteSaleHandlerTestsFixture _fixture;

    public DeleteSaleHandlerTests(DeleteSaleHandlerTestsFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact(DisplayName = "Should delete sale successfully")]
    public async Task Given_ValidDeleteSaleCommand_When_DeletingSale_Then_SaleShouldBeSoftDeleted()
    {
        // Arrange
        using var scope = _fixture.CreateScope();

        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var dbContext = scope.ServiceProvider.GetRequiredService<DefaultContext>();

        var existingSale = _fixture.GenerateValidSale();
        dbContext.Sales.Add(existingSale);
        await dbContext.SaveChangesAsync();

        var deleteCommand = new DeleteSaleCommand(existingSale.Id);

        var handler = new DeleteSaleHandler(unitOfWork);

        // Act
        await handler.Handle(deleteCommand, CancellationToken.None);

        // Assert
        var deletedSale = await dbContext.Sales
            .IgnoreQueryFilters() 
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == existingSale.Id);

        deletedSale.Should().NotBeNull();

        deletedSale!.DeletedAt.Should().NotBeNull();

        deletedSale.IsCancelled.Should().BeTrue();

        foreach (var item in deletedSale.Items)
        {
            item.DeletedAt.Should().NotBeNull();
            item.IsCancelled.Should().BeTrue();
        }
    }
}