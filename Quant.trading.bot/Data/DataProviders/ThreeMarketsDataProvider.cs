using QuantitativeTrading.Models;
using System.Linq;
using System.Runtime.CompilerServices;

namespace QuantitativeTrading.Data.DataProviders
{
    public class ThreeMarketsDataProvider : KlineDataProvider<ThreeMarketsDataProviderModel, ThreeMarketsDataProvider>
    {
        private ThreeMarketsDataProvider() { }

        public ThreeMarketsDataProvider(int index)
            => Index = index;

        public ThreeMarketsDataProvider(ThreeMarketsDatasetModel model)
            => models = Join(model);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ThreeMarketsDataProviderModel[] Join(ThreeMarketsDatasetModel model)
            => model.Coin12CoinKlines.AsParallel()
            .Join(model.Coin22CoinKlines.AsParallel(), B2AKline => B2AKline.Date, C2AKline => C2AKline.Date, (B2AKline, C2AKline) => new { B2AKline, C2AKline })
            .Join(model.Coin22Coin1Klines.AsParallel(), item => item.B2AKline.Date, C2BKline => C2BKline.Date, (item, C2BKline) => new ThreeMarketsDataProviderModel { Coin12CoinKline = item.B2AKline, Coin22CoinKline = item.C2AKline, Coin22Coin1Kline = C2BKline })
            .OrderBy(item => item.Coin12CoinKline.Date).ToArray();

        public override ThreeMarketsDataProvider Clone()
            => new() { models = models };

        public override ThreeMarketsDataProvider Clone(int startIndex, int length)
            => new() { models = models[startIndex..(startIndex + length)] };

        public override ThreeMarketsDataProvider CloneAllStatus()
            => new(Index) { models = models };
    }
}
