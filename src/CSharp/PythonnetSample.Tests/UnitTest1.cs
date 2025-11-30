using Python.Runtime;
using PythonnetSample.Bll;

namespace PythonnetSample.Tests
{
    [TestClass]
    public class UnitTest1
    {
        // ... (AssemblyInitialize 和 AssemblyCleanup 保持不变) ...
        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            Console.WriteLine("--- [AssemblyInitialize] Initializing Python Engine ---");
            PythonInitializer.EnsureInitialized();
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            Console.WriteLine("--- [AssemblyCleanup] Shutting Down Python Engine ---");
            PythonInitializer.Shutdown();
        }


        [TestMethod]
        public void Test_Python_Yfinance_Data_Fetch()
        {
            // 在测试方法内部获取 GIL
            using (Py.GIL())
            {
                // 调用 YfinanceSample 逻辑
                dynamic data = YfinanceSample();

                // 断言 1: 确保返回的数据不是空对象
                Assert.IsNotNull(data, "Yfinance should return a data object.");

                // --- 最终修复部分：显式调用 Python 的 __contains__ 魔术方法 ---

                // 获取列集合对象
                dynamic columns = data.columns;

                // 使用 __contains__ 方法检查 "Close" 列是否存在。
                // 这是最可靠的跨平台/版本检查 Pandas Index 成员的方法。
                bool containsClose = (bool)columns.__contains__("Close");

                Assert.IsTrue(containsClose, "The yfinance data frame must contain the 'Close' column.");

                // 断言 3: 确保数据行数大于 0 (如果数据源可用)
                int rowCount = (int)data.__len__();
                Assert.IsTrue(rowCount > 0, $"Expected more than 0 rows, but got {rowCount} rows.");

                Console.WriteLine($"Test_Python_Yfinance_Data_Fetch successful. Rows retrieved: {rowCount}");
            }
        }

        // ... (其他方法保持不变) ...
        private dynamic YfinanceSample()
        {
            dynamic yf = Py.Import("yfinance");
            dynamic ticker = yf.Ticker("AAPL");
            dynamic data = ticker.history(period: "1d");
            return data;
        }

        private string RunScript(string scriptFileNameWithoutExtension, string methodName, string parameter)
        {
            var pythonScript = Py.Import(scriptFileNameWithoutExtension);
            var method = pythonScript.GetAttr(methodName);
            var response = method.Invoke(new PyObject[] { new PyString(parameter) });
            return response.ToString();
        }
    }
}