using JetBrains.Annotations;

namespace SimpleTrading.WebApi.Infrastructure;

[UsedImplicitly]
public record UpdateValue<T>(T? Value = default);