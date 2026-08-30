namespace CodeWay.API.Controllers;

using CodeWay.API.Common;
using CodeWay.Application.Features.Payments.DTOs;
using CodeWay.Application.Features.Payments.Queries;
using CodeWay.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PaymentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<PaymentDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] Guid? userId, [FromQuery] PaymentStatus? status, CancellationToken cancellationToken)
    {
        var payments = await _mediator.Send(new GetPaymentsQuery(userId, status), cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<PaymentDto>>.Success(payments));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PaymentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var payment = await _mediator.Send(new GetPaymentByIdQuery(id), cancellationToken);
        return Ok(ApiResponse<PaymentDto>.Success(payment));
    }
}
