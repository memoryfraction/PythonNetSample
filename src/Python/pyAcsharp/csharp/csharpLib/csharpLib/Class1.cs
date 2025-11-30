using System;
using System.Linq;


namespace csharpLib
{
    // Python 端可实现此接口做回调
    //public interface ICalc
    //{
    //    double Add(double a, double b);
        
    //}

    public class Calc// : ICalc
    {
        public double Add(double a, double b) => a + b;

    }
}
