using Magicodes.ExporterAndImporter.Core;
using System;

namespace QuantitativeTrading.Models
{
    public class KlineModel
    {
        [ExporterHeader(DisplayName = "stock_code")]
        public string StockCode { get; set; }

        [ExporterHeader(DisplayName = "date", Format = "yyyy-MM-dd HH:mm:ss")]
        public DateTime Date { get; set; }

        [ExporterHeader(DisplayName = "open")]
        public decimal Open { get; set; }

        [ExporterHeader(DisplayName = "high")]
        public decimal High { get; set; }

        [ExporterHeader(DisplayName = "low")]
        public decimal Low { get; set; }

        [ExporterHeader(DisplayName = "close")]
        public decimal Close { get; set; }

        [ExporterHeader(DisplayName = "volume")]
        public decimal Volume { get; set; }

        [ExporterHeader(DisplayName = "money")]
        public decimal Money { get; set; }

        [ExporterHeader(DisplayName = "factor")]
        public decimal Factor { get; set; }

        [ExporterHeader(DisplayName = "change")]
        public decimal Change { get; set; }

        [ExporterHeader(DisplayName = "TradeCount")]
        public int TradeCount { get; set; }

        [ExporterHeader(DisplayName = "TakerBuyBaseVolume")]
        public decimal TakerBuyBaseVolume { get; set; }

        [ExporterHeader(DisplayName = "TakerBuyQuoteVolume")]
        public decimal TakerBuyQuoteVolume { get; set; }
    }
}
