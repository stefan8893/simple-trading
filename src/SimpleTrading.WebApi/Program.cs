using System.Diagnostics;
using System.Text.Json.Serialization;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Serilog;
using SimpleTrading.WebApi.CliCommands;
using SimpleTrading.WebApi.Configuration;
using SimpleTrading.WebApi.Extensions;
using SimpleTrading.WebApi.Filter;
using SimpleTrading.WebApi.Modules;
using SimpleTrading.WebApi.OpenApi;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>((ctx, b) =>
{
    b.RegisterModule<WebApiModule>();
    b.RegisterModule<DomainModule>();
    b.RegisterModule(new TradingDbContextModule(ctx.Configuration));
    b.RegisterModule<DataAccessModule>();
});

builder.Services.AddSerilog(logger => logger.ReadFrom.Configuration(builder.Configuration));
builder.Services
    .AddControllers(o =>
    {
        o.ModelValidatorProviders.Clear();
        o.Filters.Add<ValidationFilter>();
    })
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(
        o => { o.Audience = builder.Configuration.GetValue<string>("Auth:SimpleTradingWebApi:Audience"); },
        options => builder.Configuration.Bind("Auth:SimpleTradingWebApi", options));

const string clientAppCorsPolicy = "ClientAppCorsPolicy";
builder.Services.AddCors(options =>
{
    var clientAppUrls = builder.Configuration
        .GetSection("CorsUrls:ClientApp")
        .Get<string[]>() ?? [];

    options.AddPolicy(clientAppCorsPolicy, policy =>
    {
        policy.WithOrigins(clientAppUrls)
            .AllowCredentials()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

var clientAppEntraIdConfig = builder.Configuration
                                 .GetSection("Auth:SimpleTradingClientApp")
                                 .Get<ClientAppEntraIdConfig>()
                             ?? throw new Exception("Missing Entra ID settings");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocumentation();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.MapOpenApi();
app.UseScalarUi(clientAppEntraIdConfig);

app.UseHttpsRedirection();
app.UseRequestLocalization();

app.UseCors(clientAppCorsPolicy);

app.UseAuthentication();
app.Use401ResponseBodyProblemDetailsMiddleware();
app.UseAuthorization();
app.UseNotFoundMiddleware();

app.MapControllers()
    .RequireAuthorization();

var rootCommand = AppRootCommand.Create(app);
rootCommand.Subcommands.Add(CreateDatabaseCommand.Create(app));
rootCommand.Subcommands.Add(SeedDatabaseCommand.Create(app));
rootCommand.Subcommands.Add(DropDatabaseCommand.Create(app));
rootCommand.Subcommands.Add(GenerateClientCommand.Create(app));

var parsed = rootCommand.Parse(args);
await parsed.InvokeAsync();

namespace SimpleTrading.WebApi
{
    [UsedImplicitly]
    public class Program
    {
    }
}