namespace SimpleTrading.WebApi.Features.Trading.Dto;

public interface IPagination
{
    int? Page { get; set; }
    int? PageSize { get; set; }
}