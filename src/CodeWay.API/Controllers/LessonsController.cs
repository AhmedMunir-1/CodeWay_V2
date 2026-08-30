namespace CodeWay.API.Controllers;

using CodeWay.API.Common;
using CodeWay.Application.Features.Catalog.Commands;
using CodeWay.Application.Features.Catalog.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class LessonsController : ControllerBase
{
    private readonly IMediator _mediator;

    public LessonsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<LessonDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var lesson = await _mediator.Send(new GetLessonByIdQuery(id), cancellationToken);
        return Ok(ApiResponse<LessonDto>.Success(lesson));
    }

    [HttpGet("section/{sectionId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<LessonDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBySectionId(Guid sectionId, CancellationToken cancellationToken)
    {
        var lessons = await _mediator.Send(new GetLessonsBySectionIdQuery(sectionId), cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<LessonDto>>.Success(lessons));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<LessonDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreateLessonDto dto, CancellationToken cancellationToken)
    {
        var created = await _mediator.Send(new CreateLessonCommand(dto), cancellationToken);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<LessonDto>.Success(created, "Lesson created successfully."));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<LessonDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateLessonDto dto, CancellationToken cancellationToken)
    {
        var updated = await _mediator.Send(new UpdateLessonCommand(id, dto), cancellationToken);
        return Ok(ApiResponse<LessonDto>.Success(updated, "Lesson updated successfully."));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteLessonCommand(id), cancellationToken);
        return Ok(ApiResponse.Success("Lesson deleted successfully."));
    }
}
