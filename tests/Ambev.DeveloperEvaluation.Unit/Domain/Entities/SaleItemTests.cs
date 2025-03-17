using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

public class SaleItemTests
{
    public SaleItemTests()
    {
        
    }
    
    [Fact(DisplayName = "Discount should be zero for quantities below 4")]
    public void Given_QuantityLessThan4_When_SaleItemCreated_Then_DiscountShouldBeZero()
    {
        // Arrange
        var item = SaleItemTestData.GenerateValidSaleItem(quantity: 3);

        // Act & Assert
        Assert.Equal(0, item.Discount);
    }

    [Fact(DisplayName = "Discount should be 10% for quantities between 4 and 9")]
    public void Given_QuantityBetween4And9_When_SaleItemCreated_Then_DiscountShouldBe10Percent()
    {
        // Arrange
        const int quantity = 5;
        const decimal unitPrice = 100m;
        var item = SaleItemTestData.GenerateValidSaleItem(quantity, unitPrice);

        // Act
        const decimal expectedDiscount = quantity * unitPrice * 0.10m;

        // Assert
        Assert.Equal(expectedDiscount, item.Discount);
    }
    
    [Fact(DisplayName = "Discount should be 20% for quantities between 10 and 20")]
    public void Given_QuantityBetween10And20_When_SaleItemCreated_Then_DiscountShouldBe20Percent()
    {
        // Arrange
        const int quantity = 15;
        const decimal unitPrice = 200m;
        var item = SaleItemTestData.GenerateValidSaleItem(quantity, unitPrice);

        // Act
        const decimal expectedDiscount = quantity * unitPrice * 0.20m;

        // Assert
        Assert.Equal(expectedDiscount, item.Discount);
    }
    
    [Fact(DisplayName = "Validating SaleItem with quantity over 20 should return not valid result")]
    public void Given_QuantityAbove20_When_SaleItemCreated_Then_ShouldThrowException()
    {
        // Arrange
        const int quantity = 25;
        const decimal unitPrice = 100m;
        
        // Act & Assert
        var act = () => SaleItemTestData.GenerateValidSaleItem(quantity, unitPrice);
        act.Should().Throw<DomainException>().And.Message.Should().Contain("Quantity must be less than 20");
        
    }
}