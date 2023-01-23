# PythonNetSampleApp
# 摘要

C#/.Net有其语言方面的优势，Python在生态方面则完胜，尤其大数据分析等数据结构和框架，比如: DataFrame， Numpy等。

如果能将两者优点结合，有利于对于软件的开发工作。本文通过使用PythonNet这个框架，在C#控制台项目中调用Python包。提供使用步骤，常见问题+解决方案和参考资料等。


## 量化痛点问题 - 使用场景举例

.NET 有不完善的DataFrame，也不存在Numpy这种成熟数据分析框架; 换句话说：量化分析必须使用Python。

但是希望使用.NET，比如: ASP.NET CORE + IIS作为主要框架和生产环境，方便单元测试，项目管理，部署，IOC等各项操作;

因此，瓶颈: 能否在C#项目中调用Python？

![image](resources/images/pic1.png?raw=true)

对于上图，举例说明方案B的优势。

在发生未经处理异常的场景下， 由于存在IIS层，比如：调用交易所API超时。 WebServer(IIS)会日志异常内容，并重新启动程序。

而方案A则会导致程序在操作系统中崩溃，直到手动重启项目。 

很明显，方案A有可能引发不可挽回的损失。


# 0 Python环境搭建(非Anaconda)


## 0.1 [下载Python](https://www.python.org/downloads/)

根据自己的操作系统位数，下载对应版本。

作者使用WINDOW 64 Bit系统，因此下载64位exe版本。


## 0.2 安装Python

安装到路径: C:\Program Files\Python311

![image](resources/images/pic2.png?raw=true)

安装过程中注意添加Python 到 PATH


## 0.3 配置环境变量


### 0.3.1 环境变量配置方法1

CMD 输入


```
path=%path%;C:\Program Files\Python311\
```



### 0.3.2 环境变量配置方法2

等效方法如下;
![image](resources/images/pic3.png?raw=true)



## 0.4 常见Python环境变量
![image](resources/images/pic4.png?raw=true)

## 0.5 **验证PYTHON安装结果**

CMD


```
PYTHON
```


下图说明安装顺利
![image](resources/images/pic5.png?raw=true)


如果安装失败，说明需要把python.exe所在的路径手动添加到path中. 也可以删除python重新安装

[https://www.liaoxuefeng.com/wiki/0014316089557264a6b348958f449949df42a6d3a2e542c000/0014316090478912dab2a3a9e8f4ed49d28854b292f85bb000](https://www.liaoxuefeng.com/wiki/0014316089557264a6b348958f449949df42a6d3a2e542c000/0014316090478912dab2a3a9e8f4ed49d28854b292f85bb000)


# 1 Nuget 安装 pythonnet

0.1 使用VisualStudio新建Console项目，并使用Nuget Package Manager安装pythonnet包
![image](resources/images/pic6.png?raw=true)



# 2 Sample code


```
using Python.Runtime;
namespace ConsoleApp2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string dllPath = @"C:\Program Files\Python311\python311.dll"; // 此处需要对应Python的安装路径
            string pythonHomePath = @"C:\Program Files\Python311";
            // 对应Python内的重要路径
            string[] py_paths = {"DLLs", "lib", "lib\\site-packages",  "lib\\site-packages\\win32"
                , "lib\\site-packages\\win32\\lib",  "lib\\site-packages\\Pythonwin" };
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
                dynamic b = np.array(new List<float> { 6, 5, 4 }, dtype:  np.int32);
                Console.WriteLine(b.dtype);
                Console.WriteLine(a * b);
                Console.ReadKey();
            }
        }
    }
}
```



# 3 运行结果
![image](resources/images/pic7.png?raw=true)


出现上图，说明运行成功。


# QAs

**1 如果提示找不到Numpy？**

分析：numpy包未安装

解决方案: 

VS中 -> Package Manager Console


```
pip install numpy
```


安装后重新运行


# 参考资料

1 [C#调用pythonnet遇到的环境变量问题](https://zhuanlan.zhihu.com/p/594894616)

<span style="text-decoration:underline;">2 [利用Pythonnet，让C#调用Python（C# call Python by pythonnet）](https://www.bilibili.com/video/BV1ii4y147AX/?spm_id_from=333.337.search-card.all.click&vd_source=fcd76fcb58e4f91c2ab6d9bc6449675f)</span>

<span style="text-decoration:underline;">3 [Python for .Net文档](https://dev.listera.top/docs/pythonnet/pythonnet.github.io.html)</span>

4 Numpy的报错问题: [https://numpy.org/devdocs/user/troubleshooting-importerror.html](https://numpy.org/devdocs/user/troubleshooting-importerror.html)

<span style="text-decoration:underline;">5 [Python3 环境搭建- 菜鸟教程](https://www.runoob.com/python3/python3-install.html)</span>
