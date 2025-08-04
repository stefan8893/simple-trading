using JetBrains.Annotations;
using SimpleTrading.WebApi.Infrastructure;

namespace SimpleTrading.WebApi.Features.UserSettings.Dto;

[UsedImplicitly]
public class UpdateUserSettingsDto
{
    public string? Culture { get; set; }
    public UpdateStringValue? IsoLanguageCode { get; set; }
    public string? TimeZone { get; set; }    
    
}
