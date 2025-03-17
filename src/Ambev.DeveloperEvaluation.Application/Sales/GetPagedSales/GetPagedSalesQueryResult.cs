namespace Ambev.DeveloperEvaluation.Application.Sales.GetPagedSales;

public class GetPagedSalesQueryResult
{
    public Guid Id { get; set; }
    public string Number { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; }
    public Guid BranchId { get; set; }
    public string BranchName { get; set; }
    public decimal Amount { get; set; }
    public bool IsCancelled { get; set; }
    public DateTime CreatedAt { get; set; }

    public List<GetPagedSalesItemQueryResult> Items { get; set; } = new();
}

public class GetPagedSalesItemQueryResult
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal Amount { get; set; }
}