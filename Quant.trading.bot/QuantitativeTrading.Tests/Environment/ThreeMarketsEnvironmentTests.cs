using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantitativeTrading.Environments;
using QuantitativeTrading.Environments.ThreeMarkets;
using QuantitativeTrading.Models;

namespace QuantitativeTrading.Tests.Environment
{
    [TestClass]
    public class ThreeMarketsEnvironmentTests
    {
        private SpotEnvironment env;

        [TestInitialize]
        public void Init()
        {
            env = new(Mocks.ThreeMarketsDataProviderMock(), 100, 1, 1, 3);
        }

        [TestMethod]
        public void TestGetKline()
        {
            ThreeMarketsDataProviderModel model = env.CurrentKline;
            Assert.AreEqual(10, model.Coin12CoinKline.Close);
            Assert.AreEqual(0.3M, model.Coin22CoinKline.Close);
            Assert.AreEqual((0.3M / 10) + 0.001M, model.Coin22Coin1Kline.Close);

            env.MoveNextTime(out model);
            Assert.AreEqual(11, model.Coin12CoinKline.Close);
            Assert.AreEqual(0.4M, model.Coin22CoinKline.Close);
            Assert.AreEqual((0.4M / 11) - 0.002M, model.Coin22Coin1Kline.Close);
        }

        [TestMethod]
        public void TestNormalTransaction1()
        {
            env.Trading(TradingAction.Buy, TradingMarket.Coin12Coin);
            env.Trading(TradingAction.Buy, TradingMarket.Coin22Coin1);
            env.Trading(TradingAction.Sell, TradingMarket.Coin22Coin);
            Assert.IsTrue(env.Assets < 100 && env.Assets > 90);
            Assert.IsTrue(env.Balance < 100 && env.Balance > 90);
            Assert.IsTrue(env.Coin1Balance < 1 && env.Coin1Balance >= 0);
            Assert.IsTrue(env.Coin2Balance < 1 && env.Coin2Balance >= 0);
        }

        [TestMethod]
        public void TestNormalTransaction2()
        {
            env.Trading(TradingAction.Buy, TradingMarket.Coin22Coin);
            env.Trading(TradingAction.Sell, TradingMarket.Coin22Coin1);
            env.Trading(TradingAction.Sell, TradingMarket.Coin12Coin);
            Assert.IsTrue(env.Assets < 110 && env.Assets > 90);
            Assert.IsTrue(env.Balance < 110 && env.Balance > 90);
            Assert.IsTrue(env.Coin1Balance < 1 && env.Coin1Balance >= 0);
            Assert.IsTrue(env.Coin2Balance < 1 && env.Coin2Balance >= 0);
        }

        [TestMethod]
        public void TestInvalidTransaction1()
        {
            env.Trading(TradingAction.Sell, TradingMarket.Coin22Coin);
            Assert.AreEqual(100, env.Assets);
            Assert.AreEqual(100, env.Balance);
            Assert.AreEqual(0, env.Coin1Balance);
            Assert.AreEqual(0, env.Coin2Balance);
        }

        [TestMethod]
        public void TestInvalidTransaction2()
        {
            env.Trading(TradingAction.Buy, TradingMarket.Coin22Coin1);
            Assert.AreEqual(100, env.Assets);
            Assert.AreEqual(100, env.Balance);
            Assert.AreEqual(0, env.Coin1Balance);
            Assert.AreEqual(0, env.Coin2Balance);
        }

        [TestMethod]
        public void TestIsGameOver1()
        {
            env.MoveNextTime(out _);
            env.MoveNextTime(out _);
            env.MoveNextTime(out _);
            Assert.IsFalse(env.IsGameOver);
            env.MoveNextTime(out _);
            Assert.IsTrue(env.IsGameOver);
        }

        [TestMethod]
        public void TestIsGameOver2()
        {
            while(env.Assets > 1)
            {
                env.Trading(TradingAction.Buy, TradingMarket.Coin12Coin);
                env.Trading(TradingAction.Buy, TradingMarket.Coin22Coin1);
                env.Trading(TradingAction.Sell, TradingMarket.Coin22Coin);
            }
            Assert.IsTrue(env.IsGameOver);
        }
    }
}
