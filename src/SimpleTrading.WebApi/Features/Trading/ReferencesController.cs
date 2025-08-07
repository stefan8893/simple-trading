using Microsoft.AspNetCore.Mvc;
using OneOf.Types;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.References.AddReference;
using SimpleTrading.Domain.Trading.UseCases.References.DeleteReference;
using SimpleTrading.Domain.Trading.UseCases.References.DeleteReferences;
using SimpleTrading.Domain.Trading.UseCases.References.GetReference;
using SimpleTrading.Domain.Trading.UseCases.References.GetReferences;
using SimpleTrading.Domain.Trading.UseCases.References.UpdateReference;
using SimpleTrading.WebApi.Features.Trading.Dto;
using SimpleTrading.WebApi.Features.Trading.Dto.Reference;
using SimpleTrading.WebApi.Infrastructure;

namespace SimpleTrading.WebApi.Features.Trading;

[Route("trades/{tradeId:guid}/[controller]")]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ReferencesController : SimpleControllerBase
{
    [HttpGet("{referenceId:guid}", Name = nameof(GetReference))]
    [ProducesResponseType<ReferenceDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetReference(
        [FromServices] IGetReference getReference,
        [FromRoute] Guid tradeId,
        [FromRoute] Guid referenceId)
    {
        var result = await getReference.Execute(new GetReferenceRequestModel(tradeId, referenceId));

        return result.Match(
            referenceModel => Ok(ReferenceDto.From(referenceModel)),
            NotFoundResult
        );
    }

    [HttpGet(Name = nameof(GetReferences))]
    [ProducesResponseType<IEnumerable<ReferenceDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetReferences(
        [FromServices] IGetReferences getReferences,
        [FromRoute] Guid tradeId)
    {
        var result = await getReferences.Execute(new GetReferencesRequestModel(tradeId));

        return result.Match(
            references => Ok(references.Select(ReferenceDto.From)),
            NotFoundResult
        );
    }

    [HttpPost(Name = nameof(AddReference))]
    [ProducesResponseType<Guid>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> AddReference(
        [FromServices] IAddReference addReference,
        [FromRoute] Guid tradeId,
        [FromBody] AddReferenceDto addReferenceDto)
    {
        var addReferenceRequestModel =
            new AddReferenceRequestModel(tradeId, addReferenceDto.Type.ToDomainReferenceType(), addReferenceDto.Link!,
                addReferenceDto.Notes);

        var result = await addReference.Execute(addReferenceRequestModel);

        return result.Match(
            completed => Ok(completed.Data),
            NotFoundResult,
            ConflictResult,
            UnprocessableEntityResult
        );
    }

    [HttpPatch("{referenceId:guid}", Name = nameof(UpdateReference))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UpdateReference(
        [FromServices] IUpdateReference updateReference,
        [FromRoute] Guid tradeId,
        [FromRoute] Guid referenceId,
        [FromBody] UpdateReferenceDto dto)
    {
        var addReferenceRequestModel = MapToRequestModel(tradeId, referenceId, dto);
        var result = await updateReference.Execute(addReferenceRequestModel);

        return result.Match(
            completed => NoContent(),
            NotFoundResult,
            UnprocessableEntityResult
        );
    }

    [HttpDelete("{referenceId:guid}", Name = nameof(DeleteReference))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteReference(
        [FromServices] IDeleteReference deleteReference,
        [FromRoute] Guid tradeId,
        [FromRoute] Guid referenceId)
    {
        var result = await deleteReference.Execute(new DeleteReferenceRequestModel(tradeId, referenceId));

        return result.Match(
            completed => NoContent(),
            NotFoundResult
        );
    }

    [HttpDelete(Name = nameof(DeleteReferences))]
    [ProducesResponseType<ushort>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteReferences(
        [FromServices] IDeleteReferences deleteReferences,
        [FromRoute] Guid tradeId)
    {
        var result = await deleteReferences.Execute(new DeleteReferencesRequestModel(tradeId));

        return result.Match(
            completed => Ok(completed.Data),
            NotFoundResult
        );
    }

    private static UpdateReferenceRequestModel MapToRequestModel(Guid tradeId, Guid referenceId, UpdateReferenceDto dto)
    {
        return new UpdateReferenceRequestModel
        {
            TradeId = tradeId,
            ReferenceId = referenceId,
            Type = MapToReferenceType(dto.Type),
            Link = dto.Link,
            Notes = dto.Notes is null ? new None() : dto.Notes.Value
        };
    }

    private static ReferenceType? MapToReferenceType(ReferenceTypeDto? dto)
    {
        return dto switch
        {
            ReferenceTypeDto.Other => ReferenceType.Other,
            ReferenceTypeDto.TradingView => ReferenceType.TradingView,
            _ => null
        };
    }
}