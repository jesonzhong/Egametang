using UnityEngine;

namespace Model
{
	public enum UnitType
	{
		Hero,
		Npc
	}

	[ObjectEvent]
	public class UnitEvent : ObjectEvent<Unit>
	{
	}

	public sealed class Unit: Entity
	{
		public VInt3 IntPos;

		public GameObject GameObject;
        public GameObject HudGameObject;
		
		public void Awake()
		{
		}

		public Vector3 Position
		{
			get
			{
				return GameObject.transform.position;
			}
			set
			{
				GameObject.transform.position = value;
			}
		}

		public Quaternion Rotation
		{
			get
			{
				return GameObject.transform.rotation;
			}
			set
			{
				GameObject.transform.rotation = value;
			}
		}

		public override void Dispose()
		{
			if (this.Id == 0)
			{
				return;
			}

            if (this.GameObject != null)
            {
                PoolManager.Despawn(this.GameObject);
            }

            if (this.HudGameObject != null)
            {
                PoolManager.Despawn(this.HudGameObject);
            }

			base.Dispose();
		}
	}
}