using System.Collections.Generic;
using System.Linq;

namespace Model
{
	[ObjectEvent]
	public class MatchRoomComponentEvent : ObjectEvent<MatchRoomComponent>, IAwake
	{
		public void Awake()
		{
			this.Get().Awake();
		}
	}
	
	public class MatchRoomComponent : Component
	{
		private readonly Dictionary<long, MatchRoom> idRooms = new Dictionary<long, MatchRoom>();

		public void Awake()
		{
		}
		
		public void Add(MatchRoom room)
		{
			this.idRooms.Add(room.Id, room);
		}

		public MatchRoom Get(long id)
		{
			this.idRooms.TryGetValue(id, out MatchRoom gamer);
			return gamer;
		}

		public void Remove(long id)
		{
			this.idRooms.Remove(id);
		}

		public int Count
		{
			get
			{
				return this.idRooms.Count;
			}
		}

		public MatchRoom[] GetAll()
		{
			return this.idRooms.Values.ToArray();
		}

		public override void Dispose()
		{
			if (this.Id == 0)
			{
				return;
			}
			
			base.Dispose();

			foreach (MatchRoom room in this.idRooms.Values)
			{
                room.Dispose();
			}
		}
	}
}