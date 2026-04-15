using MediatR;

namespace StoreWebapi.Application.Features.User.GetNearByVendors;

public record GetNearByVendorsQuery(string UserIpAddress):IRequest<Result<GetNearByVendorsResponse>>, IRequest<Result<List<GetNearByVendorsResponse>>>;