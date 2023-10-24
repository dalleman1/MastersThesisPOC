namespace MastersThesisPOC.Helpers
{
    public class CsvData
    {
        public float tempavg { get; set; }
        public float lightavg { get; set; }
        public float humidityavg { get; set; }
    }

    public class CsvHouseHoldData
    {
        public float Voltage { get; set; }
        public float Global_intensity { get; set; }
        public float Sub_metering_3 { get; set; }
        public float Global_active_power { get; set; }
    }
}
