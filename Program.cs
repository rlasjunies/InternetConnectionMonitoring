using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace icm
{
    public class Program
    {
        public static Task Main(string[] args)
        {

            ICM.pingServersResult pingsResults;
            ICMConfiguration conf = new ICMConfiguration {
                servers = new List<string>(new string[] { "8.8.8.8", "4.2.2.2", "208.67.222.222" }),
                frequency = 1000,
            };

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

            while (true)
            {
                var now = DateTime.Now.ToLocalTime();
                pingsResults = ICM.pingServers(conf.servers);
                if (pingsResults.AtLeastOneServerReachable)
                {
                    ReportPingsResults(pingsResults, now);
                }
                else
                {
                    var disconnected = true;
                    while (disconnected)
                    {
                        var nowDisconnected = DateTime.Now.ToLocalTime();
                        var durationDisconnection = nowDisconnected - now;

                        pingsResults = ICM.pingServers(conf.servers);
                        if (pingsResults.AtLeastOneServerReachable)
                        {
                            disconnected = ConnectionIsBack(now, durationDisconnection);
                        }
                        else
                        {
                            StillNotConnection(nowDisconnected, durationDisconnection);
                        }
                        LetWaitFewSeconds(conf);
                    }

                }

                LetWaitFewSeconds(conf);
            }


            static void ReportPingsResults(ICM.pingServersResult pingsResults, DateTime now)
            {
                Console.WriteLine($"---");
                foreach (var serverResult in pingsResults.pingServersResults)
                {
                    var roundTrip = serverResult.RoundtripTime == 0 ? "--" : serverResult.RoundtripTime.ToString();
                    Console.WriteLine($"{now}|server:{serverResult.ServerIP}|reached:{serverResult.Reachable}|roundtrip:{roundTrip}");
                }
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

            static bool ConnectionIsBack(DateTime now, TimeSpan durationDisconnection)
            {
                bool disconnected = false;
                Console.WriteLine($"{now}|disconnected during: {durationDisconnection.ToHumanReadableString()}                                                                                                    ");
                return disconnected;
            }

            static void StillNotConnection(DateTime nowDisconnected, TimeSpan durationDisconnection)
            {
                Console.WriteLine($"{nowDisconnected}|pings are failing, disconnected since: {durationDisconnection.ToHumanReadableString()}");
                ConsoleBackToBeginningOfTheLine();
            }

            static void LetWaitFewSeconds(ICMConfiguration conf)
            {
                System.Threading.Thread.Sleep(conf.frequency);
            }
        }
    }
}