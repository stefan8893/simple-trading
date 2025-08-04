using System.Text.Json.Serialization;

namespace SimpleTrading.WebApi.Features.Trading.Dto.Reference;

[JsonConverter(typeof(JsonStringEnumConverter<ReferenceTypeDto>))]
public enum ReferenceTypeDto
{
    TradingView,
    Other
}