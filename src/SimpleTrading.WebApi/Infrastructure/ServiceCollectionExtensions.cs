namespace SimpleTrading.WebApi.Infrastructure;

using System.Reflection;


// based upon: https://blog.photogrammer.net/factory-delegates-using-microsoft-di/
public static class ServiceCollectionExtensions
{
    public static void AddFactory(this IServiceCollection services, Delegate factory)
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

        closedGenericMethod.Invoke(null, [services, factory]);
    }

    private static void AddFactoryInternal<TResult>(this IServiceCollection services, Func<TResult> factory)
        where TResult : class
    {
        services.AddSingleton(c => factory());
    }

    private static void AddFactoryInternal<T1, TResult>(this IServiceCollection services, Func<T1, TResult> factory)
        where T1 : notnull
        where TResult : class
    {
        services.AddSingleton(c => factory(c.GetRequiredService<T1>()));
    }

}
