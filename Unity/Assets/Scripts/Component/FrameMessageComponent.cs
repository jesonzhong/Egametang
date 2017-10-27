using UnityEngine;

namespace Model
{
    [ObjectEvent]
    public class FrameMessageComponentEvent : ObjectEvent<FrameMessageComponent>, IUpdate, IAwake
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

    public class FrameMessageComponent : Component
    {
        private float FrameMaxTime = 20;
        private float FrameTimeTotal = 0;
        public void Awake()
        {
            
        }

        public void Update()
        {
            FrameTimeTotal += Time.deltaTime;
            
        }

        public void AddMessage(AMessage message)
        {
            
        }
    }
}