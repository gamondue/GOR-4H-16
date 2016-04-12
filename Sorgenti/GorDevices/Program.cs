using Gor;
using Gor.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//Gozzi-Romanino 4°H
namespace Light_Calibration
{
    public class Program
    {
        static void Main(string[] args)
        {
            Adc_MCP3208 converter = new Adc_MCP3208();
            Logger l = new Logger();
            Light_PhotoResistor sensore = new Light_PhotoResistor("Luciometro", false, converter, 1, l);

            DateTime data = DateTime.Now;

            Measurement m = sensore.Measure()[0];
            string riga = (data + "\t" + "Letto: Lux" + "\t" + m.Value);
            System.IO.StreamWriter file = new System.IO.StreamWriter(@"test.txt", true);


            Console.WriteLine(riga);
            string temp;
            // do
            {
                temp = Console.ReadLine();
                if (temp != "")
                    file.WriteLine(riga + "\t" + "Campionatura: Lux" + "\t" + temp);
                // file.WriteLine(temp);
            }
            // while (temp != "");
            // file.WriteLine(temp + riga );
            file.Close();
        }

    }
}