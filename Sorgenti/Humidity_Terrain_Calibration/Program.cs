using Gor;
using Gor.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


//Giorgi-Ricci
namespace Humidity_Terrain_Calibration
{
    public class Program
    {
        static void Main(string[] args)
        {
            Adc_MCP3208 converter = new Adc_MCP3208();
            Logger l = new Logger();
            Humidity_Terrain_YL69YL38 sensore = new Humidity_Terrain_YL69YL38("Umidometro", false, converter, 1, l);
            
            DateTime data = DateTime.Now;

            Measurement m = sensore.Measure()[0];
            string riga = (data + "\t" + "Letto: Umidità" + "\t" + m.Value);
            System.IO.StreamWriter file = new System.IO.StreamWriter(@"test.txt", true);

           // using (StreamWriter sw = File.AppendText(@""))
            {
               // string pino = sensore.Read().ToString() + "\t" + args[0];         // salva nel file di testo   , che sarà nel 
               // sw.WriteLine(riga);
            }

            Console.WriteLine(riga);
            string temp;
            // do
            {
                temp = Console.ReadLine();
                if (temp != "")
                    file.WriteLine(m.Value + "\t" + "Campionatura: Umidità" + "\t" + temp);
                // file.WriteLine(temp);
            }
            // while (temp != "");
            // file.WriteLine(temp + riga );
            file.Close();
            string b = Console.ReadLine();
            int a = int.Parse(b);
            switch(a)
            {
                case 1:
                    Console.WriteLine("Letto: %" + m.Value + "\t" + "Campionatura: Umidità" + "\t" + temp);
                    break;
                case 2:
                    Console.WriteLine("Letto: %" + "\t" + m.Value);
                    break;
                case 3:
                    Console.WriteLine("coming soon");
                    break;
            }
        }

    }
}