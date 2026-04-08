namespace StoreWebapi.Application.Features.Orders;

public record CreateOrderResponse(
    Guid OrderId, 
    string CheckoutUrl, 
    bool IsFree 
);