using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.Shared;

namespace SimpleTrading.WebApi.Features.Trading.Dto.Reference;

public record ReferenceDto
{
    public required Guid Id { get; init; }
    public required ReferenceTypeDto Type { get; init; }
    public required string Link { get; init; }
    public string? Notes { get; set; }

    public static ReferenceDto From(ReferenceModel referenceModel)
    {
        return new ReferenceDto
        {
            Id = referenceModel.Id,
            Type = MapToReferenceDto(referenceModel.Type),
            Link = referenceModel.Link,
            Notes = referenceModel.Notes
        };

        ReferenceTypeDto MapToReferenceDto(ReferenceType type)
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