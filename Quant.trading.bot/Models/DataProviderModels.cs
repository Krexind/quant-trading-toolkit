namespace QuantitativeTrading.Models
{
    public class ThreeMarketsDataProviderModel
    {
        public ThreeMarketsDataProviderModel() { }
        public ThreeMarketsDataProviderModel(ThreeMarketsDataProviderModel threeMarketsDataProviderModel)
            => (Coin12CoinKline, Coin22CoinKline, Coin22Coin1Kline)
            = (threeMarketsDataProviderModel.Coin12CoinKline, threeMarketsDataProviderModel.Coin22CoinKline, threeMarketsDataProviderModel.Coin22Coin1Kline);

        public KlineModel Coin12CoinKline { get; set; }
        public KlineModel Coin22CoinKline { get; set; }
        public KlineModel Coin22Coin1Kline { get; set; }
    }
}
