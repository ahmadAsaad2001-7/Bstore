using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using StoreWebapi.Domain.Domain;
using StoreWebapi.Domain.Interfaces;

namespace StoreWebapi.Application.Features.User.GetMyLibrary;

public class GetMyLibraryHandler(IRepository repository,IHttpContextAccessor httpaccessor) :IRequestHandler<GetMyLibraryQuery,Result<List<GetMyLibraryResponse>>>
{
    public async Task<Result<List<GetMyLibraryResponse>>> Handle(GetMyLibraryQuery request, CancellationToken cancellationToken)
    {
        var UserIdClaim= httpaccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(UserIdClaim))
            return Result.Failure<List<GetMyLibraryResponse>>("Unauthorized behavior");
        var userId = Guid.Parse(UserIdClaim);
        
        var library = await repository.FindAll<UserBook>(ub=>ub.UserId == userId);
        var response = new List<GetMyLibraryResponse>();
        foreach (var entry in library)
        {
            var book = await repository.FindById<Book>(entry.BookId);
            if(book != null)
                response.Add(new GetMyLibraryResponse(book.id, book.name, entry.PurchaseDate));
        }

        return Result.Success(response);
    }
}