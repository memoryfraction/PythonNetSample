namespace PythonnetSampleConsoleApp
{
    public class RawBianceOHLC
    {
        public string timestamp { get; set; }
        public DateTime datetime { get; set; }
        public double openPrice { get; set; }
        public double highPrice { get; set; }
        public double lowPrice { get; set; }
        public double closePrice { get; set; }
        public double amount { get; set; }
        public double volume { get; set; }
        public double vwap { get; set; }
        public double count { get; set; }
    }
}
