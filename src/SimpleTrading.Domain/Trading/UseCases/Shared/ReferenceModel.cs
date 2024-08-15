namespace SimpleTrading.Domain.Trading.UseCases.Shared;

public record ReferenceModel(Guid Id, ReferenceType Type, string Link, string? Notes = null)
{
    public static ReferenceModel From(Reference reference)
    {
        return new ReferenceModel(reference.Id, reference.Type, reference.Link.AbsoluteUri, reference.Notes);
    }
}