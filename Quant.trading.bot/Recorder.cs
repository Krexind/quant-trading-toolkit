using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Magicodes.ExporterAndImporter.Csv;

namespace QuantitativeTrading
{
    public class Recorder<T>
        where T : class, new()
    {
        private readonly List<T> records;
        private readonly string file;

        public Recorder(string fileName, string filePath)
        {
            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);
            (records, this.file) = (new(), Path.Combine(filePath, fileName + ".csv"));
        }

        public void Insert(T record)
            => records.Add(record);

        public async Task SaveAsync()
        {
            if (File.Exists(file))
                File.Delete(file);
            await new CsvExporter().Export(file, records);
        }
    }
}