namespace CodeWay.Application.Features.Notifications.Commands;

using AutoMapper;
using CodeWay.Application.Contracts;
using CodeWay.Application.Features.Notifications.DTOs;
using CodeWay.Domain.Entities.Notifications;
using CodeWay.Domain.Exceptions;
using CodeWay.Domain.Interfaces;
using MediatR;

public sealed record GetNotificationsQuery(bool? UnreadOnly = null) : IRequest<IReadOnlyList<NotificationDto>>;

public sealed record MarkNotificationAsReadCommand(Guid Id) : IRequest<NotificationDto>;

public sealed record MarkAllNotificationsAsReadCommand : IRequest;

public sealed record DeleteNotificationCommand(Guid Id) : IRequest;

public sealed record SendNotificationCommand(CreateNotificationDto Dto) : IRequest<NotificationDto>;

public sealed class NotificationCommandHandler :
    IRequestHandler<GetNotificationsQuery, IReadOnlyList<NotificationDto>>,
    IRequestHandler<MarkNotificationAsReadCommand, NotificationDto>,
    IRequestHandler<MarkAllNotificationsAsReadCommand>,
    IRequestHandler<DeleteNotificationCommand>,
    IRequestHandler<SendNotificationCommand, NotificationDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public NotificationCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<NotificationDto>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId
            ?? throw new DomainException("Authentication required.");

        var notifications = await _unitOfWork.Notifications.GetAsync(n => n.UserId == userId, cancellationToken);

        var query = notifications.AsEnumerable();
        if (request.UnreadOnly.HasValue && request.UnreadOnly.Value)
        {
            query = query.Where(n => !n.IsRead);
        }

        return _mapper.Map<IReadOnlyList<NotificationDto>>(query.OrderByDescending(n => n.CreatedAtUtc).ToList());
    }

    public async Task<NotificationDto> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
    {
        var notification = await _unitOfWork.Notifications.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Notification", request.Id);

        notification.IsRead = true;
        notification.ReadAtUtc = DateTime.UtcNow;

        _unitOfWork.Notifications.Update(notification);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<NotificationDto>(notification);
    }

    public async Task Handle(MarkAllNotificationsAsReadCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId
            ?? throw new DomainException("Authentication required.");

        var unread = await _unitOfWork.Notifications.GetAsync(n => n.UserId == userId && !n.IsRead, cancellationToken);
        foreach (var n in unread)
        {
            n.IsRead = true;
            n.ReadAtUtc = DateTime.UtcNow;
            _unitOfWork.Notifications.Update(n);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task Handle(DeleteNotificationCommand request, CancellationToken cancellationToken)
    {
        var notification = await _unitOfWork.Notifications.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Notification", request.Id);

        _unitOfWork.Notifications.Remove(notification);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<NotificationDto> Handle(SendNotificationCommand request, CancellationToken cancellationToken)
    {
        var userExists = await _unitOfWork.Users.ExistsAsync(u => u.Id == request.Dto.UserId, cancellationToken);
        if (!userExists)
            throw new NotFoundException("User", request.Dto.UserId);

        var notification = new Notification
        {
            UserId = request.Dto.UserId,
            Title = request.Dto.Title.Trim(),
            Message = request.Dto.Message.Trim(),
            Type = request.Dto.Type,
            ActionUrl = request.Dto.ActionUrl?.Trim(),
            IsRead = false
        };

        await _unitOfWork.Notifications.AddAsync(notification, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<NotificationDto>(notification);
    }
}
