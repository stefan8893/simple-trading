using System.CommandLine;
using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.WebApi;
using SimpleTrading.WebApi.Clients;
using SimpleTrading.WebApi.Configuration;
using SimpleTrading.WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(_ => { },
        options => builder.Configuration.Bind("Auth:SimpleTradingWebApi", options));

var clientAppEntraIdConfig = builder.Configuration
                                 .GetSection("Auth:SimpleTradingClientApp")
                                 .Get<ClientAppEntraIdConfig>()
                             ?? throw new Exception("Missing Entra ID settings");

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(
        o => o.InvalidModelStateResponseFactory = ctx => ctx.ModelState.ToCustomErrorResponse())
    .AddJsonOptions(options =>
    {
        var enumConverter = new JsonStringEnumConverter();
        options.JsonSerializerOptions.Converters.Add(enumConverter);
    });

builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureOpenApiDocumentation(clientAppEntraIdConfig);
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddValidatorsFromAssemblyContaining<BaseInteractor>();

builder.Services.AddTradingDbContext(builder.Configuration);
builder.Services.AddDateTimeProvider();
builder.Services.AddUseCases();
builder.Services.AddSingleton<ClientGenerator>();


var app = builder.Build();

app.ConfigureSwaggerUi(clientAppEntraIdConfig);
app.UseHttpsRedirection();
app.UseRequestLocalization();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers()
    .RequireAuthorization();

var rootCommand = CliCommands.RootCommand(app);
rootCommand.AddCommand(CliCommands.CreateDatabaseCommand(app));
rootCommand.AddCommand(CliCommands.SeedDatabaseCommand(app));
rootCommand.AddCommand(CliCommands.DropDatabaseCommand(app));
rootCommand.AddCommand(CliCommands.GenerateClientCommand(app));

await rootCommand.InvokeAsync(args);

namespace SimpleTrading.WebApi
{
    public class Program
    {
    }
}