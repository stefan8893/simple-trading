using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SimpleTrading.Domain.Extensions;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.User;

namespace SimpleTrading.DataAccess;

public class TradingDbContext(DbContextOptions<TradingDbContext> options, UtcNow utcNow) : DbContext(options)
{
    public DbSet<Asset> Assets { get; set; }
    public DbSet<Currency> Currencies { get; set; }
    public DbSet<Profile> Profiles { get; set; }
    public DbSet<Reference> References { get; set; }
    public DbSet<Trade> Trades { get; set; }
    public DbSet<UserSettings> UserSettings { get; set; }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Conventions.Remove(typeof(TableNameFromDbSetConvention));
        configurationBuilder.Properties<Enum>().HaveConversion<string>();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        AddDateTimeKindUtcConverter(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        RefreshUpdatedDateOnUserSettingsChanges();

        return base.SaveChangesAsync(cancellationToken);
    }

    private void RefreshUpdatedDateOnUserSettingsChanges()
    {
        var userSettings = ChangeTracker.Entries()
            .Where(x => x is {Entity: Domain.User.UserSettings, State: EntityState.Modified})
            .Select(x => (UserSettings) x.Entity);

        var nowInUtcTime = utcNow();
        foreach (var userSetting in userSettings)
            userSetting.LastModified = nowInUtcTime;
    }

    private static void AddDateTimeKindUtcConverter(ModelBuilder modelBuilder)
    {
        var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
            v => v.ToUtcKind(),
            v => v.ToUtcKind());

        var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
            v => v.HasValue ? v.Value.ToUtcKind() : v,
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