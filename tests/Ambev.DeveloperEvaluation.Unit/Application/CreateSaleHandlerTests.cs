using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Domain;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class CreateSaleHandlerTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly CreateSaleHandler _handler;
    private readonly IEventPublisher _eventPublisher;

    public CreateSaleHandlerTests()
    {
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _mapper = Substitute.For<IMapper>();
        _eventPublisher = Substitute.For<IEventPublisher>();
        _handler = new CreateSaleHandler(_unitOfWork, _mapper, _eventPublisher);
    }

    [Fact(DisplayName = "Given valid sale command, should create sale and return result")]
    public async Task Handle_ValidRequest_ShouldReturnCreateSaleResult()
    {
        // Arrange
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        var saleEntity = Sale.Create(command.Number, command.CustomerId, command.CustomerName, command.BranchId,
            command.BranchName,
            command.Items.ConvertAll(item =>
                SaleItem.Create(item.ProductId, item.ProductName, item.Quantity, item.UnitPrice)
            ));

        var expectedResult = new CreateSaleResult { Id = saleEntity.Id };
        _mapper.Map<CreateSaleResult>(Arg.Any<Sale>()).Returns(expectedResult);

        _unitOfWork.Repository<Sale>()
            .AddAsync(saleEntity, Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(1));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(saleEntity.Id);

        await _unitOfWork.Repository<Sale>()
            .Received(1)
            .AddAsync(Arg.Is<Sale>(s =>
                s.Number == command.Number &&
                s.CustomerId == command.CustomerId &&
                s.CustomerName == command.CustomerName &&
                s.BranchId == command.BranchId &&
                s.BranchName == command.BranchName &&
                s.Items.Count == command.Items.Count
            ), Arg.Any<CancellationToken>());

        await _eventPublisher
            .Received(1)
            .PublishAsync(Arg.Is<SaleCreatedEvent>(e =>
                    e.Number == command.Number &&
                    e.CustomerId == command.CustomerId &&
                    e.CustomerName == command.CustomerName
            ), Arg.Any<CancellationToken>());

        await _unitOfWork
            .Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given invalid sale command, should throw validation exception")]
    public async Task Handle_InvalidRequest_ShouldThrowValidationException()
    {
        // Arrange
        var command = CreateSaleHandlerTestData.GenerateInvalidCommand();

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }


    [Fact(DisplayName = "Handler should call Mapper to map Sale entity to CreateSaleResult")]
    public async Task Handle_ValidRequest_ShouldCallMapper_MapSaleToResult()
    {
        // Arrange
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        var saleEntity = Sale.Create(command.Number, command.CustomerId, command.CustomerName, command.BranchId,
            command.BranchName,
            command.Items.ConvertAll(item =>
                SaleItem.Create(item.ProductId, item.ProductName, item.Quantity, item.UnitPrice)
            ));

        _mapper.Map<Sale>(command).Returns(saleEntity);
        _mapper.Map<CreateSaleResult>(saleEntity).Returns(new CreateSaleResult { Id = saleEntity.Id });

        _unitOfWork.Repository<Sale>()
            .AddAsync(saleEntity, Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(1));

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mapper.Received(1).Map<CreateSaleResult>(Arg.Any<Sale>());
    }
}