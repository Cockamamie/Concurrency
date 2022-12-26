using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace TPL
{
    public class ParallelScanner: IPScanner
    {
        public Task Scan(IPAddress[] ipAddrs, int[] ports)
        {
            var tasks = new List<Task>();
            foreach(var ipAddr in ipAddrs)
            {
                if(PingAddr(ipAddr) != IPStatus.Success)
                    continue;
            
                var checkPortTasks = ports.Select(p => Task.Run(() => CheckPort(ipAddr, p)));
                tasks.AddRange(checkPortTasks);
            }

            return Task.WhenAll(tasks);
        }
        
        private IPStatus PingAddr(IPAddress ipAddr, int timeout = 3000)
        {
            using var ping = new Ping();

            Console.WriteLine($"Pinging {ipAddr}");
            var status = ping.SendPingAsync(ipAddr, timeout).Result.Status;
            Console.WriteLine($"Pinged {ipAddr}: {status}");
			
            return status;
        }

        private static void CheckPort(IPAddress ipAddr, int port, int timeout = 3000)
        {
            using var tcpClient = new TcpClient();
			
            Console.WriteLine($"Checking {ipAddr}:{port}");
            var portStatus = tcpClient.ConnectAsync(ipAddr, port, timeout).Result; 
            Console.WriteLine($"Checked {ipAddr}:{port} - {portStatus}");
        }
    }
}