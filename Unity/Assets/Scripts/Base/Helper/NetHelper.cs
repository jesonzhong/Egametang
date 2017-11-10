using System.Collections.Generic;
using System.Net;

namespace Model
{
	public static class NetHelper
	{
		public static string[] GetAddressIPs()
		{
			//获取本地的IP地址
			List<string> addressIPs = new List<string>();
            string hostname = Dns.GetHostName();
            IPHostEntry hostEntry = Dns.GetHostEntry(hostname);
            IPAddress[] ips = hostEntry.AddressList;

            foreach (IPAddress address in ips)
			{
				if (address.AddressFamily.ToString() == "InterNetwork")
				{
					addressIPs.Add(address.ToString());
				}
			}
			return addressIPs.ToArray();
		}
	}
}
