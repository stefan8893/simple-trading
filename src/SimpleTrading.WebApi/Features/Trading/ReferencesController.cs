using System.Net.Mime;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OneOf.Types;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.References.AddReference;
using SimpleTrading.Domain.Trading.UseCases.References.DeleteReference;
using SimpleTrading.Domain.Trading.UseCases.References.DeleteReferences;
using SimpleTrading.Domain.Trading.UseCases.References.GetReference;
using SimpleTrading.Domain.Trading.UseCases.References.GetReferences;
using SimpleTrading.Domain.Trading.UseCases.References.UpdateReference;
using SimpleTrading.WebApi.Extensions;
using SimpleTrading.WebApi.Features.Trading.Dto;
using SimpleTrading.WebApi.Features.Trading.Dto.Reference;
using SimpleTrading.WebApi.Infrastructure;

namespace SimpleTrading.WebApi.Features.Trading;

[ApiController]
[Route("trades/{tradeId:guid}/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[SwaggerUiControllerPosition(2)]
public class ReferencesController : ControllerBase
{
    [HttpGet("{referenceId:guid}", Name = nameof(GetReference))]
    [ProducesResponseType<ReferenceDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetReference(
        [FromServices] IGetReference getReference,
        [FromRoute] Guid tradeId,
        [FromRoute] Guid referenceId)
    {
        var result = await getReference.Execute(new GetReferenceRequestModel(tradeId, referenceId));

        return result.Match(
            referenceModel => Ok(ReferenceDto.From(referenceModel)),
            notFound => notFound.ToActionResult()
        );
    }

    [HttpGet(Name = nameof(GetReferences))]
    [ProducesResponseType<IEnumerable<ReferenceDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetReferences(
        [FromServices] IGetReferences getReferences,
        [FromRoute] Guid tradeId)
    {
        var result = await getReferences.Execute(new GetReferencesRequestModel(tradeId));

        return result.Match(
            references => Ok(references.Select(ReferenceDto.From)),
            notFound => notFound.ToActionResult()
        );
    }

    [HttpPost(Name = nameof(AddReference))]
    [ProducesResponseType<SuccessResponse<Guid>>(StatusCodes.Status200OK)]
    [ProducesResponseType<FieldErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult> AddReference(
        [FromServices] IAddReference addReference,
        [FromRoute] Guid tradeId,
        [FromBody] AddReferenceDto addReferenceDto)
    {
        var addReferenceRequestModel =
            new AddReferenceRequestModel(tradeId, addReferenceDto.Type.ToDomainReferenceType(), addReferenceDto.Link!, addReferenceDto.Notes);

        var result = await addReference.Execute(addReferenceRequestModel);

        return result.Match(
            completed => Ok(SuccessResponse<Guid>.From(completed.Data, completed.Warnings)),
            badInput => badInput.ToActionResult(),
            notFound => notFound.ToActionResult(),
            businessError => businessError.ToActionResult()
        );
    }

    [HttpPatch("{referenceId:guid}", Name = nameof(UpdateReference))]
    [ProducesResponseType<SuccessResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<FieldErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateReference(
        [FromServices] IUpdateReference updateReference,
        [FromRoute] Guid tradeId,
        [FromRoute] Guid referenceId,
        [FromBody] UpdateReferenceDto dto)
    {
        var addReferenceRequestModel = MapToRequestModel(tradeId, referenceId, dto);
        var result = await updateReference.Execute(addReferenceRequestModel);

        return result.Match(
            completed => Ok(SuccessResponse.From(completed.Warnings)),
            badInput => badInput.ToActionResult(),
            notFound => notFound.ToActionResult()
        );
    }

    [HttpDelete("{referenceId:guid}", Name = nameof(DeleteReference))]
    [ProducesResponseType<SuccessResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteReference(
        [FromServices] IDeleteReference deleteReference,
        [FromRoute] Guid tradeId,
        [FromRoute] Guid referenceId)
    {
        var result = await deleteReference.Execute(new DeleteReferenceRequestModel(tradeId, referenceId));

        return result.Match(
            completed => Ok(SuccessResponse.From(completed.Warnings)),
            notFound => notFound.ToActionResult()
        );
    }

    [HttpDelete(Name = nameof(DeleteReferences))]
    [ProducesResponseType<SuccessResponse<ushort>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteReferences(
        [FromServices] IDeleteReferences deleteReferences,
        [FromRoute] Guid tradeId)
    {
        var result = await deleteReferences.Execute(new DeleteReferencesRequestModel(tradeId));

        return result.Match(
            completed => Ok(SuccessResponse<ushort>.From(completed)),
            notFound => notFound.ToActionResult()
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