using JetBrains.Annotations;

namespace SimpleTrading.WebApi.Features.UserSettings.Dto;

[UsedImplicitly]
public class UpdateUserLanguageDto
{
    public string? IsoLanguageCode { get; set; }
}
