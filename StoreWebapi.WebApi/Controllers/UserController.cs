using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using StoreWebapi.Application.Features.User;
using StoreWebapi.Application.Features.User.ApplyforVendorRole;

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

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserCommand loginRequest)
    {
        var result = await _mediator.Send(loginRequest);
        if(result.IsSuccess)
            return Ok(result.Value);
        return BadRequest(new { error = result.Error });
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
    {
        var result = await _mediator.Send(command);
        if (result.IsSuccess)
            return Ok(new { userId = result.Value });
        return Unauthorized(new { error = result.Error });
    }
    [Authorize(Roles = "User")] 
    [HttpPost("apply-for-vendor")]
    public async Task<IActionResult> ApplyForVendor()
    {
     
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized("User ID not found in claims.");

        var userId = Guid.Parse(userIdClaim.Value);

       
        var command = new ApplyForVendorCommand(userId);
    
        var result = await _mediator.Send(command);

        return result.IsSuccess 
            ? Ok(new { VoteId = result.Value, Message = "Application submitted to admins." }) 
            : BadRequest(result.Error);
    }
    
}