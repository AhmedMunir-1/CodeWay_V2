namespace CodeWay.Application.Features.Identity.Queries;

using CodeWay.Application.Features.Identity.DTOs;
using MediatR;

/// <summary>Query to get current authenticated user profile.</summary>
public sealed record GetCurrentUserQuery : IRequest<UserProfileDto>;
