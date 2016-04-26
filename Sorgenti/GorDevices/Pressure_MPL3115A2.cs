using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Gor.Devices
{
    class Pressure_MPL3115A2 : Sensor
    {
        Process p;
        public Pressure_MPL3115A2(string name, string externalCommand, Logger logger ) : base(name, false, logger)
        {
            p = new Process();
            p.StartInfo.FileName = externalCommand;
            p.StartInfo.Arguments = "";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            
        }
        public string Read()
        {
            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            return output;
        }
    }
}
