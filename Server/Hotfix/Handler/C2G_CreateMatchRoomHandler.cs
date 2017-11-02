using System;
using Model;

namespace Hotfix
{
	[MessageHandler(AppType.Gate)]
	public class C2G_CreateMatchRoomHandler : AMRpcHandler<C2G_CreateMatchRoom, G2C_CreateMatchRoom>
	{
		protected override async void Run(Session session, C2G_CreateMatchRoom message, Action<G2C_CreateMatchRoom> reply)
		{
            G2C_CreateMatchRoom response = new G2C_CreateMatchRoom();
			try
			{
				Player player = session.GetComponent<SessionPlayerComponent>().Player;
				// 在map服务器上创建战斗Unit
				string mapAddress = Game.Scene.GetComponent<StartConfigComponent>().MapConfigs[0].GetComponent<InnerConfig>().Address;
				Session mapSession = Game.Scene.GetComponent<NetInnerComponent>().Get(mapAddress);
                M2G_CreateMatchRoom createUnit = await mapSession.Call<M2G_CreateMatchRoom>(new G2M_CreateMatchRoom() { UnitId = player.UnitId, GateSessionId = session.Id });
				
				response.RoomId = createUnit.RoomId;
				reply(response);
			}
			catch (Exception e)
			{
				ReplyError(response, e, reply);
			}
		}
	}
}