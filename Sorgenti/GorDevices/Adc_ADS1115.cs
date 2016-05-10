using Raspberry.IO;
using Raspberry.IO.Components.Converters.Mcp3208;
using Raspberry.IO.GeneralPurpose;
using Raspberry.IO.InterIntegratedCircuit;
using Raspberry.IO.SerialPeripheralInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gor.Devices
{
    public class Adc_ADS1115 : IDisposable
    {
        Mcp3208SpiConnection adcConnection;
        public string Read(int channel)
        {
            

            string output = "";
            // Start the child process.
            using (var p = new System.Diagnostics.Process())
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

            return ((int)adcConnection.Read((Mcp3208Channel)0).Value).ToString();




        }

        public void Dispose()
        {
            Close();
        }

        public void Close()
        {
            adcConnection.Close();
        }
    }





}