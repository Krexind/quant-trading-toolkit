using QuantitativeTrading.Models;
using QuantitativeTrading.Models.Records;
using QuantitativeTrading.Models.Records.ThreeMarkets;

namespace QuantitativeTrading.Strategies.ThreeMarkets
{
    public class AutoSellCloseChange : AutoSellCloseChangeStrategy
    {
        private readonly decimal sellCondition;
        private decimal change = default;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="bufferSize"> 需要觀察的天數 </param>
        /// <param name="tradingInterval"> 每次交易的間隔 </param>
        public AutoSellCloseChange(int bufferSize, int tradingInterval, decimal sellCondition)
            : base(bufferSize, tradingInterval) => this.sellCondition = sellCondition;

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

            if ((CurrentHoldCoin == StrategyAction.Coin1 || CurrentHoldCoin == StrategyAction.Coin2) && change < sellCondition)
                return StrategyAction.Coin;

            if (buffer.Count < ObservationTime || !CanTrading())
                return StrategyAction.WaitBuffer;

            if (Coin1ToCoinChange < 0 && Coin2ToCoinChange < 0)
                return StrategyAction.Coin;
            if (Coin1ToCoinChange > Coin2ToCoinChange)
                return StrategyAction.Coin1;
            else
                return StrategyAction.Coin2;
        }

        protected override void ComputeParameter()
        {
            base.ComputeParameter();

            change = CurrentHoldCoin == StrategyAction.Coin
                ? default
                : CurrentHoldCoin == StrategyAction.Coin1
                    ? (buffer.Last.Coin12CoinKline.Close - BuyPrice) / BuyPrice * 100
                    : (buffer.Last.Coin22CoinKline.Close - BuyPrice) / BuyPrice * 100;
        }

        /// <summary>
        /// 紀錄資料
        /// </summary>
        /// <param name="record"></param>
        public override void Recording(IStrategyModels record)
        {
            IAutoSellCloseChange closeChangeSumRecord = record as IAutoSellCloseChange;
            closeChangeSumRecord.Coin1ToCoinChangeSum = Coin1ToCoinChange;
            closeChangeSumRecord.Coin2ToCoinChangeSum = Coin2ToCoinChange;
            closeChangeSumRecord.BuyPrice = BuyPrice;
            closeChangeSumRecord.CurrentHoldCoin = CurrentHoldCoin.ToString();
            closeChangeSumRecord.BuyChange = change;
        }
    }
}
