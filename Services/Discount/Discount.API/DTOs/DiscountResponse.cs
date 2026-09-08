namespace Discount.API.DTOs
{
    public record DiscountResponse
    (
        Guid ProductId,
        double Amount
        );
}
