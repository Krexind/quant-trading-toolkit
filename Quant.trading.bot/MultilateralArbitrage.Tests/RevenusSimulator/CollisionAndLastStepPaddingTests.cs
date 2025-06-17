using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultilateralArbitrage.Models;
using MultilateralArbitrage.Modules.RevenusSimulator;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultilateralArbitrage.Tests.RevenusSimulator
{
    [TestClass]
    public class CollisionAndLastStepPaddingTests
    {
        private readonly ICollection<ICollection<Symbol>> allMarketMix;
        private readonly IDictionary<string, OrderBook> orderBooks;
        private readonly IDictionary<string, LatestPrice> latestPrices;
        private readonly CollisionAndLastStepPadding revenusSimulator;

        public CollisionAndLastStepPaddingTests()
        {
            allMarketMix = new Symbol[][]
            { new Symbol[]
            {
                new() { BaseAsset = "ETH", QuoteAsset = "USDT", Name = "ETHUSDT" },
                new() { BaseAsset = "ETH", QuoteAsset = "BTC", Name = "ETHBTC" },
                new() { BaseAsset = "BTC", QuoteAsset = "USDT", Name = "BTCUSDT" }
            } };

            orderBooks = new Dictionary<string, OrderBook>()
            {
                {"ETHUSDT", new(3939.35m, 0m, 4139.35m, 0m, null) },
                {"ETHBTC", new(0.080722m, 0m, 0.082722m, 0m, null) },
                {"BTCUSDT", new(48421m, 0m, 50421m, 0m, null) }
            };

            latestPrices = new Dictionary<string, LatestPrice>()
            {
                {"ETHUSDT", new(4039.35m, null) },
                {"ETHBTC", new(0.081722m, null) },
                {"BTCUSDT", new(49421m, null) }
            };

            revenusSimulator = new(allMarketMix, 0.1);
        }

        [TestMethod]
        public async Task TestCalculateAllIncomeAsync()
        {
            ICollection<(ICollection<Symbol> marketMix, float assets)> result = await revenusSimulator.CalculateAllIncomeAsync("USDT", orderBooks, latestPrices);
            Assert.AreEqual(-3.91f, result.ToArray()[0].assets, 0.01f);
        }
    }
}
