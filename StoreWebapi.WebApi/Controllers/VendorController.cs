using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreWebapi.Application.Features.Vendor.Sell;

namespace StoreWebapi.Api.Controllers;
[Authorize(Roles = "Vendor")]
[ApiController]
[Route("[controller]")]
public class VendorController : ControllerBase
{
    private readonly ISender _mediator;

    public VendorController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> SellBookController([FromBody] SellBookCommand command)
    {
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return  BadRequest(result.Error);
        }
        return Ok(result.Value);
        
    }

}