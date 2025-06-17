using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantitativeTrading.Environments;
using QuantitativeTrading.Environments.ThreeMarkets;
using QuantitativeTrading.Models.Records.ThreeMarkets;
using QuantitativeTrading.Runners.ThreeMarkets;
using QuantitativeTrading.Strategies.ThreeMarkets;

namespace QuantitativeTrading.Tests.Runner.ThreeMarkets
{
    [TestClass]
    public class ThreeMarketsRunnerTests
    {
        private CloseChange closeChange;
        private PrivateObject privateObject;
        private Runner<CloseChange, CloseChangeRecordModel> runner;
        private ThreeMarketsEnvironmentMock env;

        [TestInitialize]
        public void Init()
        {
            closeChange = new(1, 1);
            env = new();
            runner = new(closeChange, env, null);
            privateObject = new(runner);
        }

        [TestMethod]
        public void TestNoTrading()
        {
            env.SetBalance(100);
            env.SetCoinBalance1(0);
            env.SetCoinBalance2(0);
            privateObject.Invoke("Trading", new object[] { StrategyAction.Coin });

            env.SetBalance(0);
            env.SetCoinBalance1(100);
            env.SetCoinBalance2(0);
            privateObject.Invoke("Trading", new object[] { StrategyAction.Coin1 });

            env.SetBalance(0);
            env.SetCoinBalance1(0);
            env.SetCoinBalance2(100);
            privateObject.Invoke("Trading", new object[] { StrategyAction.Coin2 });
            Assert.AreEqual(0, env.Actions.Count);
        }

        [TestMethod]
        public void TestTradingToCoin()
        {
            env.SetBalance(0);
            env.SetCoinBalance1(100);
            env.SetCoinBalance2(0);
            privateObject.Invoke("Trading", new object[] { StrategyAction.Coin });
            Assert.AreEqual(TradingAction.Sell, env.Actions[0].action);
            Assert.AreEqual(TradingMarket.Coin12Coin, env.Actions[0].market);

            env.SetBalance(0);
            env.SetCoinBalance1(0);
            env.SetCoinBalance2(100);
            privateObject.Invoke("Trading", new object[] { StrategyAction.Coin });
            Assert.AreEqual(TradingAction.Sell, env.Actions[1].action);
            Assert.AreEqual(TradingMarket.Coin22Coin, env.Actions[1].market);
        }

        [TestMethod]
        public void TestTradingToCoin1()
        {
            env.SetBalance(100);
            env.SetCoinBalance1(0);
            env.SetCoinBalance2(0);
            privateObject.Invoke("Trading", new object[] { StrategyAction.Coin1 });
            Assert.AreEqual(TradingAction.Buy, env.Actions[0].action);
            Assert.AreEqual(TradingMarket.Coin12Coin, env.Actions[0].market);

            closeChange.PolicyDecision(new() { Coin22Coin1Kline = new() { Close = -1 }, Coin12CoinKline = new() { Close = 1 }, Coin22CoinKline = new() });
            env.SetBalance(0);
            env.SetCoinBalance1(0);
            env.SetCoinBalance2(100);
            privateObject.Invoke("Trading", new object[] { StrategyAction.Coin1 });
            Assert.AreEqual(TradingAction.Sell, env.Actions[1].action);
            Assert.AreEqual(TradingMarket.Coin22Coin, env.Actions[1].market);
            Assert.AreEqual(TradingAction.Buy, env.Actions[2].action);
            Assert.AreEqual(TradingMarket.Coin12Coin, env.Actions[2].market);

            closeChange.PolicyDecision(new() { Coin22Coin1Kline = new() { Close = 1 }, Coin12CoinKline = new() { Close = 1 }, Coin22CoinKline = new() });
            env.SetBalance(0);
            env.SetCoinBalance1(0);
            env.SetCoinBalance2(100);
            privateObject.Invoke("Trading", new object[] { StrategyAction.Coin1 });
            Assert.AreEqual(TradingAction.Sell, env.Actions[3].action);
            Assert.AreEqual(TradingMarket.Coin22Coin1, env.Actions[3].market);
        }

        [TestMethod]
        public void TestTradingToCoin2()
        {
            env.SetBalance(100);
            env.SetCoinBalance1(0);
            env.SetCoinBalance2(0);
            privateObject.Invoke("Trading", new object[] { StrategyAction.Coin2 });
            Assert.AreEqual(TradingAction.Buy, env.Actions[0].action);
            Assert.AreEqual(TradingMarket.Coin22Coin, env.Actions[0].market);

            closeChange.PolicyDecision(new() { Coin22Coin1Kline = new() { Close = -1 }, Coin12CoinKline = new(), Coin22CoinKline = new() { Close = 1 } });
            env.SetBalance(0);
            env.SetCoinBalance1(100);
            env.SetCoinBalance2(0);
            privateObject.Invoke("Trading", new object[] { StrategyAction.Coin2 });
            Assert.AreEqual(TradingAction.Sell, env.Actions[1].action);
            Assert.AreEqual(TradingMarket.Coin12Coin, env.Actions[1].market);
            Assert.AreEqual(TradingAction.Buy, env.Actions[2].action);
            Assert.AreEqual(TradingMarket.Coin22Coin, env.Actions[2].market);

            closeChange.PolicyDecision(new() { Coin22Coin1Kline = new() { Close = 1 }, Coin12CoinKline = new(), Coin22CoinKline = new() { Close = 1 } });
            env.SetBalance(0);
            env.SetCoinBalance1(100);
            env.SetCoinBalance2(0);
            privateObject.Invoke("Trading", new object[] { StrategyAction.Coin2 });
            Assert.AreEqual(TradingAction.Buy, env.Actions[3].action);
            Assert.AreEqual(TradingMarket.Coin22Coin1, env.Actions[3].market);
        }
    }
}
