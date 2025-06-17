using QuantitativeTrading.Environments;
using QuantitativeTrading.Environments.ThreeMarkets;
using QuantitativeTrading.Models.Records;
using QuantitativeTrading.Strategies.ThreeMarkets;
using System.Runtime.CompilerServices;
using IEnvironmentModels = QuantitativeTrading.Models.Records.ThreeMarkets.IEnvironmentModels;

namespace QuantitativeTrading.Runners.ThreeMarkets
{
    public class AutoSellCloseChangeRunner<T, U> : Runner<T, U>
        where T : AutoSellCloseChangeStrategy
        where U : class, IEnvironmentModels, IStrategyModels, new()
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="strategy"> 策略 </param>
        /// <param name="environment"> 回測環境 </param>
        /// <param name="recorder"> 交易紀錄器 </param>
        public AutoSellCloseChangeRunner(T strategy, IThreeMarketEnvironment environment, Recorder<U> recorder)
            : base(strategy, environment, recorder) { }

        /// <summary>
        /// 執行交易動作
        /// 檢查環境的資產在哪個幣上面
        /// 根據策略結果決定如何交易
        /// 例: 用 USDT 買 BTC
        /// </summary>
        /// <param name="action"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void Trading(StrategyAction action)
        {
            SpotEnvironment spotEnvironment = environment as SpotEnvironment;
            if (action == StrategyAction.Coin)
            {
                if (environment.Coin1Asset > environment.Balance && environment.Coin1Asset > environment.Coin2Asset)
                    environment.Trading(TradingAction.Sell, TradingMarket.Coin12Coin);
                else if (environment.Coin2Asset > environment.Balance && environment.Coin2Asset > environment.Coin1Asset)
                    environment.Trading(TradingAction.Sell, TradingMarket.Coin22Coin);
                strategy.CurrentHoldCoin = action;
                strategy.BuyPrice = default;
            }
            else if (action == StrategyAction.Coin1)
            {
                if (environment.Balance > environment.Coin1Asset && environment.Balance > environment.Coin2Asset)
                {
                    environment.Trading(TradingAction.Buy, TradingMarket.Coin12Coin);
                    strategy.CurrentHoldCoin = action;
                    strategy.BuyPrice = spotEnvironment.CurrentKline.Coin12CoinKline.Close;
                }
                else if (environment.Coin2Asset > environment.Coin1Asset && environment.Balance < environment.Coin2Asset)
                {
                    if (strategy.BestCoin1ToCoin2Path(action) == BestPath.Path1)
                        TwoStepTrading(TradingMarket.Coin22Coin, TradingMarket.Coin12Coin);
                    else
                        environment.Trading(TradingAction.Sell, TradingMarket.Coin22Coin1);
                    strategy.CurrentHoldCoin = action;
                    strategy.BuyPrice = spotEnvironment.CurrentKline.Coin12CoinKline.Close;
                }
            }
            else if (action == StrategyAction.Coin2)
            {
                if (environment.Balance > environment.Coin1Asset && environment.Balance > environment.Coin2Asset)
                {
                    environment.Trading(TradingAction.Buy, TradingMarket.Coin22Coin);
                    strategy.CurrentHoldCoin = action;
                    strategy.BuyPrice = spotEnvironment.CurrentKline.Coin22CoinKline.Close;
                }
                else if (environment.Coin2Asset < environment.Coin1Asset && environment.Balance < environment.Coin1Asset)
                {
                    if (strategy.BestCoin1ToCoin2Path(action) == BestPath.Path1)
                        TwoStepTrading(TradingMarket.Coin12Coin, TradingMarket.Coin22Coin);
                    else
                        environment.Trading(TradingAction.Buy, TradingMarket.Coin22Coin1);
                    strategy.CurrentHoldCoin = action;
                    strategy.BuyPrice = spotEnvironment.CurrentKline.Coin22CoinKline.Close;
                }
            }
        }
    }
}
