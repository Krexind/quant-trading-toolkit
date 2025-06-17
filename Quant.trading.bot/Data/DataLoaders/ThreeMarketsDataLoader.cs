using QuantitativeTrading.Models;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace QuantitativeTrading.Data.DataLoaders
{
    public class ThreeMarketsDataLoader : KlineDataLoader
    {
        /// <summary>
        /// 建立 ThreeMarketsDataProviderModel
        /// 
        /// 假設 Coin 是 USDT，Coin1 是 BTC，Coin2 是 ETH
        /// </summary>
        /// <param name="Coin12CoinPath"> BTC 對 USDT 價格的路徑 (1 BTC = X USDT) </param>
        /// <param name="Coin22CoinPath"> ETH 對 USDT 價格的路徑 (1 ETH = X USDT) </param>
        /// <param name="Coin22Coin1Path"> ETH 對 BTC 價格的路徑 (1 ETH = X BTC) </param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<ThreeMarketsDatasetModel> LoadCsvDataAsync(string Coin12CoinPath, string Coin22CoinPath, string Coin22Coin1Path, DateTime startTime, DateTime endTime)
            => LoadCsvDataAsync(new[] { LoadCSVAsync(Coin12CoinPath, startTime), LoadCSVAsync(Coin22CoinPath, startTime), LoadCSVAsync(Coin22Coin1Path, startTime, endTime) });

        /// <summary>
        /// 建立 ThreeMarketsDataProviderModel
        /// 
        /// 假設 Coin 是 USDT，Coin1 是 BTC，Coin2 是 ETH
        /// </summary>
        /// <param name="Coin12CoinPath"> BTC 對 USDT 價格的路徑 (1 BTC = X USDT) </param>
        /// <param name="Coin22CoinPath"> ETH 對 USDT 價格的路徑 (1 ETH = X USDT) </param>
        /// <param name="Coin22Coin1Path"> ETH 對 BTC 價格的路徑 (1 ETH = X BTC) </param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<ThreeMarketsDatasetModel> LoadCsvDataAsync(string Coin12CoinPath, string Coin22CoinPath, string Coin22Coin1Path, DateTime startTime)
            => LoadCsvDataAsync(new[] { LoadCSVAsync(Coin12CoinPath, startTime), LoadCSVAsync(Coin22CoinPath, startTime), LoadCSVAsync(Coin22Coin1Path, startTime) });

        /// <summary>
        /// 建立 ThreeMarketsDataProviderModel
        /// 
        /// 假設 Coin 是 USDT，Coin1 是 BTC，Coin2 是 ETH
        /// </summary>
        /// <param name="Coin12CoinPath"> BTC 對 USDT 價格的路徑 (1 BTC = X USDT) </param>
        /// <param name="Coin22CoinPath"> ETH 對 USDT 價格的路徑 (1 ETH = X USDT) </param>
        /// <param name="Coin22Coin1Path"> ETH 對 BTC 價格的路徑 (1 ETH = X BTC) </param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<ThreeMarketsDatasetModel> LoadCsvDataAsync(string Coin12CoinPath, string Coin22CoinPath, string Coin22Coin1Path)
            => LoadCsvDataAsync(new[] { LoadCSVAsync(Coin12CoinPath), LoadCSVAsync(Coin22CoinPath), LoadCSVAsync(Coin22Coin1Path) });

        private static async Task<ThreeMarketsDatasetModel> LoadCsvDataAsync(Task<KlineModel[]>[] tasks)
        {
            KlineModel[][] klineModels = await Task.WhenAll(tasks);
            return new() { Coin12CoinKlines = klineModels[0], Coin22CoinKlines = klineModels[1], Coin22Coin1Klines = klineModels[2] };
        }
    }
}
