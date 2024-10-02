namespace SimpleTrading.WebApi.Features.Trading.Dto;

public record PageDto<T>(IEnumerable<T> Data, int Count, int TotalCount, int TotalPages, int Page, int PageSize);