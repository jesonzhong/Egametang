using System.Threading.Tasks;
using Model;

namespace Hotfix
{
	[ActorMessageHandler(AppType.Map)]
	public class Request_UseSkillHandler : AMActorHandler<Unit, Request_UseSkill>
	{
		protected override async Task Run(Unit unit, Request_UseSkill message)
		{
			Log.Debug("id: "+message.Id);

            Response_UseSkill us = new Response_UseSkill();
            us.Id = message.Id;
            us.skillId = message.skillId;
            MessageHelper.Broadcast(us);

            //unit.GetComponent<UnitGateComponent>().GetActorProxy().Send(message);
        }
	}
}