using Gor;
using Gor.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

//Gozzi-Romanino 4°H
namespace Light_Test
{
    public class Program
    {
        static void Main(string[] args)
        {
            Adc_MCP3208 converter = new Adc_MCP3208();
            Logger l = new Logger();
            Light_PhotoResistor sensore = new Light_PhotoResistor("Luciometro", false, converter, 1, l);
            //int lux = 12345;
            DateTime data = DateTime.Now;
            
             while (true)
            {
                System.Threading.Thread.Sleep(1000);
                Measurement m = sensore.Measure()[0];
                Console.WriteLine(m.ToString());
            }
            
        }

    }
}