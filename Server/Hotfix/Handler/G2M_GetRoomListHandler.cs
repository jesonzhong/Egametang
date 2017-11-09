using System;
using Model;

namespace Hotfix
{
	[MessageHandler(AppType.Map)]
	public class G2M_GetRoomListHandler : AMRpcHandler<G2M_GetRoomList, M2G_GetRoomList>
	{
		protected override async void Run(Session session, G2M_GetRoomList message, Action<M2G_GetRoomList> reply)
		{
            M2G_GetRoomList response = new M2G_GetRoomList();
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