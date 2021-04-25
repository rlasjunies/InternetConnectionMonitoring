using System;
using System.Collections.Generic;

namespace icm
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ICM.pingServersResult pingsResults;
            var conf = new Configuration
            {
                IPs = new List<string>(new string[] { "8.8.8.8", "4.2.2.2", "208.67.222.222" }),
                poolingFrequency = 1000,
            };

            while (true)
            {
                var now = DateTime.Now.ToLocalTime();
                pingsResults = ICM.pingServers(conf.IPs);
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

                        pingsResults = ICM.pingServers(conf.IPs);
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

                LetWaitFewSeconds(conf)
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

            static void LetWaitFewSeconds(Configuration conf)
            {
                System.Threading.Thread.Sleep(conf.poolingFrequency);
            }
        }
    }
}