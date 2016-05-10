using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Gor;
namespace Gor.Devices
{
    public class Pressure_MPL3115A2 : Sensor
    {
        Process p;
        private string externalCommand;
        Measurement m;
        public Pressure_MPL3115A2(string name, string externalCommand, Logger logger)
            : base(name, false, logger)
        {
            this.externalCommand = externalCommand;
            m.Name = name;
            m.Unit = "Pa";
        }
        public override void Initialization()
        {
            p = new Process();
            p.StartInfo.FileName = externalCommand;
            p.StartInfo.Arguments = "";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
        }
        public override string Read()
        {
            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            this.m.Value = Convert.ToDouble(output);
            return output;
        }
        public override List<Measurement> Measure()
        {
            List<Measurement> temp = new List<Measurement>(1);
            temp[0] = m;
            return temp;
        }
    }
}
