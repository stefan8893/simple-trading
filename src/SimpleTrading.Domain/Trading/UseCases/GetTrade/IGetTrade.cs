﻿using OneOf;
using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.Domain.Trading.UseCases.GetTrade;

public interface IGetTrade : IInteractor<Guid, OneOf<TradeResponseModel, NotFound>>
{
    
}