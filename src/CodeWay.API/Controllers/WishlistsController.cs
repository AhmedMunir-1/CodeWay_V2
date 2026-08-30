namespace CodeWay.API.Controllers;

using CodeWay.API.Common;
using CodeWay.Application.Features.Commerce.Commands;
using CodeWay.Application.Features.Commerce.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class WishlistsController : ControllerBase
{
    private readonly IMediator _mediator;

    public WishlistsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<WishlistDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetWishlist(CancellationToken cancellationToken)
    {
        var wishlist = await _mediator.Send(new GetWishlistQuery(), cancellationToken);
        return Ok(ApiResponse<WishlistDto>.Success(wishlist));
    }

    [HttpPost("items")]
    [ProducesResponseType(typeof(ApiResponse<WishlistDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddItem([FromBody] AddToWishlistDto dto, CancellationToken cancellationToken)
    {
        var wishlist = await _mediator.Send(new AddToWishlistCommand(dto), cancellationToken);
        return Ok(ApiResponse<WishlistDto>.Success(wishlist, "Item added to wishlist."));
    }

    [HttpDelete("items/{courseId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<WishlistDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RemoveItem(Guid courseId, CancellationToken cancellationToken)
    {
        var wishlist = await _mediator.Send(new RemoveFromWishlistCommand(courseId), cancellationToken);
        return Ok(ApiResponse<WishlistDto>.Success(wishlist, "Item removed from wishlist."));
    }
}
