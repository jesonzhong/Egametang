using System.Collections.Generic;
using System.Linq;

namespace Model
{
	[ObjectEvent]
	public class CombatRoomComponentEvent : ObjectEvent<CombatRoomComponent>, IAwake
	{
		public void Awake()
		{
			this.Get().Awake();
		}
	}
	
	public class CombatRoomComponent : Component
	{
		private readonly Dictionary<long, CombatRoom> idRooms = new Dictionary<long, CombatRoom>();

		public void Awake()
		{
		}
		
		public void Add(CombatRoom room)
		{
			this.idRooms.Add(room.Id, room);
		}

		public CombatRoom Get(long id)
		{
			this.idRooms.TryGetValue(id, out CombatRoom gamer);
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

		public CombatRoom[] GetAll()
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

			foreach (CombatRoom room in this.idRooms.Values)
			{
                room.Dispose();
			}
		}
	}
}