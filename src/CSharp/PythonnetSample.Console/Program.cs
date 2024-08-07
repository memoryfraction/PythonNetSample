using Python.Runtime;
using System.Reflection.Metadata;



namespace PythonnetSampleConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // 初始化变量
            var condaVenvHomePath =  @"D:\ProgramData\PythonVirtualEnvs\pair_trading";
            var infra = PythonNetInfra.GetPythonInfra(condaVenvHomePath,  "python39.dll");
            Runtime.PythonDLL = infra.PythonDLL;
            PythonEngine.PythonHome = infra.PythonHome;
            PythonEngine.PythonPath = infra.PythonPath;
            
            PythonEngine.Initialize();// 初始化Python引擎
            // 使用Python GIL
            using (Py.GIL())
            {
                // sample1 获取数据，并打印; 期望：打印获取的金融数据;
                dynamic data = YfinanceSample();
                // 打印股票数据
                Console.WriteLine(data);

                // sample2， 期望：打印：hello world
                RunScript("MySamplePython", "say_hello", "Tom");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scriptFileName">scriptFileName 需要copy Always</param>
        /// <param name="methodName"></param>
        static void RunScript(string scriptFileName, string methodName, string parameter)
        {
            var pythonScript = Py.Import(scriptFileName);
            var response = pythonScript.InvokeMethod(methodName, new PyObject[] { new PyString(parameter) });
            Console.WriteLine(response);
        }


        /// <summary>
        /// 获取数据，并返回
        /// </summary>
        /// <returns></returns>
        static dynamic YfinanceSample()
        {
            // 导入yfinance并获取股票数据
            dynamic yf = Py.Import("yfinance");
            dynamic ticker = yf.Ticker("AAPL");
            dynamic data = ticker.history(period: "1y");
            return data;
        }
    }
}