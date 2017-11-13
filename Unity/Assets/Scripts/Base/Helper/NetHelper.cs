using System;
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
            
            Log.Info("GetAddressIPs addressIPs :" + addressIPs.Count);
            string hostname = Dns.GetHostName();
            Log.Info("GetAddressIPs hostname :" + hostname);

            IPHostEntry hostEntry = Dns.GetHostEntry(hostname);
            IPAddress[] ips = hostEntry.AddressList;//Dns.GetHostAddresses(hostname);

            foreach (IPAddress address in ips)
			{
				if (address.AddressFamily.ToString() == "InterNetwork")
				{
                    string addressName = address.ToString();
                    Log.Info("GetAddressIPs addressName :" + addressName);
                    addressIPs.Add(addressName);
				}
			}
			return addressIPs.ToArray();
		}
    }
}
