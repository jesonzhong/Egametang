namespace Model
{
	[ObjectEvent]
	public class GamerEvent : ObjectEvent<Player>, IAwake<string>
	{
		public void Awake(string account)
		{
			this.Get().Awake(account);
		}
	}

    public enum PlayerStatus
    {
        Online = 0,
        Leave = 1,
        Lock = 2,
    }

    public sealed class Player : Entity
	{
		public string Account { get; private set; }
		
		public long UnitId { get; set; }

        public PlayerStatus PlayerStatus = PlayerStatus.Leave;


        public void Awake(string account)
		{
			this.Account = account;
            PlayerStatus = PlayerStatus.Online;

        }

        public void Leave()
        {
            PlayerStatus = PlayerStatus.Leave;
        }

		public override void Dispose()
		{
			if (this.Id == 0)
			{
				return;
			}

			base.Dispose();
		}
	}
}