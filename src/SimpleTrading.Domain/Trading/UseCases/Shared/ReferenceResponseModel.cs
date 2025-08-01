namespace SimpleTrading.Domain.Trading.UseCases.Shared;

public record ReferenceResponseModel(Guid Id, ReferenceType Type, string Link, string? Notes = null)
{
    public static ReferenceResponseModel From(Reference reference)
    {
        return new ReferenceResponseModel(reference.Id, reference.Type, reference.Link.AbsoluteUri, reference.Notes);
    }
}