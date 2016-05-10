//nome programma:Light_Test
//autore:Gozzi Enrico/Ciro Romanino
//obbiettivo: riuscire a campionare una luminosità tramite un sensore


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
            Adc_MCP3208 converter = new Adc_MCP3208();                     //utilizzo Adc_MCP3208
            Logger l = new Logger();
            Light_PhotoResistor sensore = new Light_PhotoResistor("Luciometro", false, converter, 1, l);
            
            DateTime data = DateTime.Now;                                 //data attuale
            
             while (true)
            {
                System.Threading.Thread.Sleep(1000);                     //tempo di attesa prima di visualizzare il valore campionato
                Measurement m = sensore.Measure()[0];                    //valore campionato
                Console.WriteLine(m.ToString());                         //scrittura del valore
            }
            
        }

    }
}