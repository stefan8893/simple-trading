using System.Reflection;
using System.Runtime.CompilerServices;

namespace SimpleTrading.WebApi.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFactory(this IServiceCollection services, Delegate factory,
        ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        var typeToRegister = factory.Method.ReturnType;
        var arguments = factory.Method
            .GetParameters()
            .Select(p =>
            {
                var serviceLocator = GetServiceLocator();
                if (serviceLocator is null)
                    throw new Exception(
                        "Extension method 'GetRequiredService' not found.");

                return serviceLocator.MakeGenericMethod(p.ParameterType);
            });

        var serviceDescriptor =
            new ServiceDescriptor(typeToRegister,
                sp => factory.DynamicInvoke(arguments.Select(x => x.Invoke(null, [sp])).ToArray())!, lifetime);
        services.Add(serviceDescriptor);

        return services;
    }

    private static MethodInfo? GetServiceLocator()
    {
        return AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .SelectMany(x => x.GetMethods(BindingFlags.Static | BindingFlags.Public))
            .Where(x => x.IsDefined(typeof(ExtensionAttribute), false))
            .FirstOrDefault(x => x is {Name: "GetRequiredService", IsGenericMethod: true});
    }
}