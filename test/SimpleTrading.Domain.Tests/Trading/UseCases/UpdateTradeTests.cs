using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SimpleTrading.Domain.Extensions;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.Shared;
using SimpleTrading.Domain.Trading.UseCases.UpdateTrade;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;
using SimpleTrading.WebApi;

namespace SimpleTrading.Domain.Tests.Trading.UseCases;

public class UpdateTradeTests(TestingWebApplicationFactory<Program> factory) : WebApiTests(factory)
{
    private IUpdateTrade CreateInteractor()
    {
        return ServiceLocator.GetRequiredService<IUpdateTrade>();
    }

    [Fact]
    public async Task A_trades_size_must_be_greater_than_zero_if_specified()
    {
        // arrange
        var trade = TestData.Trade.Default.Build();

        var updateTradeRequestModel = new UpdateTradeRequestModel
        {
            TradeId = trade.Id,
            Size = -5000
        };

        // act
        var response = await CreateInteractor().Execute(updateTradeRequestModel);

        // assert
        var badInput = response.Value.Should().BeOfType<BadInput>();
        badInput.Which.ValidationResult.Errors.Should().HaveCount(1)
            .And.Contain(x => x.ErrorMessage == "'Trade size' must be greater than '0'.");
    }

    [Fact]
    public async Task A_trades_result_must_be_in_enum_range()
    {
        // arrange
        var trade = TestData.Trade.Default.Build();

        var updateTradeRequestModel = new UpdateTradeRequestModel
        {
            TradeId = trade.Id,
            Result = (ResultModel?) 50
        };

        // act
        var response = await CreateInteractor().Execute(updateTradeRequestModel);

        // assert
        var badInput = response.Value.Should().BeOfType<BadInput>();
        badInput.Which.ValidationResult.Errors.Should().HaveCount(1)
            .And.Contain(x => x.ErrorMessage == "'Result' has a range of values which does not include '50'." &&
                              x.GetPropertyName() == "Result");
    }

    [Fact]
    public async Task The_entry_price_must_be_greater_than_zero_if_specified()
    {
        // arrange
        var trade = TestData.Trade.Default.Build();

        var updateTradeRequestModel = new UpdateTradeRequestModel
        {
            TradeId = trade.Id,
            EntryPrice = -1.2m
        };

        // act
        var response = await CreateInteractor().Execute(updateTradeRequestModel);

        // assert
        var badInput = response.Value.Should().BeOfType<BadInput>();
        badInput.Which.ValidationResult.Errors.Should().HaveCount(1)
            .And.Contain(x => x.ErrorMessage == "'Entry price' must be greater than '0'." &&
                              x.GetPropertyName() == "EntryPrice");
    }

    [Fact]
    public async Task The_SL_must_be_greater_than_zero_if_specified()
    {
        // arrange
        var trade = TestData.Trade.Default.Build();

        var updateTradeRequestModel = new UpdateTradeRequestModel
        {
            TradeId = trade.Id,
            StopLoss = -1.2m
        };

        // act
        var response = await CreateInteractor().Execute(updateTradeRequestModel);

        // assert
        var badInput = response.Value.Should().BeOfType<BadInput>();
        badInput.Which.ValidationResult.Errors.Should().HaveCount(1)
            .And.Contain(x => x.ErrorMessage == "'Stop loss' must be greater than '0'." &&
                              x.GetPropertyName() == "StopLoss");
    }

    [Fact]
    public async Task The_TP_must_be_greater_than_zero_if_specified()
    {
        // arrange
        var trade = TestData.Trade.Default.Build();

        var updateTradeRequestModel = new UpdateTradeRequestModel
        {
            TradeId = trade.Id,
            TakeProfit = -1.2m
        };

        // act
        var response = await CreateInteractor().Execute(updateTradeRequestModel);

        // assert
        var badInput = response.Value.Should().BeOfType<BadInput>();
        badInput.Which.ValidationResult.Errors.Should().HaveCount(1)
            .And.Contain(x => x.ErrorMessage == "'Take profit' must be greater than '0'." &&
                              x.GetPropertyName() == "TakeProfit");
    }

