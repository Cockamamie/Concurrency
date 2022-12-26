using System;
using System.Diagnostics;
using System.Net;

namespace TPL
{
    public static class Program
	{
		public static void Main(string[] args)
		{
			var ipAddrs = new[] {IPAddress.Parse("192.168.0.1"), IPAddress.Parse("127.0.0.1")/*, Place your ip addresses here*/};
			var ports = new[] {21, 25, 80, 443, 3389 };

			var watch = new Stopwatch();
			watch.Start();
			new ParallelScanner().Scan(ipAddrs, ports).Wait();
			watch.Stop();
			Console.WriteLine(watch.ElapsedMilliseconds);
		}
	}
}
