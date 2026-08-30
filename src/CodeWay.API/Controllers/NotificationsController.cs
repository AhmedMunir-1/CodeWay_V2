namespace CodeWay.API.Controllers;

using CodeWay.API.Common;
using CodeWay.Application.Features.Notifications.Commands;
using CodeWay.Application.Features.Notifications.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<NotificationDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] bool? unreadOnly, CancellationToken cancellationToken)
    {
        var notifications = await _mediator.Send(new GetNotificationsQuery(unreadOnly), cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<NotificationDto>>.Success(notifications));
    }

    [HttpPatch("{id:guid}/read")]
    [ProducesResponseType(typeof(ApiResponse<NotificationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsRead(Guid id, CancellationToken cancellationToken)
    {
        var notification = await _mediator.Send(new MarkNotificationAsReadCommand(id), cancellationToken);
        return Ok(ApiResponse<NotificationDto>.Success(notification, "Notification marked as read."));
    }

    [HttpPatch("read-all")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> MarkAllAsRead(CancellationToken cancellationToken)
    {
        await _mediator.Send(new MarkAllNotificationsAsReadCommand(), cancellationToken);
        return Ok(ApiResponse.Success("All notifications marked as read."));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteNotificationCommand(id), cancellationToken);
        return Ok(ApiResponse.Success("Notification deleted."));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<NotificationDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Send([FromBody] CreateNotificationDto dto, CancellationToken cancellationToken)
    {
        var created = await _mediator.Send(new SendNotificationCommand(dto), cancellationToken);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<NotificationDto>.Success(created, "Notification sent."));
    }
}
