using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OneOf;
using SimpleTrading.Domain.DataAccess;
using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.Domain.Trading.UseCases.Profiles;

public class GetProfilesInteractor(IValidator<GetProfilesRequestModel> validator, TradingDbContext dbContext)
    : BaseInteractor, IGetProfiles
{
    public async Task<OneOf<IReadOnlyList<GetProfilesResponseModel>, BadInput>> Execute(GetProfilesRequestModel model)
    {
        var validation = await validator.ValidateAsync(model);
        if (!validation.IsValid)
            return BadInput(validation);

        var useSearchTerm = !string.IsNullOrWhiteSpace(model.SearchTerm);
        var searchTerm = model.SearchTerm?.Trim().ToLower();

        var result = await dbContext.Profiles
            .Where(x => !useSearchTerm || EF.Functions.Like(x.Name.ToLower(), $"%{searchTerm}%"))
            .ToListAsync();

        return result
            .Select(x => new GetProfilesResponseModel(x.Id, x.Name, x.Description, x.IsSelected))
            .ToList();
    }
}