using Ambev.DeveloperEvaluation.Application.Exceptions;
using Ambev.DeveloperEvaluation.Application.Sales.GetSaleById;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.ORM;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Sales.GetSaleById;

public class GetSaleByIdHandlerTests : IClassFixture<GetSaleByIdQueryHandlerTestsFixture>
{
    private readonly GetSaleByIdQueryHandlerTestsFixture _fixture;

    public GetSaleByIdHandlerTests(GetSaleByIdQueryHandlerTestsFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = "Should return sale when sale exists (Integration)")]
    public async Task Handle_ExistingSaleId_ShouldReturnSale()
    {
        using var scope = _fixture.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<DefaultContext>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();

        // Arrange: Cria e salva a Sale no banco real
        var sale = _fixture.GenerateValidSale();
        dbContext.Sales.Add(sale);
        await dbContext.SaveChangesAsync();

        var handler = new GetSaleByIdQueryHandler(unitOfWork, mapper);

        // Act
        var result = await handler.Handle(new GetSaleByIdQuery(sale.Id), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(sale.Id);
        result.Number.Should().Be(sale.Number);
        result.CustomerId.Should().Be(sale.CustomerId);
        result.CustomerName.Should().Be(sale.CustomerName);
        result.BranchId.Should().Be(sale.BranchId);
        result.BranchName.Should().Be(sale.BranchName);
        result.Items.Count.Should().Be(sale.Items.Count);
    }

    [Fact(DisplayName = "Should throw NotFoundException when sale does not exist (Integration)")]
    public async Task Handle_NonExistingSaleId_ShouldThrowNotFoundException()
    {
        using var scope = _fixture.CreateScope();

        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();

        var handler = new GetSaleByIdQueryHandler(unitOfWork, mapper);

        // Act
        var act = async () => await handler.Handle(new GetSaleByIdQuery(Guid.NewGuid()), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}