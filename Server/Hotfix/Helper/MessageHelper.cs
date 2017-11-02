using Model;

namespace Hotfix
{
	public static class MessageHelper
	{
		public static void Broadcast(AActorMessage message)
		{
			Unit[] units = Game.Scene.GetComponent<UnitComponent>().GetAll();
            Broadcast(message, units);
        }

        public static void Broadcast(AActorMessage message,Unit[] units)
        {
            ActorProxyComponent actorProxyComponent = Game.Scene.GetComponent<ActorProxyComponent>();
            foreach (Unit unit in units)
            {
                long gateSessionId = unit.GetComponent<UnitGateComponent>().GateSessionId;
                actorProxyComponent.Get(gateSessionId).Send(message);
            }
        }
    }
}
