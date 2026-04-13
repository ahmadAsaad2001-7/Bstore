        using MediatR;
        using Microsoft.AspNetCore.Mvc;
        using StoreWebapi.Application.Features.Orders.ConfirmOrderPayment;
        using Stripe;

        namespace StoreWebapi.Api.Controllers;

        [ApiController]
        [Route("api/webhooks/stripe")]
        public class StripeWebhookController(ISender mediator, IConfiguration config) : ControllerBase
        {
            [HttpPost]
            public async Task<IActionResult> Handle()
            {
                var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
                var signature = Request.Headers["Stripe-Signature"];
                var webhookSecret = config["Stripe:WebhookSecret"]; 

                try
                {
                    var stripeEvent = EventUtility.ConstructEvent(json, signature, webhookSecret);

                    if (stripeEvent.Type == "checkout.session.completed")
                    {
                        var session = stripeEvent.Data.Object as Stripe.Checkout.Session;
            
                        
                        if (session != null && Guid.TryParse(session.ClientReferenceId, out var orderId))
                        {
                            var result = await mediator.Send(new ConfirmOrderPaymentCommand(orderId));
                
                            if (result.IsFailure) 
                            {
                                
                                Console.WriteLine($"Webhook Error: {result.Error}");
                                return Ok(); 
                            }
                        }
                    }

                    return Ok();
                }
                catch (StripeException ex)
                {
                    
                    return BadRequest(ex.Message);
                }
            }
        }