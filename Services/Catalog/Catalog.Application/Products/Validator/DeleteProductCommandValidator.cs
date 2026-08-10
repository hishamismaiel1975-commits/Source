using Catalog.Application.Products.Commands;
using FluentValidation;
using Platform.Core.Services.Localization;

public class DeleteProductCommandValidator
    : AbstractValidator<DeleteProductCommand>
{
    private readonly ILocalizationService _localizationService;

    public DeleteProductCommandValidator(
        ILocalizationService localizationService)
    {
        _localizationService = localizationService;

        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(_localizationService.Get("ProductIdRequired"));
    }
}