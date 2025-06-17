using Magicodes.ExporterAndImporter.Core;
using Magicodes.ExporterAndImporter.Csv;
using MoreLinq.Extensions;
using QuantitativeTrading.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace QuantitativeTrading.Data.DataLoaders
{
    public class KlineDataLoader
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static async Task<KlineModel[]> LoadCSVAsync(string path, DateTime startTime, DateTime endTime)
            => (await LoadAsync(path)).Where(item => item.Date >= startTime && item.Date <= endTime).OrderBy(item => item.Date).ToArray();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static async Task<KlineModel[]> LoadCSVAsync(string path, DateTime startTime)
            => (await LoadAsync(path)).Where(item => item.Date >= startTime).OrderBy(item => item.Date).ToArray();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static async Task<KlineModel[]> LoadCSVAsync(string path)
            => (await LoadAsync(path)).OrderBy(item => item.Date).ToArray();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static async Task<IEnumerable<KlineModel>> LoadAsync(string path)
        {
            IImporter importer = new CsvImporter();
            var result = await importer.Import<KlineModel>(path);
            if (result.HasError)
                throw result.Exception;

            return result.Data.DistinctBy(item => item.Date);
        }
    }
}
