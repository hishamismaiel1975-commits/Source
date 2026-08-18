
using Catalog.Application.Products.Queries;
using FluentValidation;

public class GetProductQueryValidator
    : AbstractValidator<GetProductQuery>
{
    public GetProductQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("ProductIdRequired");
    }
}