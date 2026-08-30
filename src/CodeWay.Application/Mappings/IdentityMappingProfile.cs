namespace CodeWay.Application.Mappings;

using AutoMapper;
using CodeWay.Application.Features.Identity.DTOs;
using CodeWay.Domain.Entities.Identity;

public class IdentityMappingProfile : Profile
{
    public IdentityMappingProfile()
    {
        CreateMap<User, UserProfileDto>();
        CreateMap<Role, RoleDto>();
    }
}
