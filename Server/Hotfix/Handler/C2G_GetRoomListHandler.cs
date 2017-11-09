using System;
using Model;

namespace Hotfix
{
	[MessageHandler(AppType.Gate)]
	public class C2G_GetRoomListHandler : AMRpcHandler<C2G_GetRoomList, G2C_GetRoomList>
	{
		protected override async void Run(Session session, C2G_GetRoomList message, Action<G2C_GetRoomList> reply)
		{
            G2C_GetRoomList response = new G2C_GetRoomList();
			try
			{
                string mapAddress = Game.Scene.GetComponent<StartConfigComponent>().Get(9).GetComponent<InnerConfig>().Address;
                Session mapSession = Game.Scene.GetComponent<NetInnerComponent>().Get(mapAddress);
                M2G_GetRoomList createUnit = await mapSession.Call<M2G_GetRoomList>(new G2M_GetRoomList() { });
                response.RoomIds = createUnit.RoomIds;
                reply(response);
			}
			catch (Exception e)
			{
				ReplyError(response, e, reply);
			}
		}
	}
}