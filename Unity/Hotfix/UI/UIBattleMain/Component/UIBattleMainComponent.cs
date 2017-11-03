using System;
using Model;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

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
        public static UIBattleMainComponent Instance { get; private set; }

        //for test
        private List<Effect> m_effects = new List<Effect>();

		private GameObject JoyStickBg;
        private GameObject JoyStickBtn;
		private GameObject CommonAttack;
        private GameObject Skill1;
        private GameObject Skill2;
        private GameObject Skill3;
		private GameObject Controller;
		private ControlStick m_ControlStick;
        //榜单
        private List<GameObject> rankList = new List<GameObject>();
        private List<Text> nameList = new List<Text>();
        private List<Text> scoreList = new List<Text>();


        //小地图
        private List<GameObject> imgList = new List<GameObject>();


        public void Awake()
		{
            Instance = this;

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

            //榜单
            GameObject gridGroup = rc.Get<GameObject>("Grid_Group");
            for (int i=1; i<= 10; i++)
            {
                string cellName = "cell_" + i;
                GameObject cell = gridGroup.transform.Find(cellName).gameObject;
                if (cell != null)
                {
                    rankList.Add(cell);
                    Text name = cell.transform.Find("name").GetComponent<Text>();
                    nameList.Add(name);
                    name.text = "张三" + i;
                    Text score = cell.transform.Find("score").GetComponent<Text>();
                    score.text = (100+ i* 10).ToString();
                    scoreList.Add(score);
                }
            }

            //小地图
            GameObject mapObj = rc.Get<GameObject>("map");
            for (int i = 1; i <= 10; i++)
            {
                string cellName = "point/img" + i;
                GameObject imgObj = mapObj.transform.Find(cellName).gameObject;
                imgList.Add(imgObj);

            }


        }

        public void Update()
        {
            m_ControlStick.LogicUpdate();
            ProcessMapPos();
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

        public void PlayEffect(string effectName, long id = 0)
        {
            Unit unit = Game.Scene.GetComponent<UnitComponent>().Get(PlayerComponent.Instance.MyPlayer.UnitId);
            //Effect eff = EffectFactory.Create(unit.Id * 10 + id, effectName, unit.GameObject.transform);
            //m_effects.Add(eff);

            //DelayManager.instance.delay(5, () =>
            //{
            //    DisposeEffect();
            //});

            EffectManager.instance.AddFxAutoRemove(effectName, unit.GameObject.transform);
        }

        void DisposeEffect()
        {
            if (m_effects.Count > 0)
            {
                m_effects[0].GameObject.SetActive(false);
                m_effects[0].Dispose();
                m_effects.RemoveAt(0);
            }
        }

	    void ClickAction(int ActionID)
	    {
	        SessionComponent.Instance.Session.Send(new Frame_ClickAction() {ActionID = ActionID});
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

        public void ProcessRankList(Response_RankList message)
        {
            int i = 0;
            foreach (RankInfo unitInfo in message.Units)
            {
                if (i < 10)
                {
                    rankList[i].SetActive(true);
                    nameList[i].text = unitInfo.Id.ToString();
                    scoreList[i].text = unitInfo.score.ToString();
                }

                i++;
            }

            for (int j=i; j<10; ++j)
            {
                rankList[j].SetActive(false);
            }
        }

        //处理小地图位置
        public void ProcessMapPos()
        {
            Unit[] units = UnitComponent.Instance.GetAll();
            int i = 0;
            foreach (Unit u in units)
            {
                if (i < 10)
                {
                    imgList[i].SetActive(true);
                    Vector3 v3 = u.Position;
                    imgList[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(v3.x/200.0f*205.0f, v3.z/200.0f*205.0f, 0);
                }

                i++;
            }

            for (int j = i; j < 10; ++j)
            {
                imgList[j].SetActive(false);
            }
        }

	}
}
