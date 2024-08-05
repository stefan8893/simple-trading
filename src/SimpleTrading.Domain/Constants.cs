using System.Globalization;
using SimpleTrading.Domain.Extensions;

namespace SimpleTrading.Domain;

public static class Constants
{
    public static readonly DateTime MinDate = DateTime.Parse("2000-01-01T00:00:00").ToUtcKind();

    public static readonly CultureInfo DefaultCulture = new("de-AT");

    public static readonly IReadOnlyCollection<CultureInfo> SupportedCultures =
    [
        new CultureInfo("de-AT"),
        new CultureInfo("en-US")
    ];

    public static readonly IReadOnlyList<string> SupportedLanguages =
    [
        "de",
        "en"
    ];

    public static readonly string DefaultTimeZone = "Europe/Vienna";

    public static readonly Guid DefaultProfileId = Guid.Parse("adf868b2-d0ba-4805-8f05-c813310580af");
    public static readonly Guid UserSettingsId = Guid.Parse("401c519b-956a-4a5f-bd84-77e716817771");
}