using System.Collections.Generic;
using System.Linq;

namespace Model
{
	[ObjectEvent]
	public class PlayerComponentEvent : ObjectEvent<PlayerComponent>, IAwake
	{
		public void Awake()
		{
			this.Get().Awake();
		}
	}
	
	public class PlayerComponent : Component
	{
		public static PlayerComponent Instance { get; private set; }

		public Player MyPlayer;
		
		private readonly Dictionary<string, Player> idPlayers = new Dictionary<string, Player>();

		public void Awake()
		{
			Instance = this;
		}
		
		public void Add(Player player)
		{
			this.idPlayers.Add(player.Account, player);
		}

		public Player Get(string account)
		{
			this.idPlayers.TryGetValue(account, out Player gamer);
			return gamer;
		}

		public void Remove(string account)
		{
			this.idPlayers.Remove(account);
		}

		public int Count
		{
			get
			{
				return this.idPlayers.Count;
			}
		}

		public Player[] GetAll()
		{
			return this.idPlayers.Values.ToArray();
		}

		public override void Dispose()
		{
			if (this.Id == 0)
			{
				return;
			}
			
			base.Dispose();

			foreach (Player player in this.idPlayers.Values)
			{
				player.Dispose();
			}

			Instance = null;
		}
	}
}