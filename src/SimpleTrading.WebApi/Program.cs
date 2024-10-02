using System.CommandLine;
using System.Text.Json.Serialization;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Serilog;
using SimpleTrading.DataAccess;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.WebApi.CliCommands;
using SimpleTrading.WebApi.Configuration;
using SimpleTrading.WebApi.Extensions;
using SimpleTrading.WebApi.Filter;
using SimpleTrading.WebApi.Modules;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>((ctx, b) =>
{
    b.RegisterModule<WebApiModule>();
    b.RegisterModule(new TradingDbContextModule(ctx.Configuration));
    b.RegisterModule<DomainModule>();
    b.RegisterModule<DataAccessModule>();
});

builder.Services.AddSerilog(lc => lc.ReadFrom.Configuration(builder.Configuration));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(_ => { },
        options => builder.Configuration.Bind("Auth:SimpleTradingWebApi", options));

var clientAppEntraIdConfig = builder.Configuration
                                 .GetSection("Auth:SimpleTradingClientApp")
                                 .Get<ClientAppEntraIdConfig>()
                             ?? throw new Exception("Missing Entra ID settings");

builder.Services
    .AddControllers(o =>
    {
        o.ModelValidatorProviders.Clear();
        o.Filters.Add<ValidationFilter>();
    })
    .ConfigureApiBehaviorOptions(o => o.SuppressMapClientErrors = true)
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureOpenApiDocumentation(clientAppEntraIdConfig);
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddValidatorsFromAssemblyContaining<InteractorBase>();

var app = builder.Build();

app.ConfigureSwaggerUi(clientAppEntraIdConfig);
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseRequestLocalization();
app.UseNotFoundMiddleware();

app.MapControllers()
    .RequireAuthorization();

var rootCommand = AppRootCommand.Create(app);
rootCommand.AddCommand(CreateDatabaseCommand.Create(app));
rootCommand.AddCommand(SeedDatabaseCommand.Create(app));
rootCommand.AddCommand(DropDatabaseCommand.Create(app));
rootCommand.AddCommand(GenerateClientCommand.Create(app));

await rootCommand.InvokeAsync(args);

namespace SimpleTrading.WebApi
{
    public class Program
    {
    }
}