    [Fact]
    public async Task The_Exit_price_must_be_greater_than_zero_if_specified()
    {
        // arrange
        var trade = TestData.Trade.Default.Build();

        var updateTradeRequestModel = new UpdateTradeRequestModel
        {
            TradeId = trade.Id,
            ExitPrice = -1.2m
        };

        // act
        var response = await CreateInteractor().Execute(updateTradeRequestModel);

        // assert
        var badInput = response.Value.Should().BeOfType<BadInput>();
        badInput.Which.ValidationResult.Errors.Should().HaveCount(1)
            .And.Contain(x => x.ErrorMessage == "'Exit price' must be greater than '0'." &&
                              x.GetPropertyName() == "ExitPrice");
    }

    [Fact]
    public async Task A_non_existing_trade_returns_not_found()
    {
        // arrange
        var tradeId = Guid.Parse("3069c13d-2b6f-4aef-b9a1-48cfa15be160");

        var updateTradeRequestModel = new UpdateTradeRequestModel
        {
            TradeId = tradeId
        };

        // act
        var response = await CreateInteractor().Execute(updateTradeRequestModel);

        // assert
        var notFound = response.Value.Should().BeOfType<NotFound<Trade>>();
        notFound.Which.ResourceId.Should().Be(tradeId);
    }

    [Fact]
    public async Task A_trades_asset_can_be_successfully_changed()
    {
        // arrange
        var trade = TestData.Trade.Default.Build();
        var newAsset = TestData.Asset.Default.Build();

        DbContext.AddRange(trade, newAsset);
        await DbContext.SaveChangesAsync();

        var updateTradeRequestModel = new UpdateTradeRequestModel
        {
            TradeId = trade.Id,
            AssetId = newAsset.Id
        };

        // act
        var response = await CreateInteractor().Execute(updateTradeRequestModel);

        // assert
        response.Value.Should().BeOfType<Completed>();

        var updatedTrade = await DbContextSingleOrDefault<Trade>(x => x.Id == trade.Id);
        updatedTrade.Should().NotBeNull();
        updatedTrade!.AssetId.Should().Be(newAsset.Id);
    }

    [Fact]
    public async Task A_trades_profile_can_be_successfully_changed()
    {
        // arrange
        var trade = TestData.Trade.Default.Build();
        var newProfile = TestData.Profile.Default.Build();

        DbContext.AddRange(trade, newProfile);
        await DbContext.SaveChangesAsync();

        var updateTradeRequestModel = new UpdateTradeRequestModel
        {
            TradeId = trade.Id,
            ProfileId = newProfile.Id
        };

        // act
        var response = await CreateInteractor().Execute(updateTradeRequestModel);

        // assert
        response.Value.Should().BeOfType<Completed>();

        var updatedTrade = await DbContextSingleOrDefault<Trade>(x => x.Id == trade.Id);
        updatedTrade.Should().NotBeNull();
        updatedTrade!.ProfileId.Should().Be(newProfile.Id);
    }

    [Fact]
    public async Task A_trades_currency_can_be_successfully_changed()
    {
        // arrange
        var trade = TestData.Trade.Default.Build();
        var newCurrency = TestData.Currency.Default.Build();

        DbContext.AddRange(trade, newCurrency);
        await DbContext.SaveChangesAsync();

        var updateTradeRequestModel = new UpdateTradeRequestModel
        {
            TradeId = trade.Id,
            CurrencyId = newCurrency.Id
        };

        // act
        var response = await CreateInteractor().Execute(updateTradeRequestModel);

        // assert
        response.Value.Should().BeOfType<Completed>();

        var updatedTrade = await DbContextSingleOrDefault<Trade>(x => x.Id == trade.Id);
        updatedTrade.Should().NotBeNull();
        updatedTrade!.CurrencyId.Should().Be(newCurrency.Id);
    }

