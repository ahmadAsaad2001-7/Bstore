using MediatR;
using Microsoft.AspNetCore.Mvc;
using StoreWebapi.Application.Features.User;

namespace StoreWebapi.Api.Controllers;
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
    {
        var result = await _mediator.Send(command);
        if (result.IsSuccess)
            return Ok(new { userId = result.Value });
        return BadRequest(new { error = result.Error });
    }
    
}