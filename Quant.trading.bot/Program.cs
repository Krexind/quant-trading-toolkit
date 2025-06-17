using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QuantitativeTrading.Data.DataLoaders;
using QuantitativeTrading.Data.DataProviders;
using QuantitativeTrading.Environments;
using QuantitativeTrading.Models;
using QuantitativeTrading.Runners.ThreeMarkets;
using QuantitativeTrading.Services.ThreeMarkets;

namespace QuantitativeTrading
{
    class Program
    {
        private const string datasetPath = @"C:\Users\Kenneth\OneDrive - 臺北科技大學 軟體工程實驗室\量化交易\General\原始資料集";
        private const string savePath = @"C:\Users\Kenneth\新北市南山高中\Tang - Quantitative-Backtest - Quantitative-Backtest\CloseChangeAllParams-20210101";

        static async Task Main(string[] args)
        {
            int[] observationTimes = new[] { 3, 5, 15, 30, 60, 120, 240, 360, 480, 720, 1440, 4320, 10080, 20160, 30240, 40320 };
            int[] tradingIntervals = new[] { 1, 3, 5, 15, 30, 60, 120, 240, 360, 480, 720, 1440 };
            int[] movingAverageSizes = new[] { 15, 30, 60, 120, 240, 360, 480, 720, 1440, 4320, 10080, 20160, 30240, 40320 };
            ThreeMarketsDatasetModel dataset = await ThreeMarketsDataLoader.LoadCsvDataAsync(Path.Combine(datasetPath, "BTCUSDT-Spot.csv"), Path.Combine(datasetPath, "ETHUSDT-Spot.csv"), Path.Combine(datasetPath, "ETHBTC-Spot.csv"));
            ThreeMarketsDataProvider dataProvider = new(dataset);
            EnvironmentParams environmentParams = new(20000, 10000, 0.1m, 3);

            await RunAllParams.RunAutoSellAverageCloseChangeAllParams(dataProvider, environmentParams, observationTimes, tradingIntervals, movingAverageSizes, savePath);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
                Host.CreateDefaultBuilder(args)
                    .ConfigureHostConfiguration(configHost =>
                    {
                        configHost.AddEnvironmentVariables();
                        configHost.AddCommandLine(args);
                    })
                    .ConfigureServices((hostContext, services) =>
                    {
                        services.AddHostedService<BinanceService>();
                    });
    }
}
