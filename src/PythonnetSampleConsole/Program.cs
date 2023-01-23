using Python.Runtime;

namespace PythonnetSampleConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string dllPath = @"C:\Program Files\Python311\python311.dll"; // 此处需要对应Python的安装路径
            string pythonHomePath = @"C:\Program Files\Python311";

            // 对应Python内的重要路径
            string[] py_paths = {"DLLs", "lib", "lib\\site-packages", "lib\\site-packages\\win32"
                , "lib\\site-packages\\win32\\lib", "lib\\site-packages\\Pythonwin" };
            string pySearchPath = $"{pythonHomePath};";
            foreach (string p in py_paths)
            {
                var tmpPath = Path.Combine(pythonHomePath, p);
                pySearchPath += $"{tmpPath};";
            }

            // 此处解决BadPythonDllException报错
            Runtime.PythonDLL = dllPath;
            Environment.SetEnvironmentVariable("PYTHONNET_PYDLL", dllPath);

            // 配置python环境搜索路径解决 PythonEngine.Initialize() 崩溃
            PythonEngine.PythonHome = pythonHomePath;
            PythonEngine.PythonPath = pySearchPath;

            PythonEngine.Initialize();
            using (Py.GIL())
            {
                dynamic np = Py.Import("numpy");
                Console.WriteLine(np.cos(np.pi * 2));

                dynamic sin = np.sin;
                Console.WriteLine(sin(5));

                double c = np.cos(5) + sin(5);
                Console.WriteLine(c);

                dynamic a = np.array(new List<float> { 1, 2, 3 });
                Console.WriteLine(a.dtype);

                dynamic b = np.array(new List<float> { 6, 5, 4 }, dtype: np.int32);
                Console.WriteLine(b.dtype);

                Console.WriteLine(a * b);
                Console.ReadKey();
            }
        }
    }
}