using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreWebapi.Application.Features.Orders;

namespace StoreWebapi.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class OrdersController(ISender mediator) : ControllerBase
{
    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout([FromBody] CreateOrderCommand command)
    {
        var result = await mediator.Send(command);
        
        return result.IsSuccess 
            ? Ok(result.Value) 
            : BadRequest(result.Error);
    }
    
}