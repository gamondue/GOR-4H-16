﻿using System;
using System.Collections.Generic;
using System.Threading;
using Gor.Devices;
using System.IO;

namespace Gor.Acquisition.Daemon
{
    /// <summary>
    /// GorAcquire main program
    /// </summary>
    class Program
    {
        private const double samplePeriod = 1 / 12.0;            // [minutes]
        //private const double samplePeriod = 1;            // [minutes]
        private const bool readConfigFromFile = true; 

        // ADC channel of sensors
        private const int RELATIVE_HUMIDITY_CHANNEL = 0;
        private const int PHOTO_RESISTOR_CHANNEL = 1;
        private const int TERRAIN_HUMIDITY_CHANNEL = 2;

        // sensori con stelo ITT
        const string idTermometro = "28-000006707ae6"; // gor3 172.16.13.103
        //const string idTermometro = "28-0000066e578f"; // gor2 172.16.13.102
        //const string idTermometro = "28-0000066e88a3"; // gor0 172.16.13.200
        //const string idTermometro = "28-0000066f1902"; // gor0 172.16.13.200

        // sensori in circuito ITT 
        //const string idTermometro = "22-0000003c0ff9"; // gor2 172.16.13.102
        //const string idTermometro = "28-0000062196f0"; // gor0 172.16.13.200

        // sensori con stelo gamon
        //const string idTermometro = "28-00042c5e80ff";
        //const string idTermometro = "28-00042e0c65ff";
        //const string idTermometro = "28-00042c643aff"; 
        //const string idTermometro = "28-00042e0c59ff"; 

        static DateTime nextSampleTime;

        static List<Sensor> Sensors;    // list of all sensors used by this program 
        static GorDbWriter dbWriter;    // dbms writing class

        static Adc_MCP3208 converter;

        static Humidity_Air_HIH4000 relativeHumidity;
        static Light_PhotoResistor light;
        static Humidity_Temperature_Air_DHT22 airHumidityAndTemperature;
        static Humidity_Terrain_YL69YL38 terrainHumidity;
        static Temperature_DS1822 temperature;

        //const int RTC_ADDRESS = 0x51; // realtime clock (not working)
        ////Rtc_PCF8563 rtc = new Rtc_PCF8563(RTC_ADDRESS, i2cDriver);
        //Rtc_PCF8563 rtc = new Rtc_PCF8563(RTC_ADDRESS);

