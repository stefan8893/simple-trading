using System.Text.Json.Serialization;

namespace SimpleTrading.WebApi.Features.Trading.Dto;

[JsonConverter(typeof(JsonStringEnumConverter<ResultDto>))]
public enum ResultDto
{
    Win,
    Mediocre,
    BreakEven,
    Loss
}