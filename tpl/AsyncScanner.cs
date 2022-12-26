using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace TPL
{
    public class AsyncScanner: IPScanner
    {
        public async Task Scan(IPAddress[] ipAddrs, int[] ports)
        {
            foreach(var ipAddr in ipAddrs)
            {
                if (await PingAddr(ipAddr).ConfigureAwait(false) != IPStatus.Success)
                    continue;

                foreach(var port in ports)
                    await CheckPort(ipAddr, port).ConfigureAwait(false);
            }
        }

        private async Task<IPStatus> PingAddr(IPAddress ipAddr, int timeout = 3000)
        {
            using var ping = new Ping();

            Console.WriteLine($"Pinging {ipAddr}");
            var pingReply = await ping.SendPingAsync(ipAddr, timeout).ConfigureAwait(false);
            var status = pingReply.Status;
            Console.WriteLine($"Pinged {ipAddr}: {status}");
			
            return status;
        }

        private static async Task CheckPort(IPAddress ipAddr, int port, int timeout = 3000)
        {
            using var tcpClient = new TcpClient();
			
            Console.WriteLine($"Checking {ipAddr}:{port}");
            var portStatus = await tcpClient.ConnectAsync(ipAddr, port, timeout).ConfigureAwait(false); 
            Console.WriteLine($"Checked {ipAddr}:{port} - {portStatus}");
        }
    }
}