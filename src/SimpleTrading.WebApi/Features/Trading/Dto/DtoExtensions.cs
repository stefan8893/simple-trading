using SimpleTrading.Domain.Trading.UseCases;

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
}