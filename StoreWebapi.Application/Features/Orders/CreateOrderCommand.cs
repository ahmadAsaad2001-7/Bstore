using MediatR;
using StoreWebapi.Application.Common;

namespace StoreWebapi.Application.Features.Orders;

public record CreateOrderCommand(
    List<Guid> BookIds, 
    string? CouponCode
) : IRequest<Result<CreateOrderResponse>>;