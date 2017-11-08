using System;
using Model;

namespace Hotfix
{
	[MessageHandler(AppType.Map)]
	public class G2M_JoinMatchRoomHandler : AMRpcHandler<G2M_JoinMatchRoom, M2G_JoinMatchRoom>
	{
		protected override async void Run(Session session, G2M_JoinMatchRoom message, Action<M2G_JoinMatchRoom> reply)
		{
            M2G_JoinMatchRoom response = new M2G_JoinMatchRoom();
			try
			{
                MatchRoomComponent roomComponent = Game.Scene.GetComponent<MatchRoomComponent>();
                MatchRoom matchroom = roomComponent.Get(message.RoomId);

                Unit unit = matchroom.unitComponent.Get(message.UnitId);
                if (unit == null)
                {
                    if(message.UnitId >0)
                        unit = EntityFactory.CreateWithId<Unit>(message.UnitId);
                    else
                        unit = EntityFactory.Create<Unit>();
                    
                    
                    unit.RoomID = matchroom.Id;
                    matchroom.unitComponent.Add(unit);
                }
                UnitGateComponent unitGateComponent = unit.GetComponent<UnitGateComponent>();
                if (unitGateComponent != null)
                {
                    Game.Scene.GetComponent<ActorProxyComponent>().Remove(unitGateComponent.GateSessionId);
                    unit.RemoveComponent<UnitGateComponent>();
                    ActorComponent actorComponent = unit.GetComponent<ActorComponent>();
                    await actorComponent.RemoveLocation();
                    unit.RemoveComponent<ActorComponent>();
                }
                
                unit.AddComponent<UnitGateComponent, long>(message.GateSessionId);
                await unit.AddComponent<ActorComponent, IEntityActorHandler>(new MapUnitEntityActorHandler()).AddLocation();

                //Game.Scene.GetComponent<UnitComponent>().Add(unit);

                Unit[] units = matchroom.unitComponent.GetAll();

                response.UnitIds = new long[units.Length];

                for (int i = 0; i < units.Length; i++)
                {
                    response.UnitIds[i] = units[i].Id;
                }
                Response_RankList retRankList = new Response_RankList();
                Actor_CreateUnits actorCreateUnits = new Actor_CreateUnits();
                int tmpScore = 100;
                foreach (Unit u in units)
                {
                    actorCreateUnits.Units.Add(new UnitInfo() { UnitId = u.Id, X = (int)(u.Position.X * 1000), Z = (int)(u.Position.Z * 1000) });
                    retRankList.Units.Add(new RankInfo() { Id = u.Id, name = "张三", score = tmpScore });
                    tmpScore -= 5;
                }
                Log.Debug($"{MongoHelper.ToJson(actorCreateUnits)}");
                MessageHelper.Broadcast(actorCreateUnits, units);
                MessageHelper.Broadcast(retRankList, units);
                if (matchroom.Frame > 0)
                {
                    response.AgoFrameMessage = matchroom.cacheFrameMessage;
                    response.AgoFrameMessage.Frame = matchroom.Frame;
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