using StoreWebapi.Domain.Domain;

namespace StoreWebapi.Domain.Interfaces;

public interface IPaymentService
{
    Task<Result<(string SessionId, string CheckoutUrl)>> CreateCheckoutSessionAsync(Order order, List<transaction> transactions);
}
