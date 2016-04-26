﻿using Gor;
using Gor.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
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
            //int lux = 12345;
            DateTime data = DateTime.Now;

            Measurement m = sensore.Measure()[0];
           
            System.IO.StreamWriter file = new System.IO.StreamWriter(@"test.txt", true);

            string riga =Convert.ToString(m.Value);
           
            //string temp;
            // do
            /*{
                temp = Console.ReadLine();
                if (temp != "")
                    file.WriteLine(riga  + "\t" + "Campionatura:" + "\t" + temp );
                // file.WriteLine(temp);
            }*/
            // while (temp != "");
             //file.WriteLine(temp + riga );
             
            string b =args[0];
   
            switch(b)
            {
                    
                case "s":
                    File.Delete(@"test.txt");
                    file = new System.IO.StreamWriter(@"test.txt", true);
                    break;
                case "p":
                    Console.WriteLine(m.Value);
                    file.WriteLine("Lettura:"+ "\t" + riga + "\t" + "Campionatura:" + "\t" + args[1]);
                  
                    break;
                case "t":
                    Console.WriteLine("coming soon!!!!");
                    break;
            }
            file.Close();
        }

    }
}