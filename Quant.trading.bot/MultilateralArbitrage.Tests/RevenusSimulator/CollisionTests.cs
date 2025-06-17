using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultilateralArbitrage.Models;
using MultilateralArbitrage.Modules.RevenusSimulator;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultilateralArbitrage.Tests.RevenusSimulator
{
    [TestClass]
    public class CollisionTests
    {
        private readonly ICollection<ICollection<Symbol>> allMarketMix;
        private readonly IDictionary<string, OrderBook> orderBooks;
        private readonly Collision revenusSimulator;

        public CollisionTests()
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

            revenusSimulator = new(allMarketMix, 0.1);
        }

        [TestMethod]
        public async Task TestCalculateAllIncomeAsync()
        {
            ICollection<(ICollection<Symbol> marketMix, float assets)> result = await revenusSimulator.CalculateAllIncomeAsync("USDT", orderBooks);
            Assert.AreEqual(-5.86f, result.ToArray()[0].assets, 0.01f);
        }
    }
}
