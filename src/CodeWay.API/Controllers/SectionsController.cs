namespace CodeWay.API.Controllers;

using CodeWay.API.Common;
using CodeWay.Application.Features.Catalog.Commands;
using CodeWay.Application.Features.Catalog.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class SectionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SectionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("course/{courseId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<SectionDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByCourseId(Guid courseId, CancellationToken cancellationToken)
    {
        var sections = await _mediator.Send(new GetSectionsByCourseIdQuery(courseId), cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<SectionDto>>.Success(sections));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<SectionDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreateSectionDto dto, CancellationToken cancellationToken)
    {
        var created = await _mediator.Send(new CreateSectionCommand(dto), cancellationToken);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<SectionDto>.Success(created, "Section created successfully."));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<SectionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSectionDto dto, CancellationToken cancellationToken)
    {
        var updated = await _mediator.Send(new UpdateSectionCommand(id, dto), cancellationToken);
        return Ok(ApiResponse<SectionDto>.Success(updated, "Section updated successfully."));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteSectionCommand(id), cancellationToken);
        return Ok(ApiResponse.Success("Section deleted successfully."));
    }
}
