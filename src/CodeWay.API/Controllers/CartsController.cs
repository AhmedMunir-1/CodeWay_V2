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
public class CartsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CartsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<CartDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCart(CancellationToken cancellationToken)
    {
        var cart = await _mediator.Send(new GetCartQuery(), cancellationToken);
        return Ok(ApiResponse<CartDto>.Success(cart));
    }

    [HttpPost("items")]
    [ProducesResponseType(typeof(ApiResponse<CartDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddItem([FromBody] AddToCartDto dto, CancellationToken cancellationToken)
    {
        var cart = await _mediator.Send(new AddToCartCommand(dto), cancellationToken);
        return Ok(ApiResponse<CartDto>.Success(cart, "Item added to cart."));
    }

    [HttpDelete("items/{cartItemId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CartDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveItem(Guid cartItemId, CancellationToken cancellationToken)
    {
        var cart = await _mediator.Send(new RemoveFromCartCommand(cartItemId), cancellationToken);
        return Ok(ApiResponse<CartDto>.Success(cart, "Item removed from cart."));
    }

    [HttpDelete]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Clear(CancellationToken cancellationToken)
    {
        await _mediator.Send(new ClearCartCommand(), cancellationToken);
        return Ok(ApiResponse.Success("Cart cleared."));
    }
}
