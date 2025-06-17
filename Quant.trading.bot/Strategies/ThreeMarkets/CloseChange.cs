using QuantitativeTrading.Models;
using QuantitativeTrading.Models.Records;
using QuantitativeTrading.Models.Records.ThreeMarkets;

namespace QuantitativeTrading.Strategies.ThreeMarkets
{
    /// <summary>
    /// 滾動最大漲跌幅策略
    /// </summary>
    public class CloseChange : Strategy
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="bufferSize"> 需要觀察的天數 </param>
        /// <param name="tradingInterval"> 每次交易的間隔 </param>
        public CloseChange(int bufferSize, int tradingInterval)
            : base(bufferSize, tradingInterval) { }

        /// <summary>
        /// 運行策略
        /// 
        /// 那個貨幣漲最多就持有那個貨幣，如果都是跌，就平倉
        /// </summary>
        /// <param name="model"> 當下的市場資訊 </param>
        /// <returns></returns>
        public override StrategyAction PolicyDecision(ThreeMarketsDataProviderModel model)
        {
            buffer.Enqueue(model);
            ComputeParameter();

            if (buffer.Count < ObservationTime || !CanTrading())
                return StrategyAction.WaitBuffer;

            if (Coin1ToCoinChange < 0 && Coin2ToCoinChange < 0)
                return StrategyAction.Coin;
            if (Coin1ToCoinChange > Coin2ToCoinChange)
                return StrategyAction.Coin1;
            else
                return StrategyAction.Coin2;
        }

        /// <summary>
        /// 紀錄資料
        /// </summary>
        /// <param name="record"></param>
        public override void Recording(IStrategyModels record)
        {
            ICloseChangeModels closeChangeSumRecord = record as ICloseChangeModels;
            closeChangeSumRecord.Coin1ToCoinChangeSum = Coin1ToCoinChange;
            closeChangeSumRecord.Coin2ToCoinChangeSum = Coin2ToCoinChange;
        }
    }
}
