using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantitativeTrading.Data.DataLoaders;
using QuantitativeTrading.Data.DataProviders;
using QuantitativeTrading.Environments;
using QuantitativeTrading.Runners.ThreeMarkets;
using QuantitativeTrading.Strategies.ThreeMarkets;
using System.Collections.Generic;

namespace QuantitativeTrading.Tests.Runner.ThreeMarkets
{
    [TestClass]
    public class RunFindAutoParamsCloseChangeBestParamsTests
    {
        private static ThreeMarketsDataProvider provider;

        [ClassInitialize]
        public static void ClassInit(TestContext _)
            => provider = new(ThreeMarketsDataLoader.LoadCsvDataAsync(Utils.btc_usdtPath, Utils.eth_usdtPath, Utils.eth_btcPath).Result);

        [TestInitialize]
        public void TestInit()
            => provider.Reset();

        [TestMethod]
        public void TestRunFindAutoParamsCloseChangeBestParamsJustStarted()
        {
            provider.MoveNext(out _);
            provider.MoveNext(out _);
            provider.MoveNext(out _);
            provider.MoveNext(out _);
            provider.MoveNext(out _);

            EnvironmentParams environmentParams = new(20000, 10000, 0.1m, 3);
            (int observationTime, int tradingInterval) = RunAllParams.RunFindAutoParamsCloseChangeBestParams(provider, environmentParams);
            Assert.AreEqual(10080, observationTime);
            Assert.AreEqual(720, tradingInterval);
        }

        [TestMethod]
        public void TestRunFindAutoParamsCloseChangeBestParams()
        {
            for (int i = 0; i < 31 * 1440 + 4; i++)
                provider.MoveNext(out _);

            EnvironmentParams environmentParams = new(20000, 10000, 0.1m, 3);
            (int observationTime, int _) = RunAllParams.RunFindAutoParamsCloseChangeBestParams(provider, environmentParams);
            Assert.AreEqual(3, observationTime);
        }

        [TestMethod]
        public void TestRunFindAutoParamsCloseChangeBestParamsNOCrash()
        {
            for (int i = 0; i < 100 * 1440 + 4; i++)
                provider.MoveNext(out _);

            EnvironmentParams environmentParams = new(20000, 10000, 0.1m, 3);
            (int _, int _) = RunAllParams.RunFindAutoParamsCloseChangeBestParams(provider, environmentParams);
        }

        [TestMethod]
        public void TestTradingInterval()
        {
            CloseChange strategy = new(2, 5);
            strategy.PolicyDecision(provider.Current);
            provider.MoveNext(out _);

            int counter = 0;
            while (!provider.IsEnd)
            {
                StrategyAction action = strategy.PolicyDecision(provider.Current);
                provider.MoveNext(out _);

                if (counter % 5 != 0)
                    Assert.AreEqual(StrategyAction.WaitBuffer, action);
                else
                    Assert.AreNotEqual(StrategyAction.WaitBuffer, action);

                counter++;
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(TestDatas), DynamicDataSourceType.Property)]
        public void TestDifferentObservationTimeSameStartTradingTime(int observationTime, int tradingInterval)
        {
            int day = 63;
            for (int i = 0; i < day * 1440; i++)
                provider.MoveNext(out _);

            int dataCount = observationTime + 31 * 1440;
            ThreeMarketsDataProvider newDataProvider = provider.Clone(provider.Index - dataCount, dataCount);
            CloseChange strategy = new(observationTime, tradingInterval);
            StrategyAction action = StrategyAction.WaitBuffer;
            while (action == StrategyAction.WaitBuffer)
            {
                action = strategy.PolicyDecision(newDataProvider.Current);
                newDataProvider.MoveNext(out _);
            }

            Assert.AreEqual(provider[(day - 31) * 1440].Coin22Coin1Kline.Date, newDataProvider.Current.Coin22Coin1Kline.Date);
        }

        public static IEnumerable<object[]> TestDatas
        {
            get
            {
                int[] observationTimes = new int[] { 3, 5, 15, 30, 60, 120, 240, 360, 480, 720, 1440, 4320, 10080, 20160, 30240, 40320 };
                int[] tradingIntervals = new int[] { 1, 3, 5, 15, 30, 60, 120, 240, 360, 480, 720, 1440 };
                foreach (int observationTime in observationTimes)
                    foreach (int tradingInterval in tradingIntervals)
                        yield return new object[] { observationTime, tradingInterval };
            }
        }
    }
}
