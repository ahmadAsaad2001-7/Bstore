using MediatR;

namespace StoreWebapi.Application.Features.User.GetMyLibrary;

public record GetMyLibraryQuery() : IRequest<Result<List<GetMyLibraryResponse>>>
{
   
    
    
}