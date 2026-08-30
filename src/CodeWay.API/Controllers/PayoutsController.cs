namespace CodeWay.API.Controllers;

using CodeWay.API.Common;
using CodeWay.Application.Features.Instructor.Commands;
using CodeWay.Application.Features.Instructor.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PayoutsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PayoutsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<PayoutRequestDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] Guid? instructorId, CancellationToken cancellationToken)
    {
        var payouts = await _mediator.Send(new GetPayoutRequestsQuery(instructorId), cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<PayoutRequestDto>>.Success(payouts));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<PayoutRequestDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create([FromBody] CreatePayoutRequestDto dto, CancellationToken cancellationToken)
    {
        var payout = await _mediator.Send(new CreatePayoutRequestCommand(dto), cancellationToken);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<PayoutRequestDto>.Success(payout, "Payout request submitted."));
    }

    [HttpPut("{id:guid}/process")]
    [ProducesResponseType(typeof(ApiResponse<PayoutRequestDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Process(Guid id, [FromBody] ProcessPayoutRequestDto dto, CancellationToken cancellationToken)
    {
        var processed = await _mediator.Send(new ProcessPayoutRequestCommand(id, dto), cancellationToken);
        return Ok(ApiResponse<PayoutRequestDto>.Success(processed, $"Payout request {dto.Status}."));
    }
}
