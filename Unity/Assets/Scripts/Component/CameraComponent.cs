using UnityEngine;

namespace Model
{
	[ObjectEvent]
	public class CameraComponentEvent : ObjectEvent<CameraComponent>, IAwake, IUpdate,ILateUpdate
	{
		public void Awake()
		{
			this.Get().Awake();
		}

		public void Update()
		{
			this.Get().Update();
		}

		public void LateUpdate()
		{
			this.Get().LateUpdate();
		}
	}

	public class CameraComponent : Component
	{
		// 战斗摄像机
		public Camera mainCamera;

		public Unit Unit;
		
		public Vector3 CameraPos = new Vector3 (0, 7, -4);
		public Vector3 CameraLookAtOffset = new Vector3 (0, 1, 0);
		public Camera MainCamera
		{
			get
			{
				return this.mainCamera;
			}
		}

		public void Awake()
		{
			this.mainCamera = Camera.main;
		}

		public void Update()
		{
			//UpdatePosition();
		}

		public void LateUpdate()
		{
			// 摄像机每帧更新位置
			//UpdatePosition();
		}

		public void UpdatePosition()
		{
			if (this.Unit != null)
			{
				
				this.mainCamera.transform.position = this.Unit.Position + CameraPos;
				//this.mainCamera.transform.LookAt (this.Unit.Position + CameraLookAtOffset);
				
				/*Vector3 cameraPos = this.mainCamera.transform.position;
                Vector3 targetPos = new Vector3(this.Unit.Position.x, cameraPos.y, this.Unit.Position.z - 4);
					
                //相机位置
                this.mainCamera.transform.position = Vector3.Lerp(cameraPos, targetPos, Time.deltaTime);*/
			}
		}
	}
}
