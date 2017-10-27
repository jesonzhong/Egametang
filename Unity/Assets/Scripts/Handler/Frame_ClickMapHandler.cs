using UnityEngine;

namespace Model
{
	[MessageHandler(Opcode.Frame_ClickMap)]
	public class Frame_ClickMapHandler : AMHandler<Frame_ClickMap>
	{
		protected override void Run(Frame_ClickMap message)
		{
			Unit unit = Game.Scene.GetComponent<UnitComponent>().Get(message.Id);
			MoveComponent moveComponent = unit.GetComponent<MoveComponent>();
			Vector3 dir = new Vector3(message.X / 1000f,0,message.Z / 1000f);
			moveComponent.MoveToDir(dir, 10);

			CameraComponent camComponent = Game.Scene.GetComponent<CameraComponent>();
			camComponent.UpdatePosition();
			//moveComponent.Turn2D(dest - unit.Position);
		}
	}
}
