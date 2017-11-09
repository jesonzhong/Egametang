using System.Collections.Generic;

namespace Model
{
	public class RealmGateAddressComponent : Component
	{
		public readonly List<StartConfig> GateAddress = new List<StartConfig>();
        public Dictionary<string, int> AccountGateIndex = new Dictionary<string, int>();
		public StartConfig GetAddress()
		{
			int n = RandomHelper.RandomNumber(0, this.GateAddress.Count);
			return this.GateAddress[n];
		}

        public StartConfig GetAddressByAccount(string account)
        {
            if (AccountGateIndex.ContainsKey(account))
            {
                return this.GateAddress[AccountGateIndex[account]];
            }
            else
            {
                int n = RandomHelper.RandomNumber(0, this.GateAddress.Count);
                StartConfig config  = this.GateAddress[n];
                AccountGateIndex.Add(account, n);
                return config;
            }
        }
    }
}
