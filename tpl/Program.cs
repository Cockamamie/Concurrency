using System.Net;
using System.Threading.Tasks;

namespace TPL
{
    public static class Program
	{
		public static async Task Main(string[] args)
		{
			var ipAddrs = new[] {IPAddress.Parse("192.168.0.1"), IPAddress.Parse("127.0.0.1")/*, Place your ip addresses here*/};
			var ports = new[] {21, 25, 80, 443, 3389 };
			
			new SequentialScanner().Scan(ipAddrs, ports).Wait();
		}
	}
}
