using System.Globalization;

namespace MastersThesisPOC.Helpers
{
    public interface ICsvWriter
    {
        void WriteToCsv(string columnName, List<float> values, string filePath);
    }
    public class CsvWriter : ICsvWriter
    {
        public void WriteToCsv(string columnName, List<float> values, string filePath)
        {
            using (var writer = new StreamWriter(filePath))
            {
                writer.WriteLine(columnName); // Column name

                foreach (var value in values)
                {
                    writer.WriteLine(value.ToString(CultureInfo.InvariantCulture)); // Each row of data
                }
            }
        }
    }
}
