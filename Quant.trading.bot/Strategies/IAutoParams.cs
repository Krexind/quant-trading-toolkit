namespace QuantitativeTrading.Strategies
{
    public interface IAutoParams
    {
        void UpdateParams(int bufferSize, int tradingInterval);
    }
}
