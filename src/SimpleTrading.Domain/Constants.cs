using System.Collections.Immutable;
using System.Globalization;
using SimpleTrading.Domain.Infrastructure.Extensions;

namespace SimpleTrading.Domain;

public static class Constants
{
    public const string DefaultTimeZone = "Europe/Vienna";
    public const int OpenedDateMaxDaysInTheFutureBoundary = 1;
    public static readonly DateTime MinDate = DateTime.Parse("2000-01-01T00:00:00").ToUtcKind();

    public static readonly CultureInfo DefaultCulture = new("de-AT");

    public static readonly IImmutableSet<CultureInfo> SupportedCultures =
        ImmutableHashSet.Create(new CultureInfo("de-AT"), new CultureInfo("en-US"));

    public static readonly IImmutableSet<string> SupportedLanguages =
        ImmutableHashSet.Create(StringComparer.OrdinalIgnoreCase, "en", "de");

    public static readonly Guid UserSettingsId = Guid.Parse("401c519b-956a-4a5f-bd84-77e716817771");
}