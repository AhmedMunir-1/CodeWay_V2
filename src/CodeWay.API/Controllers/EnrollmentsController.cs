namespace CodeWay.API.Controllers;

using CodeWay.API.Common;
using CodeWay.Application.Features.Learning.Commands;
using CodeWay.Application.Features.Learning.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class EnrollmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public EnrollmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<EnrollmentDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] Guid? userId, [FromQuery] Guid? courseId, CancellationToken cancellationToken)
    {
        var enrollments = await _mediator.Send(new GetEnrollmentsQuery(userId, courseId), cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<EnrollmentDto>>.Success(enrollments));
    }

    [Authorize]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var enrollment = await _mediator.Send(new GetEnrollmentByIdQuery(id), cancellationToken);
        return Ok(ApiResponse<EnrollmentDto>.Success(enrollment));
    }

    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Enroll([FromBody] CreateEnrollmentDto dto, CancellationToken cancellationToken)
    {
        var enrollment = await _mediator.Send(new EnrollCourseCommand(dto.CourseId), cancellationToken);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<EnrollmentDto>.Success(enrollment, "Enrolled in course successfully."));
    }

    [Authorize]
    [HttpPut("progress")]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProgress([FromBody] UpdateLessonProgressDto dto, CancellationToken cancellationToken)
    {
        var enrollment = await _mediator.Send(new UpdateLessonProgressCommand(dto), cancellationToken);
        return Ok(ApiResponse<EnrollmentDto>.Success(enrollment, "Lesson progress updated successfully."));
    }

    [Authorize]
    [HttpGet("{id:guid}/certificate")]
    [ProducesResponseType(typeof(ApiResponse<CertificateDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCertificate(Guid id, CancellationToken cancellationToken)
    {
        var certificate = await _mediator.Send(new GetCertificateQuery(id), cancellationToken);
        return Ok(ApiResponse<CertificateDto>.Success(certificate));
    }
}
