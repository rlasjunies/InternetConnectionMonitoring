using System;
using System.Collections.Generic;

namespace icm
{
    public class Program
    {
        // args[0] can be an IPaddress or host name.
        public static void Main(string[] args)
        {

            var conf = new Configuration {
                IPs = new List<string>(new string[] { "8.8.8.8", "4.2.2.2", "208.67.222.222" }),
                poolingFrequency = 5000,
            } ;
            
            while (true)
            {
                var now = DateTime.Now.ToLocalTime();
                var pingsResults = ICM.pingServers_TrueIfAtLeastOneIsReached(conf.IPs);
                if (pingsResults.AtLeastOneServerReachable)
                {
                    //Console.WriteLine($"{now}-Ping:{reply.Address.ToString()}-{reply.RoundtripTime}");
                    Console.WriteLine($"---");
                    foreach (var serverResult in pingsResults.pingServersResults)
                    {
                        var roundTrip = serverResult.RoundtripTime == 0 ? "--" : serverResult.RoundtripTime.ToString();
                        Console.WriteLine($"{now}|server:{serverResult.ServerIP}|reached:{serverResult.Reachable}|roundtrip:{roundTrip}");
                    }
                }
                else
                {
                    //Console.WriteLine($"{now}-Ping:{reply.Address.ToString()}-Ping fail");
                    Console.WriteLine($"{now}-Ping fail");
                }

                System.Threading.Thread.Sleep(conf.poolingFrequency);
            }
        }
    }
}