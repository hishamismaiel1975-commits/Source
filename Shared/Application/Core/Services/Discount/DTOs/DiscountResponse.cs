namespace Application.Lib.Core.Services.Discount.DTOs
{
    public record DiscountResponse
    (
        Guid ProductId,
        double Amount
        );
}
