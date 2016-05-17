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

            ConnectorPin connPin1 = ConnectorPin.P1Pin18;
            ProcessorPin procPin1 = connPin1.ToProcessor();
            int count1 = 0;

            ConnectorPin connPin2 = ConnectorPin.P1Pin40;
            ProcessorPin procPin2 = connPin2.ToProcessor();
            int count2 = 0;

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

                bool isHigh1, isHigh2;
                bool pastStatus1 = driver.Read(procPin1);
                bool pastStatus2 = driver.Read(procPin2);

                //Esempio di lettura del valore e visualizzazione solo delle differenze
                while (true)
                {
                    DateTime now = DateTime.Now;

                    string tempo = Convert.ToString(now);
                    // Split string on spaces.
                    // ... This will separate all the words.
                    string[] words = tempo.Split(' ');
                    foreach (string word in words)
                    {
                        Console.WriteLine(word);
                    }

                    isHigh1 = driver.Read(procPin1);
                    isHigh2 = driver.Read(procPin2);
                    if (isHigh1 != pastStatus1 || isHigh2 != pastStatus2)
                    {
                        if (isHigh1 != pastStatus1)
                            count1++;
                        else
                            count2++;
                        Console.WriteLine(now + "." + now.Millisecond.ToString("000") + ": Pin " + procPin1 + " " + (isHigh1 ? "HIGH" : "LOW"));
                        Console.WriteLine("                         Pin " + procPin2 + " " + (isHigh2 ? "HIGH" : "LOW"));
                        Console.WriteLine("                         Count 1 " + count1 + " Count 2 " + count2);
                        Console.WriteLine("                         Time :");
                    }
                    pastStatus1 = isHigh1;
                    pastStatus2 = isHigh2;
                }

                //// Esempio di attesa fino a che non cambia qualcosa nel pin 
                //isHigh = driver.Read(procPin1);
                //while (true)
                //{
                //    DateTime now = DateTime.Now;
                //    Console.WriteLine(now + "." + now.Millisecond.ToString("000") + ": " + (isHigh ? "HIGH" : "LOW"));
                //    // esempio di codice che attende che il valore cambi, per poi 
                //    // attendere dopo la lettura fino a che non assume il livello negato (fino a che non cambia)
                //    driver.Wait(procPin1, !isHigh, TimeSpan.FromDays(7)); //TODO: infinite
                //    // cambio lo stato della booleana per aspettare la prossima transizione sul fronte basso
                //    isHigh = !isHigh;
                //}

            }
            finally
            {
                // Leaves the pin unreleased so that other processes can keep reading
                //driver.Release(procPin);
            }
        }
    }
}
