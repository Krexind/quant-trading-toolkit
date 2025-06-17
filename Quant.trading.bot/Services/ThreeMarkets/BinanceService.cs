using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using QuantitativeTrading.Environments.ThreeMarkets;
using QuantitativeTrading.Models.Records.ThreeMarkets;
using QuantitativeTrading.Runners.ThreeMarkets;
using QuantitativeTrading.Strategies.ThreeMarkets;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace QuantitativeTrading.Services.ThreeMarkets
{
    public class BinanceService : IHostedService
    {
        private readonly IConfiguration configuration;
        private readonly ILogger logger;
        private BinanceRunner<CloseChange, CloseChangeRecordModel> runner;

        public BinanceService(IConfiguration configuration, ILogger<BinanceService> logger)
            => (this.configuration, this.logger) = (configuration, logger);

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            CloseChange closeChange = new(12 * 60, 12 * 60);
            BinanceSpot binanceSpot = new(configuration, "USDT", "BTC", "ETH", 12 * 60);
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Binance");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            Recorder<CloseChangeRecordModel> recorder = new("Binance", path);
            runner = new(logger, closeChange, binanceSpot, recorder);
            await runner.RunAsync();
            logger.LogInformation("Service is start.");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            runner.Dispose();
            logger.LogInformation("Service is stop.");
            return Task.CompletedTask;
        }
    }
}
