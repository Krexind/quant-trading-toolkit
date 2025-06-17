using QuantitativeTrading.Models.Records;

namespace QuantitativeTrading.Environments.ThreeMarkets
{
    public interface IThreeMarketEnvironment
    {
        decimal Assets { get; }
        decimal Balance { get; }
        decimal Coin1Balance { get; }
        decimal Coin2Balance { get; }
        decimal Coin1Asset { get; }
        decimal Coin2Asset { get; }

        void Recording(IEnvironmentModels record);
        void Trading(TradingAction action, TradingMarket market);
    }
}