using SimpleTrading.WebApi.Features.Dto;

namespace SimpleTrading.WebApi.Features.Trading.Dto.Reference;

public record UpdateReferenceDto
{
    public string? Link { get; set; }
    public UpdateStringValue? Notes { get; set; }
}