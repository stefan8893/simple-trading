using JetBrains.Annotations;
using SimpleTrading.WebApi.Features.Trading.Dto;

namespace SimpleTrading.WebApi.Infrastructure;

[UsedImplicitly]
public record UpdateValue<T>(T? Value = default);

[UsedImplicitly]
public record UpdateResultValue : UpdateValue<ResultDto?>;

[UsedImplicitly]
public record UpdateStringValue : UpdateValue<string?>;