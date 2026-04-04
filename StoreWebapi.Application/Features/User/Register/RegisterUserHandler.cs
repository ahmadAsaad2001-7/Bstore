using MediatR;
using Microsoft.AspNetCore.Identity;
using StoreWebapi.Application.Common;
using StoreWebapi.Application.Features.Books.Commands.Create;
using StoreWebapi.Domain.Domain;
using StoreWebapi.Domain.Domain.Enums;

namespace StoreWebapi.Application.Features.User;

public class RegisterUserHandler :IRequestHandler<RegisterUserCommand,Result<Guid>>
{
    private readonly UserManager<user> _userManager;
    public RegisterUserHandler(UserManager<user> userManager)
    {
        _userManager = userManager;
    }
    public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {

        var existing = await _userManager.FindByEmailAsync(request.Email);
        if (existing != null)
            return Result.Failure<Guid>("User already exists with this email.");

        var NewUser = new user
        {
            Id = Guid.NewGuid(),
            UserName = string.IsNullOrWhiteSpace(request.UserName) ? request.Email : request.UserName,
            Email = request.Email,
            imageUrl = request.ImageUrl ?? string.Empty,
            isSusbended = false,
            Transactions = new List<transaction>(),
            VendorTransactions = new List<transaction>(),
            Comments = new List<comment>(),
            CouponUsers = new List<couponUser>(),
            VotesInitiated = new List<vote>(),
            ParticipatingVotes = new List<voteParticipant>()
        };

        var createResult = await _userManager.CreateAsync(NewUser, request.Password);
    
        if (!createResult.Succeeded)
        {
            var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
      
            return Result.Failure<Guid>($"User creation failed: {errors}");
        }

        var RoleResult = await _userManager.AddToRoleAsync(NewUser, Roles.USER.ToString());
    
        if (!RoleResult.Succeeded)
        {
            await _userManager.DeleteAsync(NewUser); 
            var RoleErrors = string.Join("; ", RoleResult.Errors.Select(e => e.Description));
          
            return Result.Failure<Guid>($"Failed to add role: {RoleErrors}");
        }

        // 4. FIX: Corrected Result syntax
        return Result.Success(NewUser.Id);
    }
}