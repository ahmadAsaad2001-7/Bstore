
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using StoreWebapi.Application.Common;
using StoreWebapi.Domain.Domain;
using StoreWebapi.Domain.Domain.Enums;
using StoreWebapi.Domain.Interfaces;

namespace StoreWebapi.Application.Features.Orders;


public class CreateOrderHandler(
    IRepository repo, 
    IHttpContextAccessor httpContextAccessor,
    IPaymentService paymentService) 
    : IRequestHandler<CreateOrderCommand, Result<CreateOrderResponse>>
{
    public async Task<Result<CreateOrderResponse>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // 1. Authenticate Buyer
        var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Result.Failure<CreateOrderResponse>("User is not authenticated.");
        var buyerId = Guid.Parse(userIdClaim.Value);

        var books = new List<Book>();
        foreach (var id in request.BookIds)
        {
            var book = await repo.FindById<Book>(id);
            if (book != null) books.Add(book);
        }

        if (!books.Any()) return Result.Failure<CreateOrderResponse>("No valid books found.");

        // 3. Process Coupon (if provided)
        decimal discountMultiplier = 1.0m; 
        
        coupons? appliedCoupon = null;

        if (!string.IsNullOrWhiteSpace(request.CouponCode))
        {
            var matchingCoupons = await repo.FindAll<coupons>(c => c.code == request.CouponCode);
            appliedCoupon = matchingCoupons.FirstOrDefault();

            if (appliedCoupon == null)
                return Result.Failure<CreateOrderResponse>("Invalid coupon code.");
            
            if (appliedCoupon.IsExpired)
                return Result.Failure<CreateOrderResponse>("Coupon is expired.");

            discountMultiplier = 1m - (appliedCoupon.Discount_percentage / 100m);
            
        }

        var orderId = Guid.NewGuid();
        var order = new Order 
        {
            Id = orderId,
            BuyerId = buyerId,
            OrderDate = DateTime.UtcNow,
            Status = PaymentStatus.Pending.ToString(),
            Transactions = new List<transaction>()
        };

        decimal totalOrderAmount = 0;

        foreach (var book in books)
        {
            if (book.UserId == null) 
                return Result.Failure<CreateOrderResponse>($"Book {book.name} has no valid vendor.");

            var discountedPrice = book.price * discountMultiplier;
            totalOrderAmount += discountedPrice;

            var newTransaction = new transaction
            {
                id = Guid.NewGuid(),
                OrderId = orderId,        // Link to parent order
                userId = buyerId,
                vendorId = book.UserId.Value,
                bookId = book.id,
                amount = discountedPrice, // Store the EXACT price they are paying right now
                currency = "usd",
                TransactionDate = DateTime.UtcNow,
                destination = "Digital",  // Or request.Address
                Status = PaymentStatus.Pending
            };

            order.Transactions.Add(newTransaction);
        }
        
        order.TotalAmount = totalOrderAmount;

        // 6. Handle the "100% Free" Edge Case
        if (totalOrderAmount <= 0)
        {
            order.Status = "Paid"; // Skip Stripe, it's free!
            foreach (var t in order.Transactions) t.Status = PaymentStatus.Paid;

            repo.Add(order);
            await repo.SaveChangesAsync(cancellationToken);

            return Result.Success(new CreateOrderResponse(orderId, "Free", true));
        }

        // 7. Call Stripe for Payment
        // Stripe expects the order to generate the checkout session
        var stripeResult = await paymentService.CreateCheckoutSessionAsync(order, order.Transactions.ToList());
        if (stripeResult.IsFailure)
            return Result.Failure<CreateOrderResponse>("Payment gateway error: " + stripeResult.Error);

        // Save Stripe Session ID so the Webhook can find this order later
        order.StripeSessionId = stripeResult.Value.SessionId; 

        // 8. Save to Database
        repo.Add(order);
        await repo.SaveChangesAsync(cancellationToken);

        // 9. Return the URL to the Controller
        return Result.Success(new CreateOrderResponse(orderId, stripeResult.Value.CheckoutUrl, false));
    }
}
