namespace CodeWay.Application.Mappings;

using AutoMapper;
using CodeWay.Application.Features.Payments.DTOs;
using CodeWay.Domain.Entities.Payments;

public class PaymentsMappingProfile : Profile
{
    public PaymentsMappingProfile()
    {
        CreateMap<Payment, PaymentDto>()
            .ForMember(d => d.UserName, opt => opt.MapFrom(s => s.User != null ? $"{s.User.FirstName} {s.User.LastName}" : string.Empty));
    }
}
