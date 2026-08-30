namespace CodeWay.API.Controllers;

using CodeWay.API.Common;
using CodeWay.Application.Common;
using CodeWay.Application.Features.Catalog.Commands;
using CodeWay.Application.Features.Catalog.DTOs;
using CodeWay.Application.Features.Catalog.Queries;
using CodeWay.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CoursesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<CourseDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] Guid? categoryId,
        [FromQuery] Guid? instructorId,
        [FromQuery] CourseLevel? level,
        [FromQuery] CourseStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetCoursesQuery(search, categoryId, instructorId, level, status, page, pageSize, sortBy, sortDescending);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(ApiResponse<PaginatedResult<CourseDto>>.Success(result));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CourseDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var course = await _mediator.Send(new GetCourseByIdQuery(id), cancellationToken);
        return Ok(ApiResponse<CourseDetailDto>.Success(course));
    }

    [HttpGet("slug/{slug}")]
    [ProducesResponseType(typeof(ApiResponse<CourseDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBySlug(string slug, CancellationToken cancellationToken)
    {
        var course = await _mediator.Send(new GetCourseBySlugQuery(slug), cancellationToken);
        return Ok(ApiResponse<CourseDetailDto>.Success(course));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CourseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateCourseDto dto, CancellationToken cancellationToken)
    {
        var created = await _mediator.Send(new CreateCourseCommand(dto), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<CourseDto>.Success(created, "Course created successfully."));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CourseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCourseDto dto, CancellationToken cancellationToken)
    {
        var updated = await _mediator.Send(new UpdateCourseCommand(id, dto), cancellationToken);
        return Ok(ApiResponse<CourseDto>.Success(updated, "Course updated successfully."));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteCourseCommand(id), cancellationToken);
        return Ok(ApiResponse.Success("Course deleted successfully (soft delete)."));
    }

    [HttpPatch("{id:guid}/publish")]
    [ProducesResponseType(typeof(ApiResponse<CourseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Publish(Guid id, CancellationToken cancellationToken)
    {
        var published = await _mediator.Send(new PublishCourseCommand(id), cancellationToken);
        return Ok(ApiResponse<CourseDto>.Success(published, "Course published successfully."));
    }
}
