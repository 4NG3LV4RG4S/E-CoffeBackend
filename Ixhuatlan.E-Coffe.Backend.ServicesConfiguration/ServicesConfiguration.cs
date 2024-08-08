using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Ixhuatlan.E_Coffe.Backend.ServicesConfiguration;

public static class ServicesConfiguration
{
    public static IServiceCollection ConfigurationServices(this IServiceCollection serviceCollection,
        string assemblyLocation, bool isClass = false)
    {
        var services = Assembly.LoadFrom(assemblyLocation)
            .GetTypes()
            .Where(type => type.IsAbstract == false
                           && type.IsInterface == false
                           && type.IsEnum == false
                           && Regex.IsMatch(type.Name, "Nullable|Attribute|[<>]") == false
                           && type.BaseType?.Name != "DbContext");

        services
            .ToList().ForEach(type => 
                type.GetInterfaces().ToList().ForEach(interfaceType =>
                    serviceCollection.TryAddScoped(interfaceType, type)));

        if (isClass)
        {
            services.ToList().ForEach(type => serviceCollection.TryAddScoped(type));
        }

        return serviceCollection;
    }
}