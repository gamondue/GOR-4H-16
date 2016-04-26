using System;
using System.Threading;
using Raspberry.IO.GeneralPurpose;
using Raspberry.IO.GeneralPurpose.Behaviors;

namespace Test.Gpio.Chaser
{
    /// <summary>
    /// This is a sample program. Must be modified to match your GPIO project.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            const ConnectorPin led1Pin = ConnectorPin.P1Pin25;
            const ConnectorPin buttonPin = ConnectorPin.P1Pin20;
            

            

            // Declare input (switchButton) interacting with the leds behavior
            var switchButton = buttonPin.Input()
                .Name("Switch")
                .Revert()
                .Switch()
                .Enable()
                .OnStatusChanged(b =>
                                     {
                                         var driver = args.GetDriver();

                                         // Declare outputs (leds)
                                         var leds = new PinConfiguration[]
                           {
                               led1Pin.Output().Name("Led1").Enable()
                           };

                                         // Assign a behavior to the leds
                                         var behavior = new ChaserBehavior(leds)
                                         {
                                             Loop = args.GetLoop(),
                                             RoundTrip = args.GetRoundTrip(),
                                             Width = args.GetWidth(),
                                             Interval = TimeSpan.FromMilliseconds(args.GetSpeed())
                                         };

                                         behavior.RoundTrip = !behavior.RoundTrip;
                                         Console.WriteLine("Button switched {0}", b ? "on" : "off");
                                     });
            
        }
    }
}
