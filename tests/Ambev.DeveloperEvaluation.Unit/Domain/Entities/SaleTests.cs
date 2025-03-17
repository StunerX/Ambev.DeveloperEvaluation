using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

/// <summary>
/// Contains unit tests for the Sale entity class.
/// Tests cover adding items, cancelling sales, and total calculation.
/// </summary>
public class SaleTests
{
    [Fact(DisplayName = "Sale total amount should be recalculated after adding an item")]
    public void Given_Sale_When_AddingSaleItem_Then_TotalAmountShouldBeCorrect()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var item = SaleItemTestData.GenerateValidSaleItem(quantity: 5);
        var item2 = SaleItemTestData.GenerateValidSaleItem(quantity: 2);

        var oldSaleAmount = sale.Amount;

        // Act
        sale.AddItem(item);
        sale.AddItem(item2);

        // Assert
        Assert.Equal(oldSaleAmount + item.Amount + item2.Amount, sale.Amount);
    }

    [Fact(DisplayName = "Sale should be marked as cancelled")]
    public void Given_Sale_When_Cancelled_Then_IsCancelledShouldBeTrue()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();

        // Act
        sale.CancelSale();

        // Assert
        Assert.True(sale.IsCancelled);
    }

    [Fact(DisplayName = "Removing an item should recalculate total amount")]
    public void Given_SaleWithItems_When_RemoveItem_Then_TotalAmountShouldBeRecalculated()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();

        // Pre-condition Assert
        var originalSaleAmount = sale.Amount;
        var originalItemAmount = sale.Items.Sum(i => i.Amount);

        Assert.Equal(originalSaleAmount, originalItemAmount);

        // Act
        var itemToRemove = sale.Items.First();
        var removedAmount = itemToRemove.Amount;
        sale.RemoveItem(itemToRemove);

        // Assert
        Assert.Equal(sale.Amount, originalSaleAmount - removedAmount);
    }
    
    [Fact(DisplayName = "Deve atualizar os dados básicos do Sale")]
        public void Update_Should_UpdateBasicFields()
        {
            // Arrange
            var sale = Sale.Create("SALE-001", Guid.NewGuid(), "Cliente Antigo", Guid.NewGuid(), "Loja Antiga", new List<SaleItem>() {  SaleItem.Create(Guid.NewGuid(), "Primeiro produto", 1, 10) });

            var newCustomerId = Guid.NewGuid();
            var newCustomerName = "Cliente Novo";
            var newBranchId = Guid.NewGuid();
            var newBranchName = "Loja Nova";

            var items = new List<SaleItem>
            {
                SaleItem.Create(Guid.NewGuid(), "Produto 1", 1, 100)
            };

            // Act
            sale.Update(newCustomerId, newCustomerName, newBranchId, newBranchName, items);

            // Assert
            sale.CustomerId.Should().Be(newCustomerId);
            sale.CustomerName.Should().Be(newCustomerName);
            sale.BranchId.Should().Be(newBranchId);
            sale.BranchName.Should().Be(newBranchName);
        }

        [Fact(DisplayName = "Deve substituir itens antigos pelos novos")]
        public void Update_Should_ReplaceOldItemsWithNewOnes()
        {
            // Arrange
            var oldItem = SaleItem.Create(Guid.NewGuid(), "Produto Antigo", 2, 50);
            var sale = Sale.Create("SALE-002", Guid.NewGuid(), "Cliente", Guid.NewGuid(), "Loja", new List<SaleItem> { oldItem });

            var newItem1 = SaleItem.Create(Guid.NewGuid(), "Produto Novo 1", 3, 30);
            var newItem2 = SaleItem.Create(Guid.NewGuid(), "Produto Novo 2", 4, 40);

            var newItems = new List<SaleItem> { newItem1, newItem2 };

            // Act
            sale.Update(sale.CustomerId, sale.CustomerName, sale.BranchId, sale.BranchName, newItems);

            // Assert
            sale.Items.Where(x => x.IsCancelled == false).Should().HaveCount(2);
            sale.Items.Should().Contain(newItem1);
            sale.Items.Should().Contain(newItem2);

            // O item antigo deve ter sido "cancelado" (soft delete)
            oldItem.IsCancelled.Should().BeTrue();
            oldItem.DeletedAt.Should().NotBeNull();
        }

        [Fact(DisplayName = "Deve manter os itens antigos se a lista passada for vazia")]
        public void Update_Should_KeepOldItems_When_NewItemsListIsEmpty()
        {
            // Arrange
            var item = SaleItem.Create(Guid.NewGuid(), "Produto Existente", 2, 50);
            var sale = Sale.Create("SALE-003", Guid.NewGuid(), "Cliente", Guid.NewGuid(), "Loja", new List<SaleItem> { item });

            var newItems = new List<SaleItem>(); // Lista vazia

            // Act
            sale.Update(sale.CustomerId, sale.CustomerName, sale.BranchId, sale.BranchName, newItems);

            // Assert
            sale.Items.Should().HaveCount(1);
            sale.Items.First().Should().Be(item);

            // Não deveria ter feito soft delete
            item.IsCancelled.Should().BeFalse();
            item.DeletedAt.Should().BeNull();
        }

        [Fact(DisplayName = "Deve recalcular o Amount após atualização de itens")]
        public void Update_Should_RecalculateTotalAmount_AfterItemsUpdate()
        {
            var item = SaleItem.Create(Guid.NewGuid(), "Produto Existente", 2, 50);
            var sale = Sale.Create("SALE-004", Guid.NewGuid(), "Cliente", Guid.NewGuid(), "Loja", new List<SaleItem> { item });

            var newItem1 = SaleItem.Create(Guid.NewGuid(), "Produto 1", 1, 100);
            var newItem2 = SaleItem.Create(Guid.NewGuid(), "Produto 2", 2, 150);

            var newItems = new List<SaleItem> { newItem1, newItem2 };

            // Act
            sale.Update(sale.CustomerId, sale.CustomerName, sale.BranchId, sale.BranchName, newItems);

            // Assert
            var expectedAmount = newItem1.Amount + newItem2.Amount;
            sale.Amount.Should().Be(expectedAmount);
        }

        [Fact(DisplayName = "Deve marcar itens removidos com IsCancelled e DeletedAt preenchido")]
        public void Update_Should_SoftDelete_OldItems()
        {
            // Arrange
            var oldItem1 = SaleItem.Create(Guid.NewGuid(), "Produto Antigo 1", 1, 100);
            var oldItem2 = SaleItem.Create(Guid.NewGuid(), "Produto Antigo 2", 2, 200);

            var sale = Sale.Create("SALE-005", Guid.NewGuid(), "Cliente", Guid.NewGuid(), "Loja", new List<SaleItem> { oldItem1, oldItem2 });

            var newItem = SaleItem.Create(Guid.NewGuid(), "Produto Novo", 1, 50);

            // Act
            sale.Update(sale.CustomerId, sale.CustomerName, sale.BranchId, sale.BranchName, new List<SaleItem> { newItem });

            // Assert
            oldItem1.IsCancelled.Should().BeTrue();
            oldItem1.DeletedAt.Should().NotBeNull();

            oldItem2.IsCancelled.Should().BeTrue();
            oldItem2.DeletedAt.Should().NotBeNull();
        }
}