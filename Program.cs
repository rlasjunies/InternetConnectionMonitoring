using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace icm
{
    public class Program
    {
        public static Task Main(string[] args)
        {

            ICM.pingServersResult pingsResults;
            ICMConfiguration conf = LoadConfiguration(args);

            while (true)
            {
                var now = DateTime.Now.ToLocalTime();
                pingsResults = ICM.pingServers(conf.servers);
                if (pingsResults.AtLeastOneServerReachable)
                {
                    OutputOkPingsResults(pingsResults, now, conf);
                }
                else
                {
                    var disconnectionDuration = LoopWhileNoConnection(now, conf);
                    OutputReconnection(now, disconnectionDuration, conf);

                }

                LetWaitFewSeconds(conf);
            }

            static void OutputOkPingsResults(ICM.pingServersResult pingsResults, DateTime now, ICMConfiguration conf)
            {
                var pingRoundtripsTimeOutput = "";
                foreach (var serverResult in pingsResults.pingServersResults)
                {
                    pingRoundtripsTimeOutput += serverResult.RoundtripTime == 0 ? "---" : serverResult.RoundtripTime.ToString().PadLeft(3, ' ');
                    pingRoundtripsTimeOutput += "|";
                }
                var outputString = $"{now}|OK         |{pingRoundtripsTimeOutput}";
                Console.WriteLine(outputString);
                if (conf.loginfile) outputInFile(outputString);
            }

            static void ConsoleBackToBeginningOfTheLine()
            {
                try
                {
                    Console.CursorLeft = 0;
                    Console.CursorTop -= 1;
                }
                catch (System.Exception)
                {
                    // debugging console raise error with cursorleft
                }
            }

            static void outputInFile(string outputString)
            {
                string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
                var fileName = System.IO.Path.GetDirectoryName(path)
                                + "\\icm."
                                + DateTime.Now.Year.ToString()
                                + DateTime.Now.Month.ToString().PadLeft(2, '0')
                                + DateTime.Now.Day.ToString().PadLeft(2, '0')
                                + ".csv";
                try
                {
                    File.AppendAllText(fileName, outputString + Environment.NewLine);
                }
                catch (Exception Ex)
                {
                    Console.WriteLine(Ex.ToString());
                }
            }

            static void OutputNoConnection(DateTime nowDisconnected, TimeSpan durationDisconnection, ICMConfiguration conf)
            {
                var outputString = $"{nowDisconnected}|KO         |pings are failing, disconnected since: {durationDisconnection.ToHumanReadableString()}";
                Console.WriteLine(outputString);
                ConsoleBackToBeginningOfTheLine();
                if (conf.loginfiledisconnected) outputInFile(outputString);
            }

            static void LetWaitFewSeconds(ICMConfiguration conf)
            {
                System.Threading.Thread.Sleep(conf.frequency);
            }

            static ICMConfiguration LoadConfiguration(string[] args)
            {
                // default configuration
                ICMConfiguration conf = new ICMConfiguration { };

                Host.CreateDefaultBuilder(args)
                    .ConfigureAppConfiguration((hostingContext, configuration) =>
                    {
                        configuration.Sources.Clear();

                        IHostEnvironment env = hostingContext.HostingEnvironment;

                        configuration
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true);

                        IConfigurationRoot configurationRoot = configuration.Build();

                        ICMConfiguration options = new();
                        configurationRoot.GetSection("ping").Bind(options);

                        conf = options;
                    })
                    .Build();
                return conf;
            }

            static TimeSpan LoopWhileNoConnection(DateTime disconnectionStartTime, ICMConfiguration conf)
            {
                var disconnected = true;
                TimeSpan durationDisconnection = TimeSpan.Zero;

                while (disconnected)
                {
                    var nowDisconnected = DateTime.Now.ToLocalTime();
                    durationDisconnection = nowDisconnected - disconnectionStartTime;

                    var pingsResults = ICM.pingServers(conf.servers);
                    disconnected = !pingsResults.AtLeastOneServerReachable;

                    OutputNoConnection(nowDisconnected, durationDisconnection, conf);

                    LetWaitFewSeconds(conf);
                }

                return durationDisconnection;

            }

            static void OutputReconnection(DateTime now, TimeSpan disconnectionDuration, ICMConfiguration conf)
            {
                ClearCurrentConsoleLine();
                var outputString = $"{now}|RECONNECTED|{disconnectionDuration.Seconds.ToString().PadLeft(3, ' ')}|Disconnected during : {disconnectionDuration.ToHumanReadableString()}";
                Console.WriteLine(outputString);
                if (conf.loginfile) outputInFile(outputString);
            }

            static void ClearCurrentConsoleLine()
            {
                int currentLineCursor = Console.CursorTop;
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, currentLineCursor);
            }
        }
    }
}