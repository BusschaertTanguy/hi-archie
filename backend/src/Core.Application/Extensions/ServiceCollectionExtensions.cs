using System.Reflection;
using Common.Application.Extensions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        return services
            .AddValidatorsFromAssembly(assembly, includeInternalTypes: true)
            .AddCommandHandlersFromAssembly(assembly)
            .AddQueryHandlersFromAssembly(assembly);
    }
}