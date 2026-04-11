using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreWebapi.Application.Features.Admin.CastVote;

namespace StoreWebapi.Api.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Adminstrator")]
public class AdminController :ControllerBase
{
    private readonly ISender  _mediator;

    public AdminController(ISender mediator)
    {
        _mediator = mediator;
    }

   
    [HttpPost("votes/{voteId:guid}/cast")]
    public async Task<IActionResult> CastVote([FromRoute] Guid voteId, [FromBody] bool IsApproved)
    {
      
        var adminIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (adminIdClaim == null) return Unauthorized();

        var adminId = Guid.Parse(adminIdClaim.Value);

        var command = new CastVoteCommand(voteId, adminId, IsApproved);
        
        var result = await _mediator.Send(command);

        if (result.IsFailure) return BadRequest(result.Error);

        return Ok(result.Value);
    }
}