        static void Main(string[] args)
        {
            Common.InitializeCommonVariables(); 
            Common.logger.Debug("Main_00");

            //Common.logger.LoggingPrompts = false;
            //Common.logger.ShowingDebug = false;
            //Common.logger.ShowingErrors = false;
            //Common.logger.ShowingEvents = false;

            Common.logger.Prompt("GorAcquire");
            Common.logger.Prompt("Garden Of Raspberries");
            Common.logger.Prompt("Programma di acquisizione dati");
            Common.logger.Prompt("Vers." + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            Common.logger.Prompt(""); 
            Common.logger.Prompt("Sample period: " + samplePeriod + " min");
            Common.logger.Prompt("Sensor simulation: " + Gor.Common.sensorsSimulation);
            Common.logger.Prompt("Read configuration from file: " + readConfigFromFile);
            
            Sensors = new List<Sensor>();

            try
            {
                Initialize(Gor.Common.sensorsSimulation); // viene passata la modalità di simulazione

                nextSampleTime = DateTime.Now.AddMinutes(samplePeriod);

                while (!exitProgram())
                {
                    Sample();
                    SaveLog();
                    Wait();
                }
            }
            catch (Exception e)
            {
                Common.logger.Error(e.Message);
            }
            Common.logger.Debug("Stopping"); 
        }

        /// <summary>
        /// Legge se nel file "close.txt" c'è un numero diverso da "0"
        /// Se c'è un numero diverso da "0" torna con un vero, altrimenti falso
        /// ATTENZIONE: file il close.txt deve avere diritti di scrittura non solo per root
        ///             impostarli così: 
        ///             sudo chmod 666 close.txt
        /// </summary>
        /// <returns></returns>
        private static bool exitProgram()
        {
            // Neri Luca 5F
            try
            {
                int c = readControlFile(Common.CloseCommandFile);
                //Common.logger.Debug(Common.CloseCommandFile + " = " + c.ToString());

                if (c == 49) // codice ASCII di 1 
                {
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                Common.logger.Error("exitProgram " + ex.Message);
                return false; // non esce se sbagli a leggere 
            }
        }

        private static void Initialize(bool inSimulation)
        {
            Common.logger.Debug("Initialize_00");

            converter = null; // per default il convertitore è null
            if (!inSimulation)
            {                
                // acquisizione reale, convertitore
                converter = new Adc_MCP3208();
            }
            Common.logger.Debug("Initialize_10");

            if (readConfigFromFile)
            {   // lettura della configurazione da file
                configureFromFile();
                // intestazione del file dei dati: appende nel file .tsv l'intestazione
                string rigaIntestazione = "Istante campionamento";
                Common.logger.Debug("Initialize_20");
                foreach(Sensor s in Sensors)
                {
                    foreach (Measurement m in s.LastMeasurements)
                    {
                        rigaIntestazione += "\t" + m.Name + " " + m.Unit;
                    }
                }
                Common.logger.Debug("Initialize_30");
                textFileAppend(rigaIntestazione, Gor.Common.DatalogFile);
                Common.logger.Debug("Initialize_40");
            }
            else
            {
                // configurazione fissa
                Common.logger.Debug("Initialize_50");
                fixedConfiguration(inSimulation);
                Common.logger.Debug("Initialize_60");
            }

            //Rtc_PCF8563 rtc = new Rtc_PCF8563(RTC_ADDRESS);

            // mette zero nel file che stabilisce se il programma deve fermarsi
            putZeroInControlFile(Common.CloseCommandFile);
            putZeroInControlFile(Common.AcquireCommandFile);

            Common.logger.Debug("Initialize_99"); 

            return;
        }
        private static void Sample()
        {
            if (!readConfigFromFile)
            { // configurazione fissa
                Common.logger.Debug("Acquire_00");
                Common.logger.Prompt("\nSampling: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ");
                Common.logger.Prompt("Umidita' dell'aria: " + (relativeHumidity.Measure().ToString()));
                Common.logger.Debug("Acquire_10");

                Common.logger.Prompt("Temperatura: " + temperature.Measure().ToString());
                Common.logger.Debug("Acquire_20");

                Common.logger.Prompt("Luminosita': " + light.Measure().ToString());
                Common.logger.Debug("Acquire_30");

                //log.Prompts("Umidità del terreno: " + terrainHumidity.Measure());
            
                Common.logger.Prompt("");
                Common.logger.Debug("Acquire_99");
            }
            else
            {   // configurazione in Sensors
                Common.logger.Debug("Acquire_1_00");
                Common.logger.Prompt("\nSampling: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ");
                Common.logger.Debug("Acquire_1_05");

                foreach (Sensor s in Sensors)
                {
                    try
                    {
                        Common.logger.Debug("Acquire_1_10" + s.Name);
                        s.Measure();
                        foreach(Measurement m in s.LastMeasurements)
                        {
                            Common.logger.Prompt(m.Name + " " + (m.ToString()));
                        }
                        Common.logger.Debug("Acquire_1_20");
                    }
                    catch (Exception ex)
                    {
                        Common.logger.Error("Errore nella lettura di un sensore" + 
                            ex.Message);
                    }
                }
                Common.logger.Debug("Acquire_1_30");

                Common.logger.Prompt("");
                Common.logger.Debug("Acquire_1_99");
            }
            return;
        }
        private static void SaveLog()
        {
            // salvataggio delle misurazioni su file locale ASCII "datalog.tsv" (tab separated values)
            // una riga, un campionamento
            // prima riga: i nomi delle colonne, separati da tab (già fatto in Initialize())
            Common.logger.Debug("Save_00");

            string rigaDati = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); 
            foreach(Sensor sensore in Sensors)
            {
                try
                {
                    Common.logger.Debug("Save_10 " + sensore.Name);
                    foreach (Measurement m in sensore.LastMeasurements)
                    {
                        rigaDati += "\t" + m.Value.ToString(m.DisplayFormat);
                        Common.logger.Debug("Save_15 " + rigaDati);
                    }
                    //datalogAppend(sensore.LastMeasurement.ToString(), Common.DatalogFile); 
                }
                catch
                {
                    Common.logger.Error("Save_20 " + sensore.Name);
                }
            }
            textFileAppend(rigaDati, Gor.Common.DatalogFile);

            //TODO salvataggio su database
            //////dbWriter.SaveAll(Sensors); 

            Common.logger.Debug("Save_99");
            return;
        }

        /// <summary>
        /// Returns when next sample time is reached
        /// </summary>
        private static void Wait()
        {
            // remade by Monti: from 2015-03-05

            nextSampleTime = nextSampleTime.AddMinutes(samplePeriod);

            if ((int)samplePeriod ==  samplePeriod) { 
                // if the sample period is an integer or more minutes, then we will freeze the program 
                // until we are near to the straight minute (00 second).
                // We will then wait in a active loop to get the straight second

                // set next sample instant
                DateTime now = DateTime.Now;
                // time difference to go to the next sample time, NOT truncated
                TimeSpan span = new TimeSpan(0, (int)samplePeriod,0); // hh, mm, ss
                // find out next "raw" time: variable next (not truncated to the next sharp minute)
                DateTime next = now.Add(span); 
            
                // find the minute BEFORE next "raw" minute that is sharp: variable nextMinute
                int nextMinute = (next.Minute / (int) samplePeriod) * (int)samplePeriod;
                Common.logger.Debug("Wait: nextMinute: " + nextMinute.ToString());
            
                // build next sample time
                nextSampleTime = new DateTime(next.Year, next.Month, next.Day,
                    next.Hour, nextMinute, 0);
                Common.logger.Prompt("Waiting for the next sample time: " + nextSampleTime.ToString("yyyy-MM-dd HH:mm:ss"));
            
                // passive sleep with some awakenings, until 15 seconds before NextSampleTime
                DateTime littleEarlier = nextSampleTime.AddSeconds(-15);
                while (DateTime.Now < littleEarlier)
                {
                    Thread.Sleep(5000);
                    // when awake: check if I have to make a sample 
                    sampleIfRequested(); 
                    // when awake: check if I have to stop program
                    if (exitProgram())
                        return; // if I have to stop, exit method; main program will stop
                }
            }
            // active sleep, to catch sharp sample time instant
            while (DateTime.Now < nextSampleTime) ;

            return;
        }

        private static void fixedConfiguration(bool inSimulation)
        {
            try
            {
                // istanziazione dei sensori 
                relativeHumidity = new Humidity_Air_HIH4000("RH%_HIH4000", inSimulation, converter, RELATIVE_HUMIDITY_CHANNEL, Common.logger);
                Sensors.Add(relativeHumidity);
                Common.logger.Debug("Istanziazione: " + relativeHumidity.Measure().ToString());
            }
            catch
            {
                Common.logger.Error("Istanziazione: Humidity_Air_HIH4000");
            }

            try
            {
                light = new Light_PhotoResistor("Light", inSimulation, converter, PHOTO_RESISTOR_CHANNEL, Common.logger);
                Sensors.Add(light);
                Common.logger.Debug("Istanziazione: " + light.Measure().ToString());
            }
            catch
            {
                Common.logger.Error("Istanziazione: Light_PhotoResistor");
            }

            try
            {
                temperature = new Temperature_DS1822("Tair_DS1822", inSimulation, Common.logger);
                Sensors.Add(temperature);
                Common.logger.Debug("Istanziazione: " + temperature.Measure().ToString());
            }
            catch
            {
                Common.logger.Error("Istanziazione: Temperature_DS1822");
            }
            //terrainHumidity = new Humidity_Terrain_YL69YL38(inSimulation, converter, TERRAIN_HUMIDITY_CHANNEL);
        }

        private static void configureFromFile()
        {
            Common.logger.Debug("configureFromFile_00");
            using (FileStream svs = new FileStream(Gor.Common.ConfigurationFile, FileMode.Open,
                FileAccess.Read, FileShare.Read))
            using (StreamReader rd = new StreamReader(svs))
            {
                Common.logger.Debug("configureFromFile_20");
                string s = rd.ReadToEnd();
                string[] righe = s.Replace("\r", "").Split('\n');
                int i = 0; 
                foreach (string riga in righe)
                {
                    Common.logger.Debug("configureFromFile_25: riga " + riga);
                    if (i != 0 && riga.Trim() != "")
                    {
                        string[] campi = riga.Split('\t');
                        Common.logger.Debug("configureFromFile_30 " + campi[0]);
                        Sensor sens = null;
                        switch (campi[0])
                        {
                            case "Temperature_DS1822":
                                {
                                    Common.logger.Debug("configureFromFile_32 Temperature_DS1822 " + campi[1] + campi[4] + campi[2]);
                                    // la seguente usa il costruttore che indica l'ID del sensore
                                    //sens = new Temperature_DS1822(campi[1], bool.Parse(campi[4]), campi[2], Common.logger);
                                    // la seguente usa il costruttore che trova automaticamente l'ID del sensore
                                    sens = new Temperature_DS1822(campi[1], bool.Parse(campi[4]), Common.logger);
                                    break;
                                }
                            case "Humidity_Air_HIH4000":
                                {
                                    Common.logger.Debug("configureFromFile_34 Humidity_Air_HIH4000 " + campi[1] + campi[4] + campi[2]);
                                    sens = new Humidity_Air_HIH4000(campi[1], bool.Parse(campi[4]), converter, int.Parse(campi[2]), Common.logger);
                                    break;
                                }
                            case "Light_PhotoResistor":
                                {
                                    Common.logger.Debug("configureFromFile_36 Light_PhotoResistor " + campi[1] + campi[4] + campi[2]);
                                    sens = new Light_PhotoResistor(campi[1], bool.Parse(campi[4]), converter, int.Parse(campi[2]), Common.logger);
                                    break;
                                }
                            case "Humidity_Temperature_Air_DHT22":
                                {
                                    Common.logger.Debug("configureFromFile_37 Humidity_Temperature_Air_DHT22 " + campi[1] + campi[4] + campi[2]);
                                    sens = new Humidity_Temperature_Air_DHT22(campi[1], bool.Parse(campi[4]), int.Parse(campi[2]), Common.logger);
                                    break;
                                }
                        }
                        Common.logger.Debug("configureFromFile_40 " + campi[5]);
                        sens.AlarmMax = double.Parse(campi[5]);
                        Common.logger.Debug("configureFromFile_42 " + campi[6]);
                        sens.AlarmMin = double.Parse(campi[6]);
                        Common.logger.Debug("configureFromFile_44 " + campi[7]);
                        sens.MaxValue = double.Parse(campi[7]);
                        Common.logger.Debug("configureFromFile_46 " + campi[8]);
                        sens.MinValue = double.Parse(campi[8]);
                        Common.logger.Debug("configureFromFile_48 " + campi[9]);
                        sens.Unit = campi[9];
                        Common.logger.Debug("configureFromFile_50");

                        if (sens != null)
                            Sensors.Add(sens);
                        Common.logger.Debug("configureFromFile_60");
                    }
                    i++;
                }
            }
            Common.logger.Debug("configureFromFile_99");
        }

        private static void sampleIfRequested()
        {
            int c; 
            try
            {
                c = readControlFile(Common.AcquireCommandFile); 
            }
            catch (Exception ex)
            {
                Common.logger.Error("sampleIfRequested " + ex.Message);
                return; 
            }
            if (c == 49) // codice ASCII di 1 
            {
                Common.logger.Prompt("Sample on request"); 
                Sample();
                saveSingleSample();
                putZeroInControlFile(Common.AcquireCommandFile);
            }
            else
                return;
        }

        private static void saveSingleSample()
        {
            // salvataggio delle misurazioni su file locale ASCII "datalog.tsv" (tab separated values)
            // una riga, un campionamento
            // prima riga: i nomi delle colonne, separati da tab (già fatto in Initialize())
            Common.logger.Debug("saveSingleSample_00");
            // cancella il file dell'ultimo campionamento, che verrà riscritto 
            File.Delete(Common.SingleSampleFile); 

            foreach (Sensor sensore in Sensors)
            {
                try
                {
                    foreach (Measurement m in sensore.LastMeasurements)
                    {
                        Common.logger.Debug("saveSingleSample_10 " + sensore.Name);
                        textFileAppend(m.ToString(), Common.SingleSampleFile);
                    }
                }
                catch
                {
                    Common.logger.Error("saveSingleSample_20 " + sensore.Name);
                }
            }
            Common.logger.Debug("saveSingleSample_99");
            return;
        }

        #region helpers
        private static void textFileAppend(string riga, string file)
        {
            try
            {
                using (StreamWriter sw = File.AppendText(file))
                {
                    sw.WriteLine(riga);
                }
            }
            catch (Exception ex)
            {
                Common.logger.Error("textFileAppend " + ex.Message);
            }
        }
        /// <summary>
        /// Mette la stringa "0" nel file "close.txt"
        /// ATTENZIONE: file il close.txt deve avere diritti di scrittura non solo per root
        ///             impostarli così: 
        ///             sudo chmod 666 close.txt
        /// </summary>
        private static void putZeroInControlFile(string File)
        {
            try
            {
                // scrittura di uno zero nel file di controllo 
                using (StreamWriter sw = System.IO.File.CreateText(File))
                {
                    sw.Write("0");
                }
            }
            catch (Exception ex)
            {
                Common.logger.Error("putZeroInControlFile() " + ex.Message);
            }
        }

        private static int readControlFile(string file)
        {
            int c;
            // lettura del primo carattere del file di controllo
            try
            {
                using (StreamReader sr = new StreamReader(file))
                {
                    {
                        c = sr.Read();
                        //Common.logger.Debug("acquire.txt = " + c.ToString());
                    }
                }
                return c;
            }
            catch (Exception ex)
            {
                Common.logger.Error("readControlFile() " + ex.Message);
                return int.MinValue;
            }
        }
        #endregion
    }
}
