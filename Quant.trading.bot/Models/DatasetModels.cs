namespace QuantitativeTrading.Models
{
    public class ThreeMarketsDatasetModel
    {
        public KlineModel[] Coin12CoinKlines { get; set; }
        public KlineModel[] Coin22CoinKlines { get; set; }
        public KlineModel[] Coin22Coin1Klines { get; set; }
    }
}
