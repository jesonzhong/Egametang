using System.Collections.Generic;
using System.Linq;

namespace Model
{
	[ObjectEvent]
	public class CombatRoomEvent : ObjectEvent<CombatRoom>, IAwake
	{
		public void Awake()
		{
			this.Get().Awake();
		}
	}
    
    public class CombatRoom : Entity
	{
        public int Frame;
        public FrameMessage FrameMessage;
        private readonly Dictionary<long, Unit> idUnits = new Dictionary<long, Unit>();

        public void Add(Unit unit)
        {
            this.idUnits.Add(unit.Id, unit);
        }

        public Unit Get(long id)
        {
            this.idUnits.TryGetValue(id, out Unit unit);
            return unit;
        }

        public void Remove(long id)
        {
            Unit unit;
            this.idUnits.TryGetValue(id, out unit);
            this.idUnits.Remove(id);
            unit?.Dispose();
        }

        public void RemoveNoDispose(long id)
        {
            this.idUnits.Remove(id);
        }

        public int Count
        {
            get
            {
                return this.idUnits.Count;
            }
        }

        public Unit[] GetAll()
        {
            return this.idUnits.Values.ToArray();
        }

        public void Awake()
		{
            Frame = 0;
            FrameMessage = new FrameMessage() { Frame = Frame };
            //UpdateFrameAsync();
		}

        public async void UpdateFrameAsync()
        {
            TimerComponent timerComponent = Game.Scene.GetComponent<TimerComponent>();

            while (true)
            {
                if (Id == 0)
                {
                    return;
                }

                await timerComponent.WaitAsync(40);
                Broadcast(FrameMessage,GetAll());
                ++Frame;
                FrameMessage = new FrameMessage() { Frame = Frame };
            }
        }

        public void Add(AFrameMessage message)
        {
            FrameMessage.Messages.Add(message);
        }

        void Broadcast(AActorMessage message, Unit[] units)
        {
            ActorProxyComponent actorProxyComponent = Game.Scene.GetComponent<ActorProxyComponent>();
            foreach (Unit unit in units)
            {
                long gateSessionId = unit.GetComponent<UnitGateComponent>().GateSessionId;
                actorProxyComponent.Get(gateSessionId).Send(message);
            }
        }

        public override void Dispose()
		{
			if (this.Id == 0)
			{
				return;
			}

			base.Dispose();

            foreach (Unit unit in this.idUnits.Values)
            {
                unit.Dispose();
            }
        }
	}
}