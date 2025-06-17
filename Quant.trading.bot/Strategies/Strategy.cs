using QuantitativeTrading.Models.Records;

namespace QuantitativeTrading.Strategies
{
    public abstract class Strategy
    {
        public abstract void Recording(IStrategyModels record);
    }
}
