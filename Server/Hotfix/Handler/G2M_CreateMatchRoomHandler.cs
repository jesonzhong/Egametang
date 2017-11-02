using System;
using Model;

namespace Hotfix
{
	[MessageHandler(AppType.Map)]
	public class G2M_CreateMatchRoomHandler : AMRpcHandler<G2M_CreateMatchRoom, M2G_CreateMatchRoom>
	{
		protected override async void Run(Session session, G2M_CreateMatchRoom message, Action<M2G_CreateMatchRoom> reply)
		{
            M2G_CreateMatchRoom response = new M2G_CreateMatchRoom();
			try
			{
                MatchRoomComponent roomComponent = Game.Scene.GetComponent<MatchRoomComponent>();
                MatchRoom room = EntityFactory.Create<MatchRoom>();
                roomComponent.Add(room);
                
				response.RoomId = room.Id;
                response.Count = 0;

                reply(response);
			}
			catch (Exception e)
			{
				ReplyError(response, e, reply);
			}
		}
	}
}