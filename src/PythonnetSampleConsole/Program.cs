using Python.Runtime;
using System;
using System.IO;


namespace PythonnetSampleConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // 配置并准备PythonEngine
            var pythonHomePath = @"C:\Users\Rex\.conda\envs\myenvpy39";//创建一个python39的虚拟环境，并指向其路径;
            if (!Directory.Exists(pythonHomePath))
                throw new DirectoryNotFoundException(pythonHomePath);
            var dllFilePath = $@"{pythonHomePath}\python39.dll"; // 请确保指向正确的 Python DLL 路径, pythonnet兼容python39
            if (!File.Exists(dllFilePath))
                throw new FileNotFoundException(dllFilePath);

            // 对应Python内的重要路径
            string[] pyPaths = { "DLLs", "Lib", "Lib\\site-packages", "Lib\\site-packages\\numpy" };
            string pySearchPath = $"{pythonHomePath};";
            foreach (var p in pyPaths)
            {
                var tmpPath = Path.Combine(pythonHomePath, p);
                pySearchPath += $"{tmpPath};";
            }

            // 此处解决BadPythonDllException报错
            Runtime.PythonDLL = dllFilePath;
            Environment.SetEnvironmentVariable("PYTHONNET_PYDLL", dllFilePath);

            // 配置python环境搜索路径解决 PythonEngine.Initialize() 崩溃
            PythonEngine.PythonHome = pythonHomePath;
            PythonEngine.PythonPath = pySearchPath;

            VectorBT.VectorBTMethod();

            Console.ReadKey();

        }
    }
}