using MediatR;
using StoreWebapi.Domain.Domain;
using StoreWebapi.Domain.Domain.Enums;
using StoreWebapi.Domain.Interfaces;

namespace StoreWebapi.Application.Features.Orders.ConfirmOrderPayment;

public class ConfirmOrderPaymentHandler(IRepository repo) : IRequestHandler<ConfirmOrderPaymentCommand, Result>
    {
        public async Task<Result> Handle(ConfirmOrderPaymentCommand request, CancellationToken ct)
        {
            // fetch order
            var order = await repo.FindById<Order>(request.OrderId);
        
            if (order == null) 
                return Result.Failure("Order not found.");

            if (order.Status == "Paid") 
                return Result.Success(); 

            // 2. Update Statuses
            order.Status = "Paid";
        
            
            if (order.Transactions != null)
            {
                foreach (var transaction in order.Transactions)
                {
                    transaction.Status = PaymentStatus.Paid;
                }
            }

            // 3. Persist Changes
            await repo.SaveChangesAsync(ct);

            return Result.Success();
        }
}