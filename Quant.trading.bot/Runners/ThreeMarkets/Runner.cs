using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using QuantitativeTrading.Environments;
using QuantitativeTrading.Environments.ThreeMarkets;
using QuantitativeTrading.Models;
using QuantitativeTrading.Models.Records;
using QuantitativeTrading.Strategies.ThreeMarkets;
using IEnvironmentModels = QuantitativeTrading.Models.Records.ThreeMarkets.IEnvironmentModels;

namespace QuantitativeTrading.Runners.ThreeMarkets
{
    /// <summary>
    /// 執行三角市場回測的模組
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    public class Runner<T, U>
        where T : Strategy
        where U : class, IEnvironmentModels, IStrategyModels, new()
    {
        protected readonly Recorder<U> recorder;
        protected readonly IThreeMarketEnvironment environment;
        protected readonly T strategy;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="strategy"> 策略 </param>
        /// <param name="environment"> 回測環境 </param>
        /// <param name="recorder"> 交易紀錄器 </param>
        public Runner(T strategy, IThreeMarketEnvironment environment, Recorder<U> recorder)
            => (this.strategy, this.environment, this.recorder) = (strategy, environment, recorder);

        /// <summary>
        /// 開始回測，直到資料集結束或設定環境的低於最低餘額
        /// </summary>
        /// <returns></returns>
        public virtual async Task RunAsync()
        {
            SpotEnvironment spotEnvironment = environment as SpotEnvironment;
            while (!spotEnvironment.IsGameOver)
            {
                ThreeMarketsDataProviderModel data = spotEnvironment.CurrentKline;
                StrategyAction action = strategy.PolicyDecision(data);
                Trading(action);
                if (recorder is not null)
                {
                    U record = new();
                    environment.Recording(record);
                    strategy.Recording(record);
                    recorder.Insert(record);
                }
                spotEnvironment.MoveNextTime(out _);
            }

            if (recorder is not null)
                await recorder.SaveAsync();
        }

        /// <summary>
        /// 執行交易動作
        /// 檢查環境的資產在哪個幣上面
        /// 根據策略結果決定如何交易
        /// 例: 用 USDT 買 BTC
        /// </summary>
        /// <param name="action"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void Trading(StrategyAction action)
        {
            if (action == StrategyAction.Coin)
            {
                if (environment.Coin1Asset > environment.Balance && environment.Coin1Asset > environment.Coin2Asset)
                    environment.Trading(TradingAction.Sell, TradingMarket.Coin12Coin);
                else if (environment.Coin2Asset > environment.Balance && environment.Coin2Asset > environment.Coin1Asset)
                    environment.Trading(TradingAction.Sell, TradingMarket.Coin22Coin);
            }
            else if (action == StrategyAction.Coin1)
            {
                if (environment.Balance > environment.Coin1Asset && environment.Balance > environment.Coin2Asset)
                    environment.Trading(TradingAction.Buy, TradingMarket.Coin12Coin);
                else if (environment.Coin2Asset > environment.Coin1Asset && environment.Balance < environment.Coin2Asset)
                {
                    if (strategy.BestCoin1ToCoin2Path(action) == BestPath.Path1)
                        TwoStepTrading(TradingMarket.Coin22Coin, TradingMarket.Coin12Coin);
                    else
                        environment.Trading(TradingAction.Sell, TradingMarket.Coin22Coin1);
                }
            }
            else if (action == StrategyAction.Coin2)
            {
                if (environment.Balance > environment.Coin1Asset && environment.Balance > environment.Coin2Asset)
                    environment.Trading(TradingAction.Buy, TradingMarket.Coin22Coin);
                else if (environment.Coin2Asset < environment.Coin1Asset && environment.Balance < environment.Coin1Asset)
                {
                    if (strategy.BestCoin1ToCoin2Path(action) == BestPath.Path1)
                        TwoStepTrading(TradingMarket.Coin12Coin, TradingMarket.Coin22Coin);
                    else
                        environment.Trading(TradingAction.Buy, TradingMarket.Coin22Coin1);
                }
            }
        }

        /// <summary>
        /// 兩步驟交易
        /// 賣掉彼特幣，買乙太幣
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void TwoStepTrading(TradingMarket source, TradingMarket target)
        {
            environment.Trading(TradingAction.Sell, source);
            environment.Trading(TradingAction.Buy, target);
        }
    }
}
