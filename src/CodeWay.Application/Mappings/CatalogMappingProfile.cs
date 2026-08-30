namespace CodeWay.Application.Mappings;

using AutoMapper;
using CodeWay.Application.Features.Catalog.DTOs;
using CodeWay.Domain.Entities.Catalog;

public class CatalogMappingProfile : Profile
{
    public CatalogMappingProfile()
    {
        CreateMap<Category, CategoryDto>()
            .ForMember(d => d.ParentCategoryName, opt => opt.MapFrom(s => s.ParentCategory != null ? s.ParentCategory.Name : null))
            .ForMember(d => d.CoursesCount, opt => opt.MapFrom(s => s.Courses.Count));

        CreateMap<Course, CourseDto>()
            .ForMember(d => d.InstructorName, opt => opt.MapFrom(s => s.Instructor != null && s.Instructor.User != null
                ? $"{s.Instructor.User.FirstName} {s.Instructor.User.LastName}" : string.Empty))
            .ForMember(d => d.CategoryName, opt => opt.MapFrom(s => s.Category != null ? s.Category.Name : string.Empty));

        CreateMap<Course, CourseDetailDto>()
            .IncludeBase<Course, CourseDto>();

        CreateMap<Section, SectionDto>();
        CreateMap<Lesson, LessonDto>();
        CreateMap<LessonAttachment, LessonAttachmentDto>();
        CreateMap<CourseRequirement, CourseRequirementDto>();
        CreateMap<CourseLearningOutcome, CourseLearningOutcomeDto>();
    }
}
