using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gor.Devices;
using Gor;

namespace Test.Shell.Pressure
{
    class Program
    {
        static public Gor.Devices.Pressure_MPL3115A2 sensor;
        static public Logger l;

        static void Main(string[] args)
        {
            sensor = new Gor.Devices.Pressure_MPL3115A2("sensore su raspberry MPL", "MPL3115A2.py", l);
            Console.WriteLine(sensor.Read());

        }
    }
}