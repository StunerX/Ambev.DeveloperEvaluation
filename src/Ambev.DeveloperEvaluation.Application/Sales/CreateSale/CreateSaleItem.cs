namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleItem
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}