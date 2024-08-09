using FluentValidation;

namespace SimpleTrading.WebApi.Features.Trading.Dto;

public enum ResultDto
{
    Win,
    Mediocre,
    BreakEven,
    Loss
}

public class FinishTradeDto
{
    public decimal? Balance { get; set; }
    public decimal? ExitPrice { get; set; }
    public DateTime? Closed { get; set; }
    public ResultDto? Result { get; set; }
}

public class FinishTradeDtoValidator : AbstractValidator<FinishTradeDto>
{
    public FinishTradeDtoValidator()
    {
        RuleFor(x => x.Balance).NotNull();
        RuleFor(x => x.ExitPrice).NotNull();
        RuleFor(x => x.Closed).NotNull();

        RuleFor(x => x.Result)
            .NotNull()
            .IsInEnum();
    }
}