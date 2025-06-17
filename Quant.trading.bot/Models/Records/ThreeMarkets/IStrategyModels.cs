namespace QuantitativeTrading.Models.Records.ThreeMarkets
{
    public interface ICloseChangeModels : IStrategyModels
    {
        public decimal Coin1ToCoinChangeSum { get; set; }
        public decimal Coin2ToCoinChangeSum { get; set; }
    }

    public interface IAutoParamsCloseChange : ICloseChangeModels
    {
        public int ObservationTime { get; set; }
        public int TradingInterval { get; set; }
    }

    public interface IAutoSellCloseChange : ICloseChangeModels
    {
        public decimal BuyPrice { set; get; }
        public decimal BuyChange { set; get; }
        public string CurrentHoldCoin { set; get; }
    }

    public interface IAutoSellAverageCloseChange : IAutoSellCloseChange
    {
        public decimal CloseAverage { set; get; }
    }
}
