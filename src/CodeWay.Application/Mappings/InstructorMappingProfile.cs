namespace CodeWay.Application.Mappings;

using AutoMapper;
using CodeWay.Application.Features.Instructor.DTOs;
using CodeWay.Domain.Entities.Instructor;

public class InstructorMappingProfile : Profile
{
    public InstructorMappingProfile()
    {
        CreateMap<InstructorProfile, InstructorProfileDto>()
            .ForMember(d => d.UserName, opt => opt.MapFrom(s => s.User != null ? $"{s.User.FirstName} {s.User.LastName}" : string.Empty))
            .ForMember(d => d.UserEmail, opt => opt.MapFrom(s => s.User != null ? s.User.Email : string.Empty));

        CreateMap<InstructorWallet, InstructorWalletDto>();
        CreateMap<WalletTransaction, WalletTransactionDto>();
        CreateMap<PayoutRequest, PayoutRequestDto>()
            .ForMember(d => d.InstructorName, opt => opt.MapFrom(s => s.Instructor != null && s.Instructor.User != null
                ? $"{s.Instructor.User.FirstName} {s.Instructor.User.LastName}" : string.Empty));
    }
}
