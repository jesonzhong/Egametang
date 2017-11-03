using UnityEngine;

namespace Model
{
	[MessageHandler((int)Opcode.Response_UseSkill)]
	public class Response_UseSkillHandler : AMHandler<Response_UseSkill>
	{
		protected override void Run(Response_UseSkill message)
		{

			Unit unit = Game.Scene.GetComponent<UnitComponent>().Get(message.Id);
			if (unit != null)
            {
                string actName = "attack_2";
                string effectName = "Effects/EffectCommonAttack";
                switch (message.skillId)
                {
                    case 1:
                        {
                            actName = "attack_2";
                            effectName = "Effects/EffectCommonAttack";
                        }
                        break;
                    case 2:
                        {
                            actName = "skill_1";
                            effectName = "Effects/EffectSkill1";
                        }
                        break;
                    case 3:
                        {
                            actName = "powerattack_2";
                            effectName = "Effects/EffectSkill3";
                        }
                        break;
                    case 4:
                        {
                            actName = "skill_2";
                            effectName = "Effects/EffectSkill2";
                        }
                        break;

                    default:
                        break;

                }
                var animatorComponent = unit.GetComponent<AnimatorComponent>();
                if (animatorComponent != null)
                {
                    animatorComponent.Play(actName);

                    EffectManager.instance.AddFxAutoRemove(effectName, unit.GameObject.transform);
                }


                Vector3 srcPos = unit.Position;
                //计算伤害
                Unit[] units = UnitComponent.Instance.GetAll();
                foreach (Unit u in units)
                {
                    if (u.Id != message.Id)
                    {
                        Vector3 targetPos = u.Position;
                        float lengthSqure = (targetPos.x - srcPos.x) * (targetPos.x - srcPos.x) + (targetPos.z - srcPos.z) * (targetPos.z - srcPos.z);

                        int hurtDis = 10;
                        float hurt = 0.05f;
                        if (message.skillId == 2)
                        {
                            hurtDis = 15;
                            hurt = 0.1f;
                        }
                        if (message.skillId == 3)
                        {
                            hurtDis = 20;
                            hurt = 0.15f;
                        }
                        if (message.skillId == 4)
                        {
                            hurtDis = 25;
                            hurt = 0.2f;
                        }
                        if (lengthSqure < hurtDis)
                            u.GetComponent<HUDComponent>().SubHpValue(hurt);
                    }
                }

                
            }
		}
	}
}
