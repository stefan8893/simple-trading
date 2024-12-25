using Autofac;
using FluentAssertions;
using OneOf.Types;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Infrastructure.Extensions;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.Shared;
using SimpleTrading.Domain.Trading.UseCases.UpdateTrade;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.Domain.Tests.Trading.UseCases;

public class UpdateTradeTests : DomainTests
{
    private readonly DateTime _utcNow = DateTime.Parse("2024-08-14T12:00:00").ToUtcKind();
    private IUpdateTrade Interactor => ServiceLocator.Resolve<IUpdateTrade>();

    protected override void OverrideServices(ContainerBuilder builder)
    {
        builder.Register<UtcNow>(_ => () => _utcNow);
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
        var response = await Interactor.Execute(updateTradeRequestModel);

        // assert
        var badInput = response.Value.Should().BeOfType<BadInput>();
        badInput.Which.ValidationResult.Errors.Should().HaveCount(1)
            .And.Contain(x => x.ErrorMessage == "'Trade size' must be greater than '0'.");
    }

    [Fact]
    public async Task A_trades_result_must_be_in_enum_range()
    {
        // arrange
        var trade = (TestData.Trade.Default with
        {
            Opened = _utcNow,
            Closed = _utcNow,
            Balance = 0
        }).Build();

        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync();

        var updateTradeRequestModel = new UpdateTradeRequestModel
        {
            TradeId = trade.Id,
            ManuallyEnteredResult = (ResultModel?) 50
        };

        // act
        var response = await Interactor.Execute(updateTradeRequestModel);

        // assert
        var badInput = response.Value.Should().BeOfType<BadInput>();
        badInput.Which.ValidationResult.Errors.Should().HaveCount(1)
            .And.Contain(x => x.ErrorMessage == "'Result' has a range of values which does not include '50'." &&
                              x.PropertyName == "ManuallyEnteredResult");
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
        var response = await Interactor.Execute(updateTradeRequestModel);

        // assert
        var badInput = response.Value.Should().BeOfType<BadInput>();
        badInput.Which.ValidationResult.Errors.Should().HaveCount(1)
            .And.Contain(x => x.ErrorMessage == "'Entry price' must be greater than '0'." &&
                              x.PropertyName == "EntryPrice");
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
        var response = await Interactor.Execute(updateTradeRequestModel);

        // assert
        var badInput = response.Value.Should().BeOfType<BadInput>();
        badInput.Which.ValidationResult.Errors.Should().HaveCount(1)
            .And.Contain(x => x.ErrorMessage == "'Stop loss' must be greater than '0'." &&
                              x.PropertyName == "StopLoss");
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
        var response = await Interactor.Execute(updateTradeRequestModel);

        // assert
        var badInput = response.Value.Should().BeOfType<BadInput>();
        badInput.Which.ValidationResult.Errors.Should().HaveCount(1)
            .And.Contain(x => x.ErrorMessage == "'Take profit' must be greater than '0'." &&
                              x.PropertyName == "TakeProfit");
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
        var response = await Interactor.Execute(updateTradeRequestModel);

        // assert
        var badInput = response.Value.Should().BeOfType<BadInput>();
        badInput.Which.ValidationResult.Errors.Should().HaveCount(1)
            .And.Contain(x => x.ErrorMessage == "'Exit price' must be greater than '0'." &&
                              x.PropertyName == "ExitPrice");
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
        var response = await Interactor.Execute(updateTradeRequestModel);

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
        var response = await Interactor.Execute(updateTradeRequestModel);

        // assert
        response.Value.Should().BeOfType<Completed<UpdateTradeResponseModel>>();

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
        var response = await Interactor.Execute(updateTradeRequestModel);

        // assert
        var updatedTradeId = response.Value.Should()
            .BeOfType<Completed<UpdateTradeResponseModel>>()
            .Which.Data.TradeId;

        var updatedTrade = await DbContextSingleOrDefault<Trade>(x => x.Id == updatedTradeId);
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
        var response = await Interactor.Execute(updateTradeRequestModel);

        // assert
        response.Value.Should().BeOfType<Completed<UpdateTradeResponseModel>>();

        var updatedTrade = await DbContextSingleOrDefault<Trade>(x => x.Id == trade.Id);
        updatedTrade.Should().NotBeNull();
        updatedTrade!.CurrencyId.Should().Be(newCurrency.Id);
    }

    [Fact]
    public async Task A_trades_closed_date_cannot_be_updated_if_it_is_before_the_opened_Date()
    {
        // arrange
        var trade = TestData.Trade.Default.Build();
        _ = trade.Close(new CloseTradeConfiguration(trade.Opened, 50, UtcNowStub));

        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync();

        var updateTradeRequestModel = new UpdateTradeRequestModel
        {
            TradeId = trade.Id,
            Closed = new DateTimeOffset(trade.Opened.AddSeconds(-1))
        };

        // act
        var response = await Interactor.Execute(updateTradeRequestModel);

        // assert
        response.Value.Should().BeOfType<BusinessError>()
            .Which.Details.Should().Be("'Closed' must be after 'Opened'.");
    }

