using QuantitativeTrading.Data.DataProviders;
using QuantitativeTrading.Environments;
using QuantitativeTrading.Environments.ThreeMarkets;
using QuantitativeTrading.Models;
using System;
using System.Collections.Generic;

namespace QuantitativeTrading.Tests
{
    public static class Mocks
    {
        public static ThreeMarketsDataProvider ThreeMarketsDataProviderMock()
            => new(CreateThreeMarketsModelMock());

        public static ThreeMarketsDatasetModel CreateThreeMarketsModelMock()
        {
            DateTime now = DateTime.Now;
            KlineModel[] Coin12CoinKlines = new KlineModel[] { 
                new() { Close = 10, Date = now }, 
                new() { Close = 11, Date = now.AddMinutes(1) }, 
                new() { Close = 9, Date = now.AddMinutes(2) }, 
                new() { Close = 10, Date = now.AddMinutes(3) }, 
                new() { Close = 12, Date = now.AddMinutes(4) } };
            KlineModel[] Coin22CoinKlines = new KlineModel[] {
                new() { Close = 0.3M, Date = now },
                new() { Close = 0.4M, Date = now.AddMinutes(1) },
                new() { Close = 0.3M, Date = now.AddMinutes(2) },
                new() { Close = 0.2M, Date = now.AddMinutes(3) },
                new() { Close = 0.3M, Date = now.AddMinutes(4) } };
            KlineModel[] Coin22Coin1Klines = new KlineModel[] {
                new() { Close = (0.3M / 10) + 0.001M, Date = now },
                new() { Close = (0.4M / 11) - 0.002M, Date = now.AddMinutes(1) },
                new() { Close = (0.3M / 9) + 0.002M, Date = now.AddMinutes(2) },
                new() { Close = (0.2M / 10) - 0.001M, Date = now.AddMinutes(3) },
                new() { Close = (0.3M / 12) - 0.001M, Date = now.AddMinutes(4) } };

            return new() { Coin12CoinKlines = Coin12CoinKlines, Coin22CoinKlines = Coin22CoinKlines, Coin22Coin1Klines = Coin22Coin1Klines };
        }

        public static ThreeMarketsDataProvider CloseIsOneThreeMarketsDataProviderMock()
            => new(CreateCloseIsOneThreeMarketsModelMock());

        public static ThreeMarketsDatasetModel CreateCloseIsOneThreeMarketsModelMock()
        {
            DateTime now = DateTime.Now;
            KlineModel[] Coin12CoinKlines = new KlineModel[] {
                new() { Close = 1, Date = now }};
            KlineModel[] Coin22CoinKlines = new KlineModel[] {
                new() { Close = 1, Date = now }};
            KlineModel[] Coin22Coin1Klines = new KlineModel[] {
                new() { Close = 1, Date = now }};

            return new() { Coin12CoinKlines = Coin12CoinKlines, Coin22CoinKlines = Coin22CoinKlines, Coin22Coin1Klines = Coin22Coin1Klines };
        }
    }

    public class ThreeMarketsEnvironmentMock : SpotEnvironment
    {
        public readonly List<(TradingAction action, TradingMarket market)> Actions;

        public ThreeMarketsEnvironmentMock()
            : base(Mocks.CloseIsOneThreeMarketsDataProviderMock(), default, default, default, default)
            => Actions = new();

        public override void Trading(TradingAction action, TradingMarket market)
            => Actions.Add((action, market));

        public void SetBalance(decimal balance)
            => Balance = balance;

        public void SetCoinBalance1(decimal balance)
            => Coin1Balance = balance;

        public void SetCoinBalance2(decimal balance)
            => Coin2Balance = balance;
    }
}
