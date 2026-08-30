namespace CodeWay.API.Controllers;

using CodeWay.API.Common;
using CodeWay.Application.Features.Instructor.Commands;
using CodeWay.Application.Features.Instructor.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class InstructorsController : ControllerBase
{
    private readonly IMediator _mediator;

    public InstructorsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<InstructorProfileDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] bool? approvedOnly, CancellationToken cancellationToken)
    {
        var profiles = await _mediator.Send(new GetInstructorProfilesQuery(approvedOnly), cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<InstructorProfileDto>>.Success(profiles));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<InstructorProfileDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var profile = await _mediator.Send(new GetInstructorProfileByIdQuery(id), cancellationToken);
        return Ok(ApiResponse<InstructorProfileDto>.Success(profile));
    }

    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(ApiResponse<InstructorProfileDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyProfile(CancellationToken cancellationToken)
    {
        var profile = await _mediator.Send(new GetMyInstructorProfileQuery(), cancellationToken);
        return Ok(ApiResponse<InstructorProfileDto>.Success(profile));
    }

    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<InstructorProfileDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateInstructorProfileDto dto, CancellationToken cancellationToken)
    {
        var profile = await _mediator.Send(new CreateInstructorProfileCommand(dto), cancellationToken);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<InstructorProfileDto>.Success(profile, "Instructor profile created successfully."));
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<InstructorProfileDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateInstructorProfileDto dto, CancellationToken cancellationToken)
    {
        var updated = await _mediator.Send(new UpdateInstructorProfileCommand(id, dto), cancellationToken);
        return Ok(ApiResponse<InstructorProfileDto>.Success(updated, "Instructor profile updated successfully."));
    }

    [HttpPatch("{id:guid}/approve")]
    [ProducesResponseType(typeof(ApiResponse<InstructorProfileDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Approve(Guid id, CancellationToken cancellationToken)
    {
        var approved = await _mediator.Send(new ApproveInstructorProfileCommand(id), cancellationToken);
        return Ok(ApiResponse<InstructorProfileDto>.Success(approved, "Instructor profile approved."));
    }

    [Authorize]
    [HttpGet("wallet")]
    [ProducesResponseType(typeof(ApiResponse<InstructorWalletDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetWallet([FromQuery] Guid? instructorId, CancellationToken cancellationToken)
    {
        var wallet = await _mediator.Send(new GetInstructorWalletQuery(instructorId), cancellationToken);
        return Ok(ApiResponse<InstructorWalletDto>.Success(wallet));
    }
}
