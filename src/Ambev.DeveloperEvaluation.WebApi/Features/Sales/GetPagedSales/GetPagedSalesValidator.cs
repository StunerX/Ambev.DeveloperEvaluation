using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetPagedSales;

public class GetPagedSalesValidator : AbstractValidator<GetPagedSalesRequest>
{
    public GetPagedSalesValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.PageSize).GreaterThan(0);
    }
}