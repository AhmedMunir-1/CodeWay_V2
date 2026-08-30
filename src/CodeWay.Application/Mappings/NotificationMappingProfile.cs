namespace CodeWay.Application.Mappings;

using AutoMapper;
using CodeWay.Application.Features.Notifications.DTOs;
using CodeWay.Domain.Entities.Notifications;

public class NotificationMappingProfile : Profile
{
    public NotificationMappingProfile()
    {
        CreateMap<Notification, NotificationDto>();
    }
}
