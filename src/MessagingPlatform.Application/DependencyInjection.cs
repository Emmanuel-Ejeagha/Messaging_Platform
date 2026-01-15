using System;
using FluentValidation;
using MediatR;
using MessagingPlatform.Application.Common.Behaviors;
using MessagingPlatform.Application.Common.Interfaces;
using MessagingPlatform.Application.Common.Mappings;
using Microsoft.Extensions.DependencyInjection;

namespace MessagingPlatform.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // AutoMapper
        services.AddAutoMapper(typeof(MappingProfile).Assembly);

        // MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        });

        // FluentValidation
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        // Current User Service 
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // Domain Event Service
        services.AddScoped<IDomainEventService, DomainEventService>();

        return services;
    }
}
