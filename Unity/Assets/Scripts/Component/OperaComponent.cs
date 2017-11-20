using UnityEngine;

namespace Model
{
    [ObjectEvent]
    public class OperaComponentEvent : ObjectEvent<OperaComponent>, IAwake,IFrameUpdate
    {

	    public void Awake()
	    {
		    this.Get().Awake();
	    }

        public void FrameUpdate(int gameFramesPerSecond)
        {
            this.Get().FrameUpdate(gameFramesPerSecond);
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

        public void FrameUpdate(int gameFramesPerSecond)
        {
            Vector2 mInputVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            Vector2 direction = mInputVector;
            if (direction.magnitude > 0)
            {
                direction.Normalize();
                SessionComponent.Instance.Session.Send(new Frame_ClickMap() { X = (int)(direction.x * 1000), Z = (int)(direction.y * 1000) });
            }

            //just for test
            if (Input.GetMouseButtonDown(1))
            {

            }

            if (Input.GetMouseButtonDown(0))
            {

            }
        }
        
    }
}
