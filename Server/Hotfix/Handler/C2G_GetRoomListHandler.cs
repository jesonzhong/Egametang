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
                MatchRoomComponent roomComponent = Game.Scene.GetComponent<MatchRoomComponent>();
                MatchRoom[] rooms = roomComponent.GetAll();

                response.RoomIds = new long[rooms.Length];
                for (int i = 0; i < rooms.Length; i++)
                {
                    response.RoomIds[i] = rooms[i].Id;
                }
                reply(response);
			}
			catch (Exception e)
			{
				ReplyError(response, e, reply);
			}
		}
	}
}