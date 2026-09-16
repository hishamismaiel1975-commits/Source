namespace Application.Lib.Core.DTOs.Discount
{
    public record DiscountResponse
    (
        Guid ProductId,
        double Amount
        );
}
