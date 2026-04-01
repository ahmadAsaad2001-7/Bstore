using MediatR;
using Microsoft.AspNetCore.Mvc;
using StoreWebapi.Application.Features.Books.Queries;
using StoreWebapi.Application.Features.Books.SearchBooks;

namespace StoreWebapi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly ISender _mediator; 


    public BooksController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetBook([FromRoute] Guid id)
    {
        // Log the received id
        Console.WriteLine($"Received id: {id}");
        Console.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage);
            return BadRequest(new { errors });
        }

      
        var query = new GetBookByIdQuery(id);
        Console.WriteLine($"Controller created query with Id: {query.Id}");

        try
        {
            var result = await _mediator.Send(query);


            if (result.IsFailure)
            {

                return result.Error!.Contains("not found")
                    ? NotFound(result.Error)
                    : BadRequest(result.Error);
            }

            return Ok(result.Value);
        } catch(Exception ex)
        {
            return StatusCode(500, $"Internal error: {ex.Message}");
        }
    }
}