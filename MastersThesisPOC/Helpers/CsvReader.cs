using CsvHelper;
using System.Collections.Generic;
using System.Globalization;
using System.IO;


namespace MastersThesisPOC.Helpers
{
    public interface ICsvReader
    {
        public List<float> ReadCsvColumn(string filePath);
    }

    public class CsvReader : ICsvReader
    {
        public List<float> ReadCsvColumn(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvHelper.CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<CsvData>().ToList();
                return records.Select(r => r.tempavg).ToList();
            }
        }
    }
}
