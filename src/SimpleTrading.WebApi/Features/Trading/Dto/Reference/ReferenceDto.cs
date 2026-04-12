using SimpleTrading.Domain.Trading.UseCases.Shared;

namespace SimpleTrading.WebApi.Features.Trading.Dto.Reference;

public record ReferenceDto
{
    public required Guid Id { get; init; }
    public required string Link { get; init; }
    public string? Notes { get; set; }

    public static ReferenceDto From(ReferenceResponseModel referenceResponseModel)
    {
        return new ReferenceDto
        {
            Id = referenceResponseModel.Id,
            Link = referenceResponseModel.Link,
            Notes = referenceResponseModel.Notes
        };
    }
}