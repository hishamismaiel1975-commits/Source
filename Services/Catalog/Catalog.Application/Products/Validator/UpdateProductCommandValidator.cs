using Catalog.Application.Products.Commands;
using FluentValidation;
using Platform.Core.Services.Localization;

public class UpdateProductCommandValidator
    : AbstractValidator<UpdateProductCommand>
{
    private readonly ILocalizationService _localizationService;

    public UpdateProductCommandValidator(
        ILocalizationService localizationService)
    {
        _localizationService = localizationService;

        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(_localizationService.Get("ProductIdRequired"));

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(_localizationService.Get("ProductNameRequired"))
            .MaximumLength(200)
            .WithMessage(_localizationService.Get("ProductNameMaxLength"));

        RuleFor(x => x.Summary)
            .NotEmpty()
            .WithMessage(_localizationService.Get("ProductSummaryRequired"))
            .MaximumLength(500)
            .WithMessage(_localizationService.Get("ProductSummaryMaxLength"));

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(_localizationService.Get("ProductDescriptionRequired"));

        RuleFor(x => x.ImageFile)
            .NotEmpty()
            .WithMessage(_localizationService.Get("ProductImageFileRequired"));

        RuleFor(x => x.BrandId)
            .NotEmpty()
            .WithMessage(_localizationService.Get("ProductBrandIdRequired"));

        RuleFor(x => x.TypeId)
            .NotEmpty()
            .WithMessage(_localizationService.Get("ProductTypeIdRequired"));

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage(_localizationService.Get("ProductPricePositive"));
    }
}