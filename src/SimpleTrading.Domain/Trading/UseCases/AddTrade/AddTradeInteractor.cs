using OneOf;
using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.Domain.Trading.UseCases.AddTrade;

using AddTradeResponse = OneOf<Completed<Guid>, BadInput, NotFound, BusinessError>;

public class AddTradeInteractor : BaseInteractor, IAddTrade
{
    public Task<AddTradeResponse> Execute(AddTradeRequestModel model)
    {
        throw new NotImplementedException();
    }
}