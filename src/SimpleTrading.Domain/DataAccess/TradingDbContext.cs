using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SimpleTrading.Domain.Extensions;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.User;

namespace SimpleTrading.Domain.DataAccess;

public class TradingDbContext(DbContextOptions<TradingDbContext> options) : DbContext(options)
{
    public required DbSet<Asset> Assets { get; set; }
    public required DbSet<Currency> Currencies { get; set; }
    public required DbSet<Profile> Profiles { get; set; }
    public required DbSet<Reference> References { get; set; }
    public required DbSet<Trade> Trades { get; set; }
    public required DbSet<UserSettings> UserSettings { get; set; }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Conventions.Remove(typeof(TableNameFromDbSetConvention));
        configurationBuilder.Properties<Enum>().HaveConversion<string>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        AddDateTimeKindUtcConverter(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    private static void AddDateTimeKindUtcConverter(ModelBuilder modelBuilder)
    {
        var dateTimeConverter = new ValueConverter<DateTime, DateTime>(v => v.ToUniversalTime(),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
            v => v.HasValue ? v.Value.ToUniversalTime() : v,
            v => v.HasValue ? v.Value.ToUtcKind() : v);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (entityType.IsKeyless) continue;

            foreach (var property in entityType.GetProperties())
                if (property.ClrType == typeof(DateTime))
                    property.SetValueConverter(dateTimeConverter);
                else if (property.ClrType == typeof(DateTime?))
                    property.SetValueConverter(nullableDateTimeConverter);
        }
    }
}