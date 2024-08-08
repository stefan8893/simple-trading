using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.GetTrade;

namespace SimpleTrading.WebApi.Features.Trading.Dto.Reference;

using ReferenceModel = TradeResponseModel.ReferenceModel;

public record ReferenceDto
{
    public Guid Id { get; init; }
    public ReferenceTypeDto Type { get; init; }
    public required string Link { get; init; }
    public string? Notes { get; set; }

    public static ReferenceDto From(ReferenceModel referenceModel)
    {
        return new ReferenceDto
        {
            Id = referenceModel.Id,
            Type = MapToResultDto(referenceModel.Type),
            Link = referenceModel.Link,
            Notes = referenceModel.Notes
        };

        ReferenceTypeDto MapToResultDto(ReferenceType type)
        {
            return type switch
            {
                ReferenceType.TradingView => ReferenceTypeDto.TradingView,
                ReferenceType.Other => ReferenceTypeDto.Other,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }
}