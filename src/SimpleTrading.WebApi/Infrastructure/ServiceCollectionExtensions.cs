using System.Reflection;

namespace SimpleTrading.WebApi.Infrastructure;

// based upon: https://blog.photogrammer.net/factory-delegates-using-microsoft-di/
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFactory(this IServiceCollection services, Delegate factory,
        ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        var returnType = factory.Method.ReturnType;
        var parameters = factory.Method
            .GetParameters()
            .Select(x => x.ParameterType)
            .ToArray();

        var addFactoryGenericParametersLength = parameters.Length + 1;

        var openGenericMethod = typeof(ServiceCollectionExtensions)
            .GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
            .Where(x => x.Name == nameof(AddFactoryInternal))
            .SingleOrDefault(x => x.GetGenericArguments().Length == addFactoryGenericParametersLength);

        ArgumentNullException.ThrowIfNull(openGenericMethod);
        var closedGenericMethod = openGenericMethod.MakeGenericMethod([.. parameters, returnType]);

        closedGenericMethod.Invoke(null, [services, factory, lifetime]);
        
        return services;
    }

    private static void AddFactoryInternal<TResult>(this IServiceCollection services, Func<TResult> factory,
        ServiceLifetime lifetime)
        where TResult : class
    {
        var serviceDescriptor =
            new ServiceDescriptor(typeof(TResult), c => factory(), lifetime);
        services.Add(serviceDescriptor);
    }

    private static void AddFactoryInternal<T1, TResult>(this IServiceCollection services, Func<T1, TResult> factory,
        ServiceLifetime lifetime)
        where T1 : notnull
        where TResult : class
    {
        var serviceDescriptor =
            new ServiceDescriptor(typeof(TResult), c => factory(c.GetRequiredService<T1>()), lifetime);
        services.Add(serviceDescriptor);
    }
}