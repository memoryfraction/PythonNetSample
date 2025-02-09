using Python.Runtime;
using System;

namespace PythonnetSampleConsoleApp
{
    internal class Program
    {
        private static bool pythonInitialized = false; // 确保 Python 运行时只初始化一次
        private static readonly object lockObj = new object(); // 线程安全锁

        static void Main(string[] args)
        {
            InitializePythonEngine(); // 确保 Python 运行时只初始化一次

            using (Py.GIL()) // 进入 Python 全局解释器锁，确保线程安全
            {
                // Sample1 获取数据，并打印; 期望：打印获取的金融数据;
                dynamic data = YfinanceSample();
                Console.WriteLine(data);

                // Sample2， 期望：打印：hello world
                // 添加Python脚本文件路径:"Data\\MySamplePython.py"
                string scriptDirectory = AppDomain.CurrentDomain.BaseDirectory + "data";
                PythonEngine.Exec($"import sys; sys.path.append(r'{scriptDirectory}')");

                RunScript("MySamplePython", "say_hello", "Tom");
            }
        }

        /// <summary>
        /// 初始化 Python 运行时，确保线程安全，防止多线程环境下重复初始化导致崩溃
        /// </summary>
        private static void InitializePythonEngine()
        {
            if (!pythonInitialized) // 先检查是否已初始化，减少不必要的锁竞争
            {
                lock (lockObj) // 确保多线程环境下不会重复初始化
                {
                    if (!pythonInitialized) // 双重检查锁，防止多个线程同时进入
                    {
                        var condaVenvHomePath = @"D:\\ProgramData\\PythonVirtualEnvs\\pair_trading";
                        var infra = PythonNetInfra.GetPythonInfra(condaVenvHomePath, "python39.dll");
                        Runtime.PythonDLL = infra.PythonDLL;
                        PythonEngine.PythonHome = infra.PythonHome;
                        PythonEngine.PythonPath = infra.PythonPath;

                        PythonEngine.Initialize(); // 只初始化一次

                        pythonInitialized = true; // 标记为已初始化，防止重复调用
                    }
                }
            }
        }

        /// <summary>
        /// 运行 Python 脚本
        /// </summary>
        /// <param name="scriptFileNameWithoutExtension">scriptFileNameWithoutExtension 需要设置为 "Copy Always"</param>
        /// <param name="methodName">Python 方法名</param>
        /// <param name="parameter">传递的参数</param>
        static void RunScript(string scriptFileNameWithoutExtension, string methodName, string parameter)
        {
            var pythonScript = Py.Import(scriptFileNameWithoutExtension);
            var response = pythonScript.InvokeMethod(methodName, new PyObject[] { new PyString(parameter) });
            Console.WriteLine(response);
        }

        /// <summary>
        /// 获取股票数据，并返回
        /// </summary>
        /// <returns>股票历史数据</returns>
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
