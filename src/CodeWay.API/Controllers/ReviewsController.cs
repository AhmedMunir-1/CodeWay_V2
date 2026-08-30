namespace CodeWay.API.Controllers;

using CodeWay.API.Common;
using CodeWay.Application.Features.Learning.Commands;
using CodeWay.Application.Features.Learning.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReviewsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("course/{courseId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ReviewDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByCourseId(Guid courseId, CancellationToken cancellationToken)
    {
        var reviews = await _mediator.Send(new GetReviewsByCourseIdQuery(courseId), cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<ReviewDto>>.Success(reviews));
    }

    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ReviewDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateReviewDto dto, CancellationToken cancellationToken)
    {
        var created = await _mediator.Send(new CreateReviewCommand(dto), cancellationToken);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<ReviewDto>.Success(created, "Review submitted successfully."));
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ReviewDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateReviewDto dto, CancellationToken cancellationToken)
    {
        var updated = await _mediator.Send(new UpdateReviewCommand(id, dto), cancellationToken);
        return Ok(ApiResponse<ReviewDto>.Success(updated, "Review updated successfully."));
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteReviewCommand(id), cancellationToken);
        return Ok(ApiResponse.Success("Review deleted successfully."));
    }
}
