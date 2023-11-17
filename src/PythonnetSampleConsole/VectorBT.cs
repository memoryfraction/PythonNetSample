using CsvHelper;
using Python.Runtime;
using System.Globalization;

namespace PythonnetSampleConsoleApp
{
    public class VectorBT
    {
        //private static void Generate
        static List<double> _prices;
        static List<bool> _entries;
        static List<double> _sizes;
        static bool _accumulate = true;
        static double _init_cash = 60000;
        static double _fee = 0.001;


        private static void AssignParams()
        {
            _prices = new List<double>();
            _entries = new List<bool>();
            _sizes = new List<double>();

            var ohlcvList = new List<RawBianceOHLC>();
            var fullPathFile = AppDomain.CurrentDomain.BaseDirectory + @"data\Biance_BTC_From20180101_To20230101_1d.csv";
            if (File.Exists(fullPathFile))
                ohlcvList = ReadSrcPrice_Csv(fullPathFile).ToList();
            foreach (var ohlcv in ohlcvList)
            {
                var price = ohlcv.closePrice;
                var day = ohlcv.datetime.Day;
                var entry = false;
                var size = 0.0;
                if (day == 16)
                {
                    entry = true;
                    size = 1000.0 / price;
                }
                _prices.Add(price);
                _entries.Add(entry);
                _sizes.Add(size);
            }

        }


        public static void VectorBTMethod()  
        {
            // 产生vectorBT所需的数据
            AssignParams();

            // 调用VectorBT，生成回测结果
            PythonEngine.Initialize();
            using (Py.GIL())
            {
                dynamic vbt = Py.Import("vectorbt");
                // size: array_like, 具体: $1000/当前价格;
                // 初始资金不足会导致订单减少
                var pf = vbt.Portfolio.from_signals(
                    close: _prices,
                    size : _sizes,  
                    entries : _entries,
                    init_cash : _init_cash, 
                    fees : _fee,
                    accumulate : _accumulate
                );

                Console.Write(pf.stats()); // 查看回测结果指标，比如：胜率，总回报等
                Console.Write(pf.orders.records_readable); // 查看订单明细
                pf.plot().show(); // 绘图 订单 叠加 标的物价格

                Console.ReadKey();
            }
        }


        private static IEnumerable<RawBianceOHLC> ReadSrcPrice_Csv(string fullPathFile)
        {
            var list = new List<RawBianceOHLC>();
            if (File.Exists(fullPathFile))
            {
                using (var reader = new StreamReader(fullPathFile))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var ohlcList = csv.GetRecords<RawBianceOHLC>();
                    foreach (var ohlcItem in ohlcList)
                    {
                        list.Add(ohlcItem);
                    }
                }
            }
            list = list.OrderBy(x => x.datetime).ToList();
            return list;
        }
    }
}
