using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantitativeTrading.Data.DataLoaders;
using QuantitativeTrading.Models;
using System.Threading.Tasks;

namespace QuantitativeTrading.Tests.Models
{
    [TestClass]
    public class ThreeMarketsDataProviderModelTests
    {
        [TestMethod]
        public async Task TestLoadDataAsync()
        {
            ThreeMarketsDatasetModel dataset = await ThreeMarketsDataLoader.LoadCsvDataAsync(Utils.btc_usdtPath, Utils.eth_usdtPath, Utils.eth_btcPath);
            Assert.AreEqual("BTCUSDT-Spot", dataset.Coin12CoinKlines[0].StockCode);
            Assert.AreEqual(309559, dataset.Coin12CoinKlines.Length);
            Assert.AreEqual("ETHUSDT-Spot", dataset.Coin22CoinKlines[0].StockCode);
            Assert.AreEqual(309559, dataset.Coin22CoinKlines.Length);
            Assert.AreEqual("ETHBTC-Spot", dataset.Coin22Coin1Klines[0].StockCode);
            Assert.AreEqual(309559, dataset.Coin22Coin1Klines.Length);
        }
    }
}
