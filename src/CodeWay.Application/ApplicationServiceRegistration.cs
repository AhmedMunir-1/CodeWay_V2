namespace CodeWay.Application;

using System.Reflection;
using CodeWay.Application.Common;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // Register AutoMapper profiles in Application assembly
        services.AddAutoMapper(cfg => cfg.AddMaps(assembly));

        // Register MediatR handlers
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        // Register all FluentValidation validators
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}
