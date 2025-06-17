using Magicodes.ExporterAndImporter.Csv;
using QuantitativeTrading.Data.DataProviders;
using QuantitativeTrading.Environments;
using QuantitativeTrading.Environments.ThreeMarkets;
using QuantitativeTrading.Models;
using QuantitativeTrading.Models.Records;
using QuantitativeTrading.Models.Records.ThreeMarkets;
using QuantitativeTrading.Strategies.ThreeMarkets;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IEnvironmentModels = QuantitativeTrading.Models.Records.ThreeMarkets.IEnvironmentModels;

namespace QuantitativeTrading.Runners.ThreeMarkets
{
    /// <summary>
    /// 執行所有參數，尋找最佳參數
    /// </summary>
    public static class RunAllParams
    {
        private static volatile int counter = 0;

        /// <summary>
        /// 運行 CloseChangeSum 策略的參數
        /// </summary>
        /// <param name="dataProvider"> 回測資料 </param>
        /// <param name="environmentParams"> 回測環境的參數 </param>
        /// <param name="observationTimes"> 使用歷史多久的時間觀察 </param>
        /// <param name="tradingIntervals"> 交易頻率(多久交易一次) </param>
        /// <param name="savePath"> 存檔位置 </param>
        /// <returns></returns>
        public static async Task RunCloseChangeAllParams(ThreeMarketsDataProvider dataProvider, EnvironmentParams environmentParams, int[] observationTimes, int[] tradingIntervals, string savePath)
        {
            List<(int observationTime, int tradingInterval)> combinations = new();
            ConcurrentBag<ThreeMarketsCombinationModels> results = new();
            foreach (int observationTime in observationTimes)
                foreach (int tradingInterval in tradingIntervals)
                    combinations.Add((observationTime, tradingInterval));

            Parallel.ForEach(combinations, new ParallelOptions { MaxDegreeOfParallelism = 6 }, (combination) =>
            {
                CloseChange strategy = new(combination.observationTime, combination.tradingInterval);
                string combinationName = $"{Utils.MinuteToHrOrDay(combination.observationTime)}-{Utils.MinuteToHrOrDay(combination.tradingInterval)}";
                ThreeMarketsCombinationModels result = RunAllDatasetParams<CloseChangeRecordModel>(dataProvider, strategy, environmentParams, combinationName, savePath).Result;
                results.Add(result);
                counter++;
                Console.WriteLine(counter);
            });

            var resultsArray = results.ToArray();
            await new CsvExporter().Export(Path.Combine(savePath, "CombinationResult.csv"), resultsArray);
        }

        /// <summary>
        /// 運行 AutoSellCloseChange 策略的參數
        /// </summary>
        /// <param name="dataProvider"> 回測資料 </param>
        /// <param name="environmentParams"> 回測環境的參數 </param>
        /// <param name="observationTimes"> 使用歷史多久的時間觀察 </param>
        /// <param name="tradingIntervals"> 交易頻率(多久交易一次) </param>
        /// <param name="savePath"> 存檔位置 </param>
        /// <returns></returns>
        public static async Task RunAutoSellCloseChangeAllParams(ThreeMarketsDataProvider dataProvider, EnvironmentParams environmentParams, int[] observationTimes, int[] tradingIntervals, decimal[] sellConditions, string savePath)
        {
            List<(int observationTime, int tradingInterval, decimal sellCondition)> combinations = new();
            ConcurrentBag<ThreeMarketsCombinationModels> results = new();
            foreach (int observationTime in observationTimes)
                foreach (int tradingInterval in tradingIntervals)
                    foreach (decimal sellCondition in sellConditions)
                        combinations.Add((observationTime, tradingInterval, sellCondition));

            Parallel.ForEach(combinations, new ParallelOptions { MaxDegreeOfParallelism = 10 }, (combination) =>
            {
                AutoSellCloseChange strategy = new(combination.observationTime, combination.tradingInterval, combination.sellCondition);
                string combinationName = $"{Utils.MinuteToHrOrDay(combination.observationTime)}-{Utils.MinuteToHrOrDay(combination.tradingInterval)}-{combination.sellCondition}";
                ThreeMarketsCombinationModels result = RunAllDatasetParams<AutoSellCloseChangeRecordModel>(dataProvider, strategy, environmentParams, combinationName, savePath).Result;
                results.Add(result);
                counter++;
                Console.WriteLine(counter);
            });

            var resultsArray = results.ToArray();
            await new CsvExporter().Export(Path.Combine(savePath, "CombinationResult.csv"), resultsArray);
        }

