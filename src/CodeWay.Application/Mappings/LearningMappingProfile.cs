namespace CodeWay.Application.Mappings;

using AutoMapper;
using CodeWay.Application.Features.Learning.DTOs;
using CodeWay.Domain.Entities.Learning;

public class LearningMappingProfile : Profile
{
    public LearningMappingProfile()
    {
        CreateMap<Enrollment, EnrollmentDto>()
            .ForMember(d => d.UserName, opt => opt.MapFrom(s => s.User != null ? $"{s.User.FirstName} {s.User.LastName}" : string.Empty))
            .ForMember(d => d.CourseTitle, opt => opt.MapFrom(s => s.Course != null ? s.Course.Title : string.Empty))
            .ForMember(d => d.CourseSlug, opt => opt.MapFrom(s => s.Course != null ? s.Course.Slug : string.Empty))
            .ForMember(d => d.CourseThumbnailUrl, opt => opt.MapFrom(s => s.Course != null ? s.Course.ThumbnailUrl : null));

        CreateMap<LessonProgress, LessonProgressDto>()
            .ForMember(d => d.LessonTitle, opt => opt.MapFrom(s => s.Lesson != null ? s.Lesson.Title : string.Empty));

        CreateMap<Certificate, CertificateDto>();

        CreateMap<Review, ReviewDto>()
            .ForMember(d => d.UserName, opt => opt.MapFrom(s => s.User != null ? $"{s.User.FirstName} {s.User.LastName}" : string.Empty))
            .ForMember(d => d.UserProfilePictureUrl, opt => opt.MapFrom(s => s.User != null ? s.User.ProfilePictureUrl : null));
    }
}
