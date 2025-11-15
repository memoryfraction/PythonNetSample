using Python.Runtime;
using PythonnetSample.Bll; // 👈 必须引用 BLL 命名空间才能访问 PythonInitializer

namespace PythonnetSample.ConsoleApp
{
    // 修复 CS5001 错误的关键：提供包含 Main 方法的 Program 类
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("C# Main Application Started.");

            // --- 1. 初始化 Python 引擎 ---
            try
            {
                // 调用包含清除 PYTHONTZPATH 和路径配置的初始化方法
                PythonInitializer.EnsureInitialized();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FATAL] Python initialization failed: {ex.Message}");
                return;
            }

            // --- 2. 应用程序执行流 ---
            try
            {
                // 获取 GIL 以执行 Python 代码
                using (Py.GIL())
                {
                    // Sample 1: Yfinance Data Fetch 
                    Console.WriteLine("\n--- Running Sample 1: Yfinance Data Fetch ---");
                    dynamic data = YfinanceSample();
                    Console.WriteLine(data);

                    // Sample 2: Python Script Execution
                    Console.WriteLine("\n--- Running Sample 2: Python Script Execution ---");

                    // 确保 Python 能够找到自定义脚本
                    string scriptDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
                    PythonEngine.Exec($"import sys; sys.path.append(r'{scriptDirectory}')");

                    RunScript("MySamplePython", "say_hello", "Tom");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] An error occurred during Python execution: {ex.Message}");
            }
            finally
            {
                // 确保在程序退出前安全地关闭 Python 引擎
                PythonInitializer.Shutdown();
            }

            Console.WriteLine("\nC# Main Application Finished.");
        }

        // --- 辅助方法 (从您的原始代码中恢复) ---

        static void RunScript(string scriptFileNameWithoutExtension, string methodName, string parameter)
        {
            var pythonScript = Py.Import(scriptFileNameWithoutExtension);
            var method = pythonScript.GetAttr(methodName);
            var response = method.Invoke(new PyObject[] { new PyString(parameter) });
            Console.WriteLine(response);
        }

        static dynamic YfinanceSample()
        {
            dynamic yf = Py.Import("yfinance");
            dynamic ticker = yf.Ticker("AAPL");
            dynamic data = ticker.history(period: "1y");
            return data;
        }
    }
}