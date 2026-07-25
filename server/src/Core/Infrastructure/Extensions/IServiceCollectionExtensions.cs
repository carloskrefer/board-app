using Microsoft.Extensions.DependencyInjection;

namespace Core.Infrastructure.Extensions;

public static class IServiceCollectionExtensions
{
    public static void RemoveServiceByType(this IServiceCollection services, Type serviceType)
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == serviceType);

        if (descriptor is not null)
            services.Remove(descriptor);
    }
}