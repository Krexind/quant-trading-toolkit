using System;
using System.IO;

namespace QuantitativeTrading.Tests
{
    public static class Utils
    {
        public static readonly string TestFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFiles");
        public static readonly string btc_usdtPath = Path.Combine(Utils.TestFilePath, "BTCUSDT-Spot.csv");
        public static readonly string eth_usdtPath = Path.Combine(Utils.TestFilePath, "ETHUSDT-Spot.csv");
        public static readonly string eth_btcPath = Path.Combine(Utils.TestFilePath, "ETHBTC-Spot.csv");
    }
}
