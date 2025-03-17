using System.Linq.Expressions;
using Ambev.DeveloperEvaluation.Application.Exceptions;
using Ambev.DeveloperEvaluation.Application.Sales.GetSaleById;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class GetSaleByIdHandlerTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly GetSaleByIdQueryHandler _handler;

    public GetSaleByIdHandlerTests()
    {
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _mapper = Substitute.For<IMapper>();

        _handler = new GetSaleByIdQueryHandler(_unitOfWork, _mapper);
    }

    [Fact(DisplayName = "Should return sale when sale exists")]
    public async Task Handle_ExistingSaleId_ShouldReturnSale()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var sale = GetSaleByIdHandlerTestData.GenerateValidSale();
        var expectedResult = new GetSaleByIdQueryResult
        {
            Id = saleId,
            CustomerId = sale.CustomerId,
            CustomerName = sale.CustomerName,
            BranchId = sale.BranchId,
            BranchName = sale.BranchName,
            Items = sale.Items.Select(x => new GetSaleItemResult
            {
                ProductId = x.ProductId,
                ProductName = x.ProductName,
                Quantity = x.Quantity,
                UnitPrice = x.UnitPrice,
            }).ToList(),
        };

        _unitOfWork.Repository<Sale>()
            .FirstOrDefaultAsync(
                Arg.Any<Expression<Func<Sale, bool>>>(),
                Arg.Any<Expression<Func<Sale, Sale>>>(),
                Arg.Any<string[]>(),
                Arg.Any<CancellationToken>())
            .Returns(sale);
        
        _mapper.Map<GetSaleByIdQueryResult>(sale).Returns(expectedResult);

        // Act
        var result = await _handler.Handle(new GetSaleByIdQuery(saleId), CancellationToken.None);
       
        // Assert
        
        await _unitOfWork.Repository<Sale>().Received(1).FirstOrDefaultAsync(
            Arg.Any<Expression<Func<Sale, bool>>>(),
            Arg.Any<Expression<Func<Sale, Sale>>>(),
            Arg.Any<string[]>(),
            Arg.Any<CancellationToken>());
        
        result.Should().NotBeNull();
        result.CustomerId.Should().Be(sale.CustomerId);
        result.CustomerName.Should().Be(sale.CustomerName);
        result.BranchId.Should().Be(sale.BranchId);
        result.BranchName.Should().Be(sale.BranchName);
        result.Items.Count.Should().Be(sale.Items.Count);
        
    }

    [Fact(DisplayName = "Should throw NotFoundException when sale does not exist")]
    public async Task Handle_NonExistingSaleId_ShouldThrowNotFoundException()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        _unitOfWork.Repository<Sale>()
            .FirstOrDefaultAsync(
                Arg.Any<Expression<Func<Sale, bool>>>(),
                Arg.Any<Expression<Func<Sale, Sale>>>(),
                Arg.Any<string[]>(),
                Arg.Any<CancellationToken>())
            .Returns((Sale)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(new GetSaleByIdQuery(saleId), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Sale with Id {saleId} not found.");
    }
}