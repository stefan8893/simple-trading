﻿using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.WebApi.Infrastructure;

public record SuccessResponse
{
    private SuccessResponse(IEnumerable<string> warnings)
    {
        Warnings = warnings;
    }

    public IEnumerable<string> Warnings { get; init; } = [];

    public static readonly SuccessResponse Empty = From([]); 
    
    public static SuccessResponse From(IEnumerable<string> warnings)
    {
        return new SuccessResponse(warnings);
    }
}


public record SuccessResponse<T> where T : notnull
{
    private SuccessResponse(T data)
    {
        Data = data;
    }

    private SuccessResponse(T data, IEnumerable<string> warnings)
    {
        Data = data;
        Warnings = warnings;
    }

    public T Data { get; init; }
    public IEnumerable<string> Warnings { get; init; } = [];

    public static SuccessResponse<T> From(T data)
    {
        return new SuccessResponse<T>(data);
    }
    
    public static SuccessResponse<T> From(T data, IEnumerable<Warning> warnings)
    {
        return new SuccessResponse<T>(data, warnings.Select(x => x.Reason));
    }
    
    public static SuccessResponse<T> From(Completed<T> completed)
    {
        return new SuccessResponse<T>(completed.Data);
    }
    
    public static SuccessResponse<T> From(CompletedWithWarnings<T> completed)
    {
        return new SuccessResponse<T>(completed.Data, completed.Warnings.Select(x => x.Reason));
    }
}