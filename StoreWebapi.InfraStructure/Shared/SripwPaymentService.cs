using Microsoft.Extensions.Configuration;
using StoreWebapi.Domain.Common;
using StoreWebapi.Domain.Domain;
using StoreWebapi.Domain.Interfaces;
using Stripe;
using Stripe.Checkout;

namespace StoreWebapi.Infrastructure.Shared;

public class StripePaymentService : IPaymentService
{
    private readonly string _secretKey;

    public StripePaymentService(IConfiguration config)
    {
        _secretKey = config["Stripe:SecretKey"] ?? throw new ArgumentNullException("Stripe Secret Key missing");
        StripeConfiguration.ApiKey = _secretKey;
    }

    public async Task<Result<(string SessionId, string CheckoutUrl)>> CreateCheckoutSessionAsync(Order order, List<transaction> transactions)
    {
        try
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = transactions.Select(t => new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        // Stripe uses cents ($20.00 = 2000)
                        UnitAmount = (long)(t.amount * 100), 
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions 
                        { 
                            Name = t.book?.name ?? "Book Purchase" 
                        }
                    },
                    Quantity = 1
                }).ToList(),
                Mode = "payment",
                
                SuccessUrl = "that's my url here i need to add" + order.Id,
                CancelUrl = "here my cancellation url",
                ClientReferenceId = order.Id.ToString()
            };

            var service = new SessionService();
            Session session = await service.CreateAsync(options);

            return Result.Success((session.Id, session.Url));
        }
        catch (StripeException ex)
        {
            return Result.Failure<(string, string)>($"Stripe error: {ex.Message}");
        }
    }
}