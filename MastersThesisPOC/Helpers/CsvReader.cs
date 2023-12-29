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
        List<float> ReadCsvColumnGI(string filePath);
        List<float> ReadCsvColumnSubMetering3(string filePath);
        List<float> ReadCsvColumnGlobalActivePower(string filePath);
        List<TimeSpan> ReadCsvColumnTime(string filePath);
        List<float> ReadCsvColumPondTemp(string filePath);
        List<float> ReadCsvColumnHumidityReal(string filePath);
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

        public List<float> ReadCsvColumPondTemp(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            {
                var config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ",",
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
                    var records = csv.GetRecords<CsvPondData>().ToList();
                    return records.Select(r => r.Temperature).ToList();
                }
            }
        }

        public List<float> ReadCsvColumnHumidityReal(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            {
                var config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ",",
                    MissingFieldFound = null,
                    ReadingExceptionOccurred = context =>
                    {
                        if (context.Exception is CsvHelper.TypeConversion.TypeConverterException)
                        {
                            return false;
                        }
                        return true;
                    },
                    HasHeaderRecord = false, // Set this to false since your CSV file doesn't have headers
                };

                using (var csv = new CsvHelper.CsvReader(reader, config))
                {
                    var records = csv.GetRecords<CsvHumidityData>().ToList();
                    return records.Select(r => r.Humidity).ToList();
                }
            }
        }


        public List<TimeSpan> ReadCsvColumnTime(string filePath)
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
                    return records.Select(r => r.Time).ToList();
                }
            }
        }

        public List<float> ReadCsvColumnGI(string filePath)
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
                    return records.Select(r => r.Global_intensity).ToList();
                }
            }
        }

        public List<float> ReadCsvColumnSubMetering3(string filePath)
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
                    return records.Select(r => r.Sub_metering_3).ToList();
                }
            }
        }

        public List<float> ReadCsvColumnGlobalActivePower(string filePath)
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
                    return records.Select(r => r.Global_active_power).ToList();
                }
            }
        }
    }
}