    [Fact]
    public async Task Updating_a_trades_opened_date_to_be_more_than_one_day_in_the_future_is_not_possible()
    {
        // arrange
        var trade = TestData.Trade.Default.Build();
        _ = trade.Close(new CloseTradeConfiguration(trade.Opened, 50, UtcNowStub));

        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync();

        var updateTradeRequestModel = new UpdateTradeRequestModel
        {
            TradeId = trade.Id,
            Opened = new DateTimeOffset(_utcNow.AddDays(1).AddSeconds(1))
        };

        // act
        var response = await Interactor.Execute(updateTradeRequestModel);

        // assert
        response.Value.Should().BeOfType<BadInput>()
            .Which.ValidationResult.Errors.Should().HaveCount(1)
            .And.Contain(x =>
                x.ErrorMessage == "'Opened' must be less than or equal to '15.08.2024 14:00'.")
            .And.Contain(x => x.PropertyName == "Opened");
    }

    [Fact]
    public async Task You_cant_update_the_closed_date_if_the_trade_has_not_yet_been_closed()
    {
        // arrange
        var trade = TestData.Trade.Default.Build();

        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync();

        var updateTradeRequestModel = new UpdateTradeRequestModel
        {
            TradeId = trade.Id,
            Closed = new DateTimeOffset(trade.Opened)
        };

        // act
        var response = await Interactor.Execute(updateTradeRequestModel);

        // assert
        var badInput = response.Value.Should().BeOfType<BadInput>();
        badInput.Which.ValidationResult.Errors.Should().HaveCount(1)
            .And.Contain(x => x.PropertyName == "Closed" &&
                              x.ErrorMessage == "'Closed' can only be updated, if the trade has already been closed.");
    }

    [Fact]
    public async Task You_cant_update_the_balance_if_the_trade_has_not_yet_been_closed()
    {
        // arrange
        var trade = TestData.Trade.Default.Build();

        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync();

        var updateTradeRequestModel = new UpdateTradeRequestModel
        {
            TradeId = trade.Id,
            Balance = 50m
        };

        // act
        var response = await Interactor.Execute(updateTradeRequestModel);

        // assert
        var badInput = response.Value.Should().BeOfType<BadInput>();
        badInput.Which.ValidationResult.Errors.Should().HaveCount(1)
            .And.Contain(x => x.PropertyName == "Balance" &&
                              x.ErrorMessage == "'Balance' can only be updated, if the trade has already been closed.");
    }

    [Fact]
    public async Task The_balance_of_a_trade_can_be_updated_when_a_trade_is_finished()
    {
        // arrange
        var trade = TestData.Trade.Default.Build();
        _ = trade.Close(new CloseTradeConfiguration(trade.Opened, 50, UtcNowStub));
        const decimal newBalance = 100m;

        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync();

        var updateTradeRequestModel = new UpdateTradeRequestModel
        {
            TradeId = trade.Id,
            Balance = newBalance
        };

        // act
        var response = await Interactor.Execute(updateTradeRequestModel);

        // assert
        response.Value.Should().BeOfType<Completed<UpdateTradeResponseModel>>();
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
        var response = await Interactor.Execute(updateTradeRequestModel);

        // assert
        response.Value.Should().BeOfType<BadInput>()
            .Which.ValidationResult.Errors.Should().HaveCount(1)
            .And.Contain(x =>
                x.ErrorMessage ==
                "The length of 'Notes' must be 4000 characters or fewer. You entered 4001 characters.")
            .And.Contain(x => x.PropertyName == "Notes");
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
        _ = trade.Close(new CloseTradeConfiguration(trade.Opened, 50, UtcNowStub));

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
        var response = await Interactor.Execute(updateTradeRequestModel);

        // assert
        response.Value.Should().BeOfType<Completed<UpdateTradeResponseModel>>();
        var updatedTrade = await DbContextSingleOrDefault<Trade>(x => x.Id == trade.Id);
        updatedTrade.Should().NotBeNull();
        updatedTrade!.PositionPrices.Should().Be(newPositionPrices);
    }

