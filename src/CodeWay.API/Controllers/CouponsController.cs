namespace CodeWay.API.Controllers;

using CodeWay.API.Common;
using CodeWay.Application.Features.Commerce.Commands;
using CodeWay.Application.Features.Commerce.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class CouponsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CouponsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<CouponDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var coupons = await _mediator.Send(new GetCouponsQuery(), cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<CouponDto>>.Success(coupons));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CouponDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var coupon = await _mediator.Send(new GetCouponByIdQuery(id), cancellationToken);
        return Ok(ApiResponse<CouponDto>.Success(coupon));
    }

    [HttpGet("validate")]
    [ProducesResponseType(typeof(ApiResponse<CouponValidationResultDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Validate([FromQuery] string code, [FromQuery] decimal cartTotal, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ValidateCouponQuery(code, cartTotal), cancellationToken);
        return Ok(ApiResponse<CouponValidationResultDto>.Success(result));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CouponDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateCouponDto dto, CancellationToken cancellationToken)
    {
        var created = await _mediator.Send(new CreateCouponCommand(dto), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<CouponDto>.Success(created, "Coupon created successfully."));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CouponDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCouponDto dto, CancellationToken cancellationToken)
    {
        var updated = await _mediator.Send(new UpdateCouponCommand(id, dto), cancellationToken);
        return Ok(ApiResponse<CouponDto>.Success(updated, "Coupon updated successfully."));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteCouponCommand(id), cancellationToken);
        return Ok(ApiResponse.Success("Coupon deleted successfully."));
    }
}
