using System;
using Model;

namespace Hotfix
{
	[MessageHandler(AppType.Gate)]
	public class C2G_JoinMatchRoomHandler : AMRpcHandler<C2G_JoinMatchRoom, G2C_JoinMatchRoom>
	{
		protected override async void Run(Session session, C2G_JoinMatchRoom message, Action<G2C_JoinMatchRoom> reply)
		{
            G2C_JoinMatchRoom response = new G2C_JoinMatchRoom();
			try
			{
				Player player = session.GetComponent<SessionPlayerComponent>().Player;
				// 在map服务器上创建战斗Unit
				string mapAddress = Game.Scene.GetComponent<StartConfigComponent>().MapConfigs[0].GetComponent<InnerConfig>().Address;
				Session mapSession = Game.Scene.GetComponent<NetInnerComponent>().Get(mapAddress);
                M2G_JoinMatchRoom joinRoom = await mapSession.Call<M2G_JoinMatchRoom>(new G2M_JoinMatchRoom() { RoomId = message.RoomId, UnitId = player.UnitId, GateSessionId = session.Id });
				
				response.UnitIds = joinRoom.UnitIds;
				reply(response);
			}
			catch (Exception e)
			{
				ReplyError(response, e, reply);
			}
		}
	}
}