        /// <summary>
        /// 運行 AutoSellCloseChange 策略的參數
        /// </summary>
        /// <param name="dataProvider"> 回測資料 </param>
        /// <param name="environmentParams"> 回測環境的參數 </param>
        /// <param name="observationTimes"> 使用歷史多久的時間觀察 </param>
        /// <param name="tradingIntervals"> 交易頻率(多久交易一次) </param>
        /// <param name="savePath"> 存檔位置 </param>
        /// <returns></returns>
        public static async Task RunAutoSellAverageCloseChangeAllParams(ThreeMarketsDataProvider dataProvider, EnvironmentParams environmentParams, int[] observationTimes, int[] tradingIntervals, int[] movingAverageSizes, string savePath)
        {
            List<(int observationTime, int tradingInterval, int movingAverageSize)> combinations = new();
            ConcurrentBag<ThreeMarketsCombinationModels> results = new();
            foreach (int observationTime in observationTimes)
                foreach (int tradingInterval in tradingIntervals)
                    foreach (int movingAverageSize in movingAverageSizes)
                        combinations.Add((observationTime, tradingInterval, movingAverageSize));

            Parallel.ForEach(combinations, new ParallelOptions { MaxDegreeOfParallelism = 10 }, (combination) =>
            {
                AutoSellAverageCloseChange strategy = new(combination.observationTime, combination.tradingInterval, combination.movingAverageSize);
                string combinationName = $"{Utils.MinuteToHrOrDay(combination.observationTime)}-{Utils.MinuteToHrOrDay(combination.tradingInterval)}-{combination.movingAverageSize}";
                ThreeMarketsCombinationModels result = RunAllDatasetParams<AutoSellAverageCloseChangeRecordModel>(dataProvider, strategy, environmentParams, combinationName, savePath).Result;
                results.Add(result);
                counter++;
                Console.WriteLine(counter);
            });

            var resultsArray = results.ToArray();
            await new CsvExporter().Export(Path.Combine(savePath, "CombinationResult.csv"), resultsArray);
        }

        public static (int observationTime, int tradingInterval) RunFindAutoParamsCloseChangeBestParams(ThreeMarketsDataProvider dataProvider, EnvironmentParams environmentParams)
        {
            int[] observationTimes = new int[] { 3, 5, 15, 30, 60, 120, 240, 360, 480, 720, 1440, 4320, 10080, 20160, 30240, 40320 };
            int[] tradingIntervals = new int[] { 1, 3, 5, 15, 30, 60, 120, 240, 360, 480, 720, 1440 };
            List<(int observationTime, int tradingInterval)> combinations = new();
            ConcurrentBag<ThreeMarketsCombinationModels> results = new();
            foreach (int observationTime in observationTimes)
                foreach (int tradingInterval in tradingIntervals)
                    if (dataProvider.Index > CalculateDataCount(observationTime))
                        combinations.Add((observationTime, tradingInterval));

            if (!combinations.Any())
                return (10080, 720);

            Parallel.ForEach(combinations, new ParallelOptions { MaxDegreeOfParallelism = 6 }, (combination) =>
            {
                CloseChange strategy = new(combination.observationTime, combination.tradingInterval);
                string combinationName = $"{Utils.MinuteToHrOrDay(combination.observationTime)}-{Utils.MinuteToHrOrDay(combination.tradingInterval)}";
                ThreeMarketsCombinationModels result = RunPartDatasetParams<CloseChangeRecordModel>(dataProvider, strategy, environmentParams).Result;
                
                results.Add(result);
            });

            ThreeMarketsCombinationModels bestParms = results.AsQueryable().OrderByDescending(item => item.Assets).First();
            return (bestParms.ObservationTime, bestParms.TradingInterval);
        }

        /// <summary>
        /// 開始回測
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="dataProvider"> 回測資料 </param>
        /// <param name="strategy"> 策略 </param>
        /// <param name="environmentParams"> 回測環境的參數 </param>
        /// <param name="combination"> 回測紀錄名稱 </param>
        /// <param name="savePath"> 存檔位置 </param>
        /// <returns></returns>
        private static Task<ThreeMarketsCombinationModels> RunPartDatasetParams<V>(ThreeMarketsDataProvider dataProvider, Strategy strategy, EnvironmentParams environmentParams)
            where V : class, IEnvironmentModels, IStrategyModels, new()
        {
            int dataCount = CalculateDataCount(strategy.ObservationTime);
            Trace.Assert(dataProvider.Index > dataCount);
            ThreeMarketsDataProvider newDataProvider = dataProvider.Clone(dataProvider.Index - dataCount, dataCount);
            return RunParams<V>(newDataProvider, strategy, environmentParams, string.Empty, string.Empty);
        }

