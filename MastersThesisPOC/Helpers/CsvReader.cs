using CsvHelper;
using System.Collections.Generic;
using System.Globalization;
using System.IO;


namespace MastersThesisPOC.Helpers
{
    public interface ICsvReader
    {
        List<float> ReadCsvColumn(string filePath);
        List<float> ReadCsvColumnHumidity(string filePath);
        List<float> ReadCsvColumnVoltage(string filePath);
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

        public List<float> ReadCsvColumnHumidity(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvHelper.CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<CsvData>().ToList();
                return records.Select(r => r.humidityavg).ToList();
            }
        }

        public List<float> ReadCsvColumnVoltage(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            {
                var config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ";",
                    MissingFieldFound = null, // Add this line to ignore missing fields
                    ReadingExceptionOccurred = context =>
                    {
                        // Check if the exception is a TypeConverterException
                        if (context.Exception is CsvHelper.TypeConversion.TypeConverterException)
                        {
                            return false; // Return false to ignore the record and continue reading.
                        }
                        return true; // Return true for other exceptions to throw them.
                    }
                };
                using (var csv = new CsvHelper.CsvReader(reader, config))
                {
                    var records = csv.GetRecords<CsvHouseHoldData>().ToList();
                    return records.Select(r => r.Voltage).ToList();
                }
            }
        }
    }
}