    [Fact]
    public async Task Updating_position_prices_of_a_closed_trade_leads_to_recalculation_of_the_performance()
    {
        // arrange
        var oldPositionPrice = new PositionPrices {Entry = 0.95m, StopLoss = 0.8m, TakeProfit = 1.4m, Exit = 1.25m};

        var trade = (TestData.Trade.Default with
                {
                    Balance = 500,
                    Opened = UtcNowStub(),
                    Closed = UtcNowStub(),
                    PositionPrices = oldPositionPrice
                }
            ).Build();

        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync();

        var updateTradeRequestModel = new UpdateTradeRequestModel
        {
            TradeId = trade.Id,
            TakeProfit = 1.30m,
            StopLoss = new None(),
            ExitPrice = new None(),
            Notes = new None(),
            ManuallyEnteredResult = new None()
        };

        // act
        var response = await Interactor.Execute(updateTradeRequestModel);

        // assert
        response.Value.Should().BeOfType<Completed<UpdateTradeResponseModel>>();
        var updatedTrade = await DbContextSingleOrDefault<Trade>(x => x.Id == trade.Id);
        updatedTrade?.Result.Should().NotBeNull();
        updatedTrade!.Result!.Performance.Should().Be(85);
    }

    [Fact]
    public async Task Manually_entered_results_can_be_updated_multiple_times()
    {
        // arrange
        var trade = (TestData.Trade.Default with
        {
            Opened = _utcNow,
            Closed = _utcNow,
            Balance = 500m,
            Result = ResultModel.Win
        }).Build();
        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync();

        trade.IsClosed.Should().BeTrue();
        _ = await Interactor.Execute(new UpdateTradeRequestModel
            {TradeId = trade.Id, ManuallyEnteredResult = ResultModel.Win});

        // act
        var response = await Interactor.Execute(new UpdateTradeRequestModel
        {
            TradeId = trade.Id,
            ManuallyEnteredResult = ResultModel.Mediocre
        });

        // assert
        response.Value.Should().BeOfType<Completed<UpdateTradeResponseModel>>();
        var updatedTrade = await DbContextSingleOrDefault<Trade>(x => x.Id == trade.Id);
        updatedTrade.Should().NotBeNull();
        updatedTrade!.Result.Should().NotBeNull();
        updatedTrade.Result!.Name.Should().Be(Result.Mediocre);
        updatedTrade.Result.Source.Should().Be(ResultSource.ManuallyEntered);
    }

    [Fact]
    public async Task Result_cannot_be_overriden_since_the_trade_is_not_closed()
    {
        // arrange
        var trade = TestData.Trade.Default.Build();
        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync();

        trade.IsClosed.Should().BeFalse();

        // act
        var response = await Interactor.Execute(new UpdateTradeRequestModel
        {
            TradeId = trade.Id,
            ManuallyEnteredResult = ResultModel.Mediocre
        });

        // assert
        var badInput = response.Value.Should().BeOfType<BadInput>();
        badInput.Which.ValidationResult.Errors.Should().HaveCount(1)
            .And.Contain(x => x.PropertyName == "ManuallyEnteredResult" &&
                              x.ErrorMessage == "'Result' can only be updated, if the trade has already been closed.");
    }

    [Fact]
    public async Task Result_cannot_be_overriden_to_null_since_the_trade_is_not_closed()
    {
        // arrange
        var trade = TestData.Trade.Default.Build();
        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync();

        trade.IsClosed.Should().BeFalse();

        // act
        var updateTradeRequestModel = new UpdateTradeRequestModel
        {
            TradeId = trade.Id,
            ManuallyEnteredResult = null
        };
        var response = await Interactor.Execute(updateTradeRequestModel);

        // assert
        var badInput = response.Value.Should().BeOfType<BadInput>();
        badInput.Which.ValidationResult.Errors.Should().HaveCount(1)
            .And.Contain(x => x.PropertyName == "ManuallyEnteredResult" &&
                              x.ErrorMessage == "'Result' can only be updated, if the trade has already been closed.");
    }

    [Fact]
    public async Task Result_can_be_overriden_since_the_trade_is_closed()
    {
        // arrange
        var trade = (TestData.Trade.Default with
        {
            Opened = _utcNow,
            Closed = _utcNow,
            Balance = 0m
        }).Build();
        DbContext.Trades.Add(trade);
        await DbContext.SaveChangesAsync();

        trade.IsClosed.Should().BeTrue();

        // act
        var response = await Interactor.Execute(new UpdateTradeRequestModel
        {
            TradeId = trade.Id,
            ManuallyEnteredResult = ResultModel.Loss
        });

        // assert
        response.Value.Should().BeOfType<Completed<UpdateTradeResponseModel>>();

        var updatedTrade = await DbContextSingleOrDefault<Trade>(x => x.Id == trade.Id);
        updatedTrade.Should().NotBeNull();
        updatedTrade!.Result.Should().NotBeNull();
        updatedTrade.Result!.Name.Should().Be(Result.Loss);
    }

    private DateTime UtcNowStub()
    {
        return _utcNow;
    }
}