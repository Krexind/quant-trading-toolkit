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
    /// ����T�������^�����Ҳ�
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
        /// ��l��
        /// </summary>
        /// <param name="strategy"> ���� </param>
        /// <param name="environment"> �^������ </param>
        /// <param name="recorder"> ��������� </param>
        public Runner(T strategy, IThreeMarketEnvironment environment, Recorder<U> recorder)
            => (this.strategy, this.environment, this.recorder) = (strategy, environment, recorder);

        /// <summary>
        /// �}�l�^���A�����ƶ������γ]�w���Ҫ��C��̧C�l�B
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
        /// �������ʧ@
        /// �ˬd���Ҫ��겣�b���ӹ��W��
        /// �ھڵ������G�M�w�p����
        /// ��: �� USDT �R BTC
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
        /// ��B�J���
        /// �汼���S���A�R�A�ӹ�
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
