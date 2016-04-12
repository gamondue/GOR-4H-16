using Raspberry.IO.GeneralPurpose;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Test.Gpio.DigitalInput
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectorPin connPin1 = ConnectorPin.P1Pin40;
            ProcessorPin procPin1 = connPin1.ToProcessor();

            ConnectorPin connPin2 = ConnectorPin.P1Pin38;
            ProcessorPin procPin2 = connPin2.ToProcessor();

            var driver = GpioConnectionSettings.DefaultDriver;

            Console.WriteLine("Digital Input Sample: just a button");
            Console.WriteLine();

            Console.WriteLine("\tWatching Processor Pin: {0}, {2}, Connector pin: {1}, {3}", 
                procPin1, procPin2, connPin1, connPin2);
            Console.WriteLine();

            Console.WriteLine("Press CTRL-C to stop");

            try
            {
                driver.Allocate(procPin1, PinDirection.Input);
                driver.Allocate(procPin2, PinDirection.Input);

                bool isHigh, isHigh2;
                //Esempio di lettura del valore ed attesa di un po' di tempo, prima di leggere di nuovo 
                while (true)
                {
                    DateTime now = DateTime.Now;
                    isHigh = driver.Read(procPin1);
                    isHigh2 = driver.Read(procPin2);
                    Console.WriteLine(now + "." + now.Millisecond.ToString("000") + ": Pin " + procPin1 + " " + (isHigh ? "HIGH" : "LOW"));
                    Console.WriteLine("                         Pin " + procPin2 + " " + (isHigh2 ? "HIGH" : "LOW"));
                    // attende un po' 
                    Thread.Sleep(500);
                }

                // Esempio di attesa fino a che non cambia qualcosa nel pin 
                isHigh = driver.Read(procPin1);
                while (true)
                {
                    DateTime now = DateTime.Now;
                    Console.WriteLine(now + "." + now.Millisecond.ToString("000") + ": " + (isHigh ? "HIGH" : "LOW"));
                    // esempio di codice che attende che il valore cambi, per poi 
                    // attendere dopo la lettura fino a che non assume il livello negato (fino a che non cambia)
                    driver.Wait(procPin1, !isHigh, TimeSpan.FromDays(7)); //TODO: infinite
                    // cambio lo stato della booleana per aspettare la prossima transizione sul fronte basso
                    isHigh = !isHigh;
                }

            }
            finally
            {
                // Leaves the pin unreleased so that other processes can keep reading
                //driver.Release(procPin);
            }
        }
    }
}