    [Fact]
    public async Task A_trades_closed_date_cannot_be_updated_if_it_is_before_the_opened_Date()
    {
        // arrange
        var trade = TestData.Trade.Default.Build();
        var _ = trade.Close(new Trade.CloseTradeDto(trade.Opened, 50, UtcNowStub));

        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync();

        var updateTradeRequestModel = new UpdateTradeRequestModel
        {
            TradeId = trade.Id,
            Closed = trade.Opened.AddSeconds(-1)
        };

        // act
        var response = await CreateInteractor().Execute(updateTradeRequestModel);

        // assert
        response.Value.Should().BeOfType<BusinessError>()
            .Which.Reason.Should().Be("The 'Closed' date must be after the 'Opened' date.");
    }

    [Fact]
    public async Task You_cant_update_the_closed_date_if_the_trade_has_not_yet_been_finished()
    {
        // arrange
        var trade = TestData.Trade.Default.Build();

        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync();

        var updateTradeRequestModel = new UpdateTradeRequestModel
        {
            TradeId = trade.Id,
            Closed = trade.Opened.AddSeconds(-1)
        };

        // act
        var response = await CreateInteractor().Execute(updateTradeRequestModel);

        // assert
        response.Value.Should().BeOfType<BusinessError>()
            .Which.Reason.Should()
            .Be("Updating 'Balance' and 'Closed' is only possible when a trade has already been closed.");
    }

    [Fact]
    public async Task The_balance_of_a_trade_can_be_updated_when_a_trade_is_finished()
    {
        // arrange
        var trade = TestData.Trade.Default.Build();
        var _ = trade.Close(new Trade.CloseTradeDto(trade.Opened, 50, UtcNowStub));
        const decimal newBalance = 100m;

        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync();

        var updateTradeRequestModel = new UpdateTradeRequestModel
        {
            TradeId = trade.Id,
            Balance = newBalance
        };

        // act
        var response = await CreateInteractor().Execute(updateTradeRequestModel);

        // assert
        response.Value.Should().BeOfType<Completed>();
        var updatedTrade = await DbContextSingleOrDefault<Trade>(x => x.Id == trade.Id);
        updatedTrade.Should().NotBeNull();
        updatedTrade!.Balance.Should().Be(newBalance);
    }

    [Fact]
    public async Task Notes_must_not_contain_more_than_4000_chars()
    {
        // arrange
        var trade = TestData.Trade.Default.Build();

        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync();

        var updateTradeRequestModel = new UpdateTradeRequestModel
        {
            TradeId = trade.Id,
            Notes = new string('a', 4001)
        };

        // act
        var response = await CreateInteractor().Execute(updateTradeRequestModel);

        // assert
        response.Value.Should().BeOfType<BadInput>()
            .Which.ValidationResult.Errors.Should().HaveCount(1)
            .And.Contain(x =>
                x.ErrorMessage ==
                "The length of 'Notes' must be 4000 characters or fewer. You entered 4001 characters.");
    }


    [Fact]
    public async Task Position_prices_can_be_successfully_updated()
    {
        // arrange
        var oldPositionPrice = new PositionPrices {Entry = 1.0m, StopLoss = null, TakeProfit = null, Exit = null};
        var newPositionPrices = new PositionPrices {Entry = 0.95m, StopLoss = 0.8m, TakeProfit = 1.4m, Exit = 1.25m};

        var trade = (TestData.Trade.Default with
                {
                    PositionPrices = oldPositionPrice
                }
            ).Build();
        var _ = trade.Close(new Trade.CloseTradeDto(trade.Opened, 50, UtcNowStub));

        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync();

        var updateTradeRequestModel = new UpdateTradeRequestModel
        {
            TradeId = trade.Id,
            EntryPrice = newPositionPrices.Entry,
            StopLoss = newPositionPrices.StopLoss,
            TakeProfit = newPositionPrices.TakeProfit,
            ExitPrice = newPositionPrices.Exit
        };

        // act
        var response = await CreateInteractor().Execute(updateTradeRequestModel);

        // assert
        response.Value.Should().BeOfType<Completed>();
        var updatedTrade = await DbContextSingleOrDefault<Trade>(x => x.Id == trade.Id);
        updatedTrade.Should().NotBeNull();
        updatedTrade!.PositionPrices.Should().Be(newPositionPrices);
    }

    private DateTime UtcNowStub()
    {
        return DateTime.Parse("2024-08-14T12:00:00").ToUtcKind();
    }
}