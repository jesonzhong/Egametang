using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Model
{
	[ObjectEvent]
	public class HUDComponentEvent : ObjectEvent<HUDComponent>, IAwake, IUpdate
	{
		public void Awake()
		{
			this.Get().Awake();
		}

		public void Update()
		{
			this.Get().Update();
		}
	}

	public class HUDComponent : Component
	{
        private TextMesh nameTextMesh;
        private Slider3D hpSlider;

        protected float hpValue = 1.0f;

        public void Awake()
        {
            GameObject rootObject = this.GetEntity<Unit>().HudGameObject;

            nameTextMesh = rootObject.transform.Find("Canvas/Center/name").GetComponent<TextMesh>();
            //ID先显示在名字的位置
            nameTextMesh.text = this.GetEntity<Unit>().Id.ToString();

            hpSlider = rootObject.transform.Find("Canvas/Center/Slider").GetComponent<Slider3D>();
            hpSlider.sliderValue = 1.0f;
        }

        public void SetHpValue(float value)
        {
            if (hpValue != value)
            {
                hpSlider.sliderValue = value;
                hpValue = value;
            }
        }

        public void AddHpValue(float value)
        {
            hpValue += value;
            hpSlider.sliderValue = hpValue;
        }

        public void SubHpValue(float value)
        {
            hpValue -= value;
            hpSlider.sliderValue = hpValue;

            //直接满血。。
            if (hpValue < 0)
                hpValue = 1;
        }

        public void Update()
		{

		}


		public override void Dispose()
		{
			if (this.Id == 0)
			{
				return;
			}
			base.Dispose();
		}
	}
}