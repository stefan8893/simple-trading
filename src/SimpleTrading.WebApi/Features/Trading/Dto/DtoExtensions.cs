using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.Shared;
using SimpleTrading.WebApi.Features.Trading.Dto.Reference;

namespace SimpleTrading.WebApi.Features.Trading.Dto;

public static class ResultModelExtensions
{
    public static ResultDto? ToResultDto(this ResultModel? resultModel)
    {
        return resultModel switch
        {
            ResultModel.Loss => ResultDto.Loss,
            ResultModel.BreakEven => ResultDto.BreakEven,
            ResultModel.Mediocre => ResultDto.Mediocre,
            ResultModel.Win => ResultDto.Win,
            _ => null
        };
    }

    public static ReferenceType ToDomainReferenceType(this ReferenceTypeDto? typeDto)
    {
        var tradeResult = typeDto switch
        {
            ReferenceTypeDto.Other => ReferenceType.Other,
            ReferenceTypeDto.TradingView => ReferenceType.TradingView,
            _ => throw new ArgumentOutOfRangeException(nameof(typeDto), typeDto, null)
        };

        return tradeResult;
    }
}