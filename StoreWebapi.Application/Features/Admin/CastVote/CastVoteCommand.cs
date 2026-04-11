using MediatR;

namespace StoreWebapi.Application.Features.Admin.CastVote;

public record CastVoteCommand() : IRequest<Result<CastVoteResponse>>;
