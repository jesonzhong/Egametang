using System;
using Model;
using UnityEngine;
using UnityEngine.UI;

namespace Hotfix
{
	[ObjectEvent]
	public class UIBattleMainComponentEvent : ObjectEvent<UIBattleMainComponent>, IAwake,IUpdate
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
	
	public class UIBattleMainComponent: Component
	{
		private GameObject JoyStickBg;
        private GameObject JoyStickBtn;
		private GameObject CommonAttack;
        private GameObject Skill1;
        private GameObject Skill2;
        private GameObject Skill3;
		private GameObject Controller;
		private ControlStick m_ControlStick;
        public void Awake()
		{
			ReferenceCollector rc = this.GetEntity<UI>().GameObject.GetComponent<ReferenceCollector>();
            CommonAttack = rc.Get<GameObject>("CommonAttack");
            if (CommonAttack != null)
            {
                CommonAttack.GetComponent<Button>().onClick.Add(OnCommonAttack);
            }
            
            Skill1 = rc.Get<GameObject>("Skill1");
            Skill1.GetComponent<Button>().onClick.Add(OnSkill1);
            Skill2 = rc.Get<GameObject>("Skill2");
            Skill2.GetComponent<Button>().onClick.Add(OnSkill2);
            Skill3 = rc.Get<GameObject>("Skill3");
            Skill3.GetComponent<Button>().onClick.Add(OnSkill3);

            JoyStickBg = rc.Get<GameObject>("ControllerBG");
            JoyStickBtn = rc.Get<GameObject>("ControllerButton");

			Controller = rc.Get<GameObject>("Controller");

			m_ControlStick = Controller.GetComponent<ControlStick>();
		}

		public void Update()
		{
			m_ControlStick.LogicUpdate();
		}

		/*public void PlayAnimation(string actName)
		{
		    Unit unit = Game.Scene.GetComponent<UnitComponent>().Get(PlayerComponent.Instance.MyPlayer.UnitId);
		    var animatorComponent = unit.GetComponent<AnimatorComponent>();
		    if (animatorComponent != null)
		    {
		        animatorComponent.Play(actName);
		    }
		}*/

		void ClickAction(int action)
		{
			SessionComponent.Instance.Session.Send( new Frame_ClickAction() { ActionID = action});
		}

		public void OnCommonAttack()
        {
            Log.Debug("OnCommonAttack");
	        ClickAction(1000);
        }

        public void OnSkill1()
        {
	        ClickAction(1001);
            Log.Debug("OnSkill1");
        }

        public void OnSkill2()
        {
	        ClickAction(1002);
            Log.Debug("OnSkill2");
        }

        public void OnSkill3()
        {
	        ClickAction(1003);
            Log.Debug("OnSkill3");
        }

	}
}
