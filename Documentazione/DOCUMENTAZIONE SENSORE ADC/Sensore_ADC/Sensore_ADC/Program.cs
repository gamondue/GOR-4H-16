using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Diagnostics;

namespace Sensore_ADC
{
    class Program
    {
        
        static private string metodo()
        {
            string output = "";
            // Start the child process.
            using (var p = new Process())
            {
                // Redirect the output stream of the child process.
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                //p.StartInfo.FileName = "sudo python /home/pi/Baruzzi_ADC/Adafruit_Python_ADS1x15-master/Adafruit_ADS1x15/prova_ADC.py";
                p.StartInfo.FileName = "python";
                p.StartInfo.Arguments = "/home/pi/Baruzzi_ADC/Adafruit_Python_ADS1x15-master/Adafruit_ADS1x15/prova_ADC.py";
                p.Start();
                // Do not wait for the child process to exit before
                // reading to the end of its redirected stream.
                // p.WaitForExit();
                // Read the output stream first and then wait.
                output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
            }

            //Console.WriteLine("[DEBUG] 'uname -a' => " + output);
            return output;
           // Console.WriteLine(output);
        }
        static void Main(string[] args)
        {
            metodo();
        }
        
    }
}
