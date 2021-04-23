using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;


namespace icm
{
    public static class ICM
    {

        public record pingOneServerResult
        {
            public System.Net.IPAddress ServerIP { get; init; }
            public long RoundtripTime { get; init; }
            public int Ttl { get; init; }
            public bool Reachable { get; init;}

        }

        public static pingOneServerResult pingOneServer_TrueIfIsReachable(string serverAddress)
        {
            Ping pingSender = new Ping();
            PingOptions options = new PingOptions();

            // Use the default Ttl value which is 128,
            // but change the fragmentation behavior.
            options.DontFragment = true;

            // Create a buffer of 32 bytes of data to be transmitted.
            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            int timeout = 120;

            PingReply reply = pingSender.Send(serverAddress, timeout, buffer, options);
            // return (reply.Status == IPStatus.Success);
            return new pingOneServerResult
            {
                ServerIP = reply.Address,
                RoundtripTime = reply.RoundtripTime,
                Ttl = reply.Options is null? 0 : reply.Options.Ttl,
                Reachable = (reply.Status == IPStatus.Success)
            };
        }

        // ping a server list, 
        // return false if none are reachable, 
        // true elsewhere
        public static pingServersResult pingServers_TrueIfAtLeastOneIsReached(List<string> serversIP)
        {

            // var now = DateTime.Now.ToLocalTime();
            var IsOneServerReachable = false;
            List<pingOneServerResult> pingsResults = new List<pingOneServerResult>{};
            foreach (var serverIP in serversIP)
            {
                var pingOneServerResult = pingOneServer_TrueIfIsReachable(serverIP);
                pingsResults.Add(pingOneServerResult);
                IsOneServerReachable = IsOneServerReachable || pingOneServerResult.Reachable;
            }

            return new pingServersResult {
                pingServersResults = pingsResults,
                AtLeastOneServerReachable = IsOneServerReachable
            };
            //     Console.WriteLine($"{now}-Ping:{reply.Address.ToString()}-{reply.RoundtripTime}");
            //     Console.WriteLine($"{now}-Ping:{reply.Address.ToString()}-Ping fail");
        }

        public record pingServersResult
        {
            public List<pingOneServerResult> pingServersResults {get;init;}
            public bool AtLeastOneServerReachable { get; init;}

        }
    }
}