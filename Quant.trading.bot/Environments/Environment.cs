using QuantitativeTrading.Data.DataProviders;
using QuantitativeTrading.Models.Records;
using System;
using System.Runtime.CompilerServices;

namespace QuantitativeTrading.Environments
{
    /// <summary>
    /// 回測環境
    /// </summary>
    /// <typeparam name="T"> 回測資料的結構 </typeparam>
    /// <typeparam name="U"> 回測資料 </typeparam>
    public abstract class Environment<T, U>
        where U : KlineDataProvider<T, U>
    {
        /// <summary>
        /// 資產
        /// </summary>
        public abstract decimal Assets { get; }
        /// <summary>
        /// 遊戲結束
        /// 破產
        /// 資料集跑完
        /// </summary>
        public bool IsGameOver => Assets <= gameOverAssets || dataProvider.IsEnd;
        /// <summary>
        /// 餘額
        /// </summary>
        public decimal Balance { get; protected set; }
        /// <summary>
        /// 當前的 K line
        /// </summary>
        public T CurrentKline => dataProvider.Current;

        /// <summary>
        /// 手續費 (已經除100)
        /// </summary>
        protected readonly decimal handlingFee;
        /// <summary>
        /// 回測資料集
        /// </summary>
        protected readonly U dataProvider;
        /// <summary>
        /// 破產資產
        /// </summary>
        private readonly decimal gameOverAssets;
        /// <summary>
        /// 最小交易單位
        /// </summary>
        private readonly decimal smallestUnit;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="dataProvider"> 回測資料 </param>
        /// <param name="environmentParams"> 回測環境參數 </param>
        public Environment(U dataProvider, EnvironmentParams environmentParams)
            => (this.dataProvider, Balance, gameOverAssets, handlingFee, smallestUnit) = (dataProvider, environmentParams.Balance, environmentParams.GameOverAssets, environmentParams.HandlingFee / 100, ConvertSmallestUnit(environmentParams.SmallestUnit));

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="dataProvider"> 回測資料 </param>
        /// <param name="balance"> 初始餘額 </param>
        /// <param name="gameOverAssets"> 破產資產 </param>
        /// <param name="handlingFee"> 手續費% </param>
        /// <param name="smallestUnit"> 最小交易單位(小數點後幾位) </param>
        public Environment(U dataProvider, decimal balance, decimal gameOverAssets, decimal handlingFee, int smallestUnit)
            => (this.dataProvider, Balance, this.gameOverAssets, this.handlingFee, this.smallestUnit) = (dataProvider, balance, gameOverAssets, handlingFee / 100, ConvertSmallestUnit(smallestUnit));

        /// <summary>
        /// 回測資料移動到下一個時間
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool MoveNextTime(out T model)
            => dataProvider.MoveNext(out model);

        /// <summary>
        /// 複製一份當前狀態的 data provider
        /// </summary>
        /// <returns></returns>
        public U CloneCurrentDataProvider()
            => dataProvider.CloneAllStatus();

        /// <summary>
        /// 紀錄環境狀態
        /// </summary>
        /// <param name="record"></param>
        public abstract void Recording(IEnvironmentModels record);

        /// <summary>
        /// 屏蔽掉最小交易單位，小數點後的數字
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected decimal DecimalPointMask(decimal d)
            => Math.Floor(d * smallestUnit) / smallestUnit;

        /// <summary>
        /// 轉換最小交易單位
        /// 例如: 小數點後 3 位 => 0.001
        /// </summary>
        /// <param name="smallestUnit"></param>
        /// <returns></returns>
        private static decimal ConvertSmallestUnit(int smallestUnit)
            => Convert.ToDecimal(Math.Pow(10, smallestUnit));
    }

    /// <summary>
    /// 回測環境參數
    /// </summary>
    /// <param name="Balance"> 初始餘額 </param>
    /// <param name="GameOverAssets"> 破產資產 </param>
    /// <param name="HandlingFee"> 手續費% </param>
    /// <param name="SmallestUnit"> 最小交易單位(小數點後幾位) </param>
    public record EnvironmentParams(decimal Balance, decimal GameOverAssets, decimal HandlingFee, int SmallestUnit);

    public enum TradingAction
    {
        Buy = 0,
        Sell
    }
}