        private static int CalculateDataCount(int observationTime) => observationTime + (31 * 1440);

        /// <summary>
        /// 開始回測
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="dataProvider"> 回測資料 </param>
        /// <param name="strategy"> 策略 </param>
        /// <param name="environmentParams"> 回測環境的參數 </param>
        /// <param name="combination"> 回測紀錄名稱 </param>
        /// <param name="savePath"> 存檔位置 </param>
        /// <returns></returns>
        private static Task<ThreeMarketsCombinationModels> RunAllDatasetParams<V>(ThreeMarketsDataProvider dataProvider, Strategy strategy, EnvironmentParams environmentParams, string combination, string savePath)
            where V : class, IEnvironmentModels, IStrategyModels, new()
        {
            ThreeMarketsDataProvider newDataProvider = dataProvider.Clone();
            return RunParams<V>(newDataProvider, strategy, environmentParams, combination, savePath);
        }

        /// <summary>
        /// 開始回測
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="dataProvider"> 回測資料 </param>
        /// <param name="strategy"> 策略 </param>
        /// <param name="environmentParams"> 回測環境的參數 </param>
        /// <param name="combination"> 回測紀錄名稱 </param>
        /// <param name="savePath"> 存檔位置 </param>
        /// <returns></returns>
        private static Task<ThreeMarketsCombinationModels> RunAllDatasetParams<V>(ThreeMarketsDataProvider dataProvider, AutoSellCloseChangeStrategy strategy, EnvironmentParams environmentParams, string combination, string savePath)
            where V : class, IEnvironmentModels, IStrategyModels, new()
        {
            ThreeMarketsDataProvider newDataProvider = dataProvider.Clone();
            return RunParams<V>(newDataProvider, strategy, environmentParams, combination, savePath);
        }

        /// <summary>
        /// 開始回測
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="dataProvider"> 回測資料 </param>
        /// <param name="strategy"> 策略 </param>
        /// <param name="environmentParams"> 回測環境的參數 </param>
        /// <param name="combination"> 回測紀錄名稱 </param>
        /// <param name="savePath"> 存檔位置 </param>
        /// <returns></returns>
        private static async Task<ThreeMarketsCombinationModels> RunParams<V>(ThreeMarketsDataProvider dataProvider, Strategy strategy, EnvironmentParams environmentParams, string combination, string savePath)
            where V : class, IEnvironmentModels, IStrategyModels, new()
        {
            SpotEnvironment env = new(dataProvider, environmentParams);
            Runner<Strategy, V> runner;
            if (combination != string.Empty || savePath != string.Empty)
                runner = new(strategy, env, new(combination, savePath));
            else
                runner = new(strategy, env, null);
            await runner.RunAsync();
            return new() { Combination = combination, Assets = env.Assets, Balance = env.Balance, CoinBalance1 = env.Coin1Balance, CoinBalance2 = env.Coin2Balance, EndDate = env.CurrentKline.Coin22Coin1Kline.Date, ObservationTime = strategy.ObservationTime, TradingInterval = strategy.TradingInterval };
        }

        /// <summary>
        /// 開始回測
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="dataProvider"> 回測資料 </param>
        /// <param name="strategy"> 策略 </param>
        /// <param name="environmentParams"> 回測環境的參數 </param>
        /// <param name="combination"> 回測紀錄名稱 </param>
        /// <param name="savePath"> 存檔位置 </param>
        /// <returns></returns>
        private static async Task<ThreeMarketsCombinationModels> RunParams<V>(ThreeMarketsDataProvider dataProvider, AutoSellCloseChangeStrategy strategy, EnvironmentParams environmentParams, string combination, string savePath)
            where V : class, IEnvironmentModels, IStrategyModels, new()
        {
            SpotEnvironment env = new(dataProvider, environmentParams);
            AutoSellCloseChangeRunner<AutoSellCloseChangeStrategy, V> runner;
            if (combination != string.Empty || savePath != string.Empty)
                runner = new(strategy, env, new(combination, savePath));
            else
                runner = new(strategy, env, null);
            await runner.RunAsync();
            return new() { Combination = combination, Assets = env.Assets, Balance = env.Balance, CoinBalance1 = env.Coin1Balance, CoinBalance2 = env.Coin2Balance, EndDate = env.CurrentKline.Coin22Coin1Kline.Date, ObservationTime = strategy.ObservationTime, TradingInterval = strategy.TradingInterval };
        }
    }
}
