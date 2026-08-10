using Catalog.Application.Products.Commands;
using FluentValidation;

public class UpdateProductCommandValidator
    : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {

        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("ProductIdRequired");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("ProductNameRequired")
            .MaximumLength(200)
            .WithMessage("ProductNameMaxLength");

        RuleFor(x => x.Summary)
            .NotEmpty()
            .WithMessage("ProductSummaryRequired")
            .MaximumLength(500)
            .WithMessage("ProductSummaryMaxLength");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("ProductDescriptionRequired");

        RuleFor(x => x.ImageFile)
            .NotEmpty()
            .WithMessage("ProductImageFileRequired");

        RuleFor(x => x.BrandId)
            .NotEmpty()
            .WithMessage("ProductBrandIdRequired");

        RuleFor(x => x.TypeId)
            .NotEmpty()
            .WithMessage("ProductTypeIdRequired");

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("ProductPricePositive");
    }
}