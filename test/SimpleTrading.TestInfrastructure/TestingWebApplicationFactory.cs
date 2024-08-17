using System.Data.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SimpleTrading.DataAccess;

namespace SimpleTrading.TestInfrastructure;

public class TestingWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    public Action<WebHostBuilderContext, IServiceCollection>? OverrideServices { get; set; }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<TradingDbContext>));

            services.Remove(dbContextDescriptor!);

            var dbConnectionDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbConnection));

            services.Remove(dbConnectionDescriptor!);

            services.AddSingleton<DbConnection>(_ =>
            {
                var connection = new SqliteConnection($"DataSource=file:{Guid.NewGuid()}?mode=memory");
                connection.Open();

                return connection;
            });

            services.AddDbContext<TradingDbContext>((sp, options) =>
            {
                var connection = sp.GetRequiredService<DbConnection>();
                options.UseSqlite(connection,
                    x => x.MigrationsAssembly("SimpleTrading.DataAccess.Sqlite"));
                options.EnableSensitiveDataLogging();
            });
        });

        builder.UseEnvironment("Development");

        builder.ConfigureLogging(lb => { lb.SetMinimumLevel(LogLevel.Error); });

        if (OverrideServices is not null)
            builder.ConfigureServices(OverrideServices);
    }
}