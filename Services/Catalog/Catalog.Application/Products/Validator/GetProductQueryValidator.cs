
using Catalog.Application.Products.Queries;
using FluentValidation;
using Platform.Core.Services.Localization;

public class GetProductQueryValidator
    : AbstractValidator<GetProductQuery>
{
    private readonly ILocalizationService _localizationService;

    public GetProductQueryValidator(
        ILocalizationService localizationService)
    {
        _localizationService = localizationService;

        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(_localizationService.Get("ProductIdRequired"));
    }
}