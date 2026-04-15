using MediatR;
using StoreWebapi.Domain.Domain;
using StoreWebapi.Domain.Interfaces;

namespace StoreWebapi.Application.Features.User.GetNearByVendors;

public class GetNearByVendorsHandler(IGeoLocationService geoService, IRepository repo) 
    : IRequestHandler<GetNearByVendorsQuery, Result<List<GetNearByVendorsResponse>>>
{
    public async Task<Result<List<GetNearByVendorsResponse>>> Handle(GetNearByVendorsQuery request, CancellationToken cancellationToken)
    {
        // 1. Get user location from IP
        var locResult = await geoService.GetLocationFromIpAsync(request.UserIpAddress);
        
        // Handle null or failure from the geoService
        if (locResult == null || locResult.IsFailure)
            return Result.Failure<List<GetNearByVendorsResponse>>("Could not determine your location.");
        
        // 2. Encode User's location to a CellId
        var userCellResult = geoService.EncodeToCellId(locResult.Value.lat, locResult.Value.lon, 13);
        if (userCellResult.IsFailure) 
            return Result.Failure<List<GetNearByVendorsResponse>>(userCellResult.Error);

        string userCell = userCellResult.Value;

        // 3. Define search area (Center cell + 8 neighbors)
        var searchArea = new List<string> { userCell };
        var neighborsResult = geoService.GetNeighborCellIds(userCell);
        
        if (neighborsResult.IsSuccess)
        {
            searchArea.AddRange(neighborsResult.Value);
        }

        // 4. Query DB for users (vendors) within those cells
        // Note: Filter by u.cellId != null to avoid errors with Contains
        var nearbyVendors = await repo.FindAll<user>(u => 
           u.cellId != null && searchArea.Contains(u.cellId)
        );

        // 5. Map the domain 'user' to the 'GetNearByVendorsResponse' DTO
        var response = nearbyVendors.Select(v => new GetNearByVendorsResponse
        {
           VendorId = v.Id,
           VendorName = v.UserName ?? "Unknown Vendor" 
        }).ToList();

        // 6. Return the successful result
        return Result.Success(response);
    }
}