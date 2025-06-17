using QuantitativeTrading.Models.Records;
using QuantitativeTrading.Models.Records.ThreeMarkets;

namespace QuantitativeTrading.Strategies.ThreeMarkets
{
    /// <summary>
    /// 自動探索滾動最大漲跌幅參數策略
    /// </summary>
    public class AutoParamsCloseChange : CloseChange, IAutoParams
    {
        public AutoParamsCloseChange() : base(720, 720) { }

        /// <summary>
        /// 更新策略的參數
        /// </summary>
        /// <param name="bufferSize"> 觀察時間 </param>
        /// <param name="tradingInterval"> 交易間隔的時間 </param>
        public void UpdateParams(int bufferSize, int tradingInterval)
        {
            buffer.Resize(bufferSize);
            TradingInterval = tradingInterval;
        }

        /// <summary>
        /// 紀錄資料
        /// </summary>
        /// <param name="record"></param>
        public override void Recording(IStrategyModels record)
        {
            IAutoParamsCloseChange autoParamscloseChangeSumRecord = record as IAutoParamsCloseChange;
            autoParamscloseChangeSumRecord.Coin1ToCoinChangeSum = Coin1ToCoinChange;
            autoParamscloseChangeSumRecord.Coin2ToCoinChangeSum = Coin2ToCoinChange;
            autoParamscloseChangeSumRecord.ObservationTime = ObservationTime;
            autoParamscloseChangeSumRecord.TradingInterval = TradingInterval;
        }
    }
}
