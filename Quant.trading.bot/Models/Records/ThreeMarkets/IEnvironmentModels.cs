using System;

namespace QuantitativeTrading.Models.Records.ThreeMarkets
{
    public interface IEnvironmentModels : Records.IEnvironmentModels
    {
        public DateTime Date { get; set; }
        public decimal Coin12CoinClose { get; set; }
        public decimal Coin22CoinClose { get; set; }
        public decimal Coin22Coin1Close { get; set; }
        public decimal Assets { get; set; }
        public decimal Balance { get; set; }
        public decimal Coin1Balance { get; set; }
        public decimal Coin2Balance { get; set; }
        public decimal Coin1Asset { get; set; }
        public decimal Coin2Asset { get; set; }
    }
}
