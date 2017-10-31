using UnityEngine;

namespace Model
{
    [ObjectEvent]
    public class OperaComponentEvent : ObjectEvent<OperaComponent>, IUpdate, IAwake
    {
        public void Update()
        {
            this.Get().Update();
        }

	    public void Awake()
	    {
		    this.Get().Awake();
	    }
    }

    public class OperaComponent: Component
    {
        public Vector3 ClickPoint;

	    public int mapMask;

	    public void Awake()
	    {
		    this.mapMask = LayerMask.GetMask("Map");
	    }

        public void Update()
        {
 	        Vector2 mInputVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
	        Vector2 direction = mInputVector;
	        direction.Normalize();
	        SessionComponent.Instance.Session.Send(new Frame_ClickMap() { X = (int)(direction.x * 1000), Z = (int)(direction.y * 1000) });

            //just for test
            if (Input.GetMouseButtonDown(1))
            {
                Unit unit = Game.Scene.GetComponent<UnitComponent>().Get(PlayerComponent.Instance.MyPlayer.UnitId);

                unit.GetComponent<HUDComponent>().AddHpValue(0.05f);
            }

            if (Input.GetMouseButtonDown(0))
            {
                Unit unit = Game.Scene.GetComponent<UnitComponent>().Get(PlayerComponent.Instance.MyPlayer.UnitId);

                unit.GetComponent<HUDComponent>().AddHpValue(-0.05f);
            }
        }
    }
}
