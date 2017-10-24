using System;
using Model;
using UnityEngine;
using UnityEngine.UI;

namespace Hotfix
{
	[ObjectEvent]
	public class UIBattleMainComponentEvent : ObjectEvent<UIBattleMainComponent>, IAwake
	{
		public void Awake()
		{
			this.Get().Awake();
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

        }

        public void PlayAnimation(string actName)
        {
            Unit unit = Game.Scene.GetComponent<UnitComponent>().Get(PlayerComponent.Instance.MyPlayer.UnitId);
            var animatorComponent = unit.GetComponent<AnimatorComponent>();
            if (animatorComponent != null)
            {
                animatorComponent.Play(actName);
            }
        }

        public void OnCommonAttack()
        {
            Log.Debug("OnCommonAttack");
            PlayAnimation("attack_2");
        }

        public void OnSkill1()
        {
            PlayAnimation("skill_1");
            Log.Debug("OnSkill1");
        }

        public void OnSkill2()
        {
            PlayAnimation("skill_2");
            Log.Debug("OnSkill2");
        }

        public void OnSkill3()
        {
            PlayAnimation("powerattack_2");
            Log.Debug("OnSkill3");
        }

	}
}
