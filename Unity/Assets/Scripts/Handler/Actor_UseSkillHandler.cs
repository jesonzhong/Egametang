using UnityEngine;

namespace Model
{
	[MessageHandler(Opcode.Response_UseSkill)]
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
                            actName = "skill_2";
                            effectName = "Effects/EffectSkill2";
                        }
                        break;
                    case 4:
                        {
                            actName = "powerattack_2";
                            effectName = "Effects/EffectSkill3";
                        }
                        break;
                }
                var animatorComponent = unit.GetComponent<AnimatorComponent>();
                if (animatorComponent != null)
                {
                    animatorComponent.Play(actName);

                    EffectManager.instance.AddFxAutoRemove(effectName, unit.GameObject.transform);
                }
            }
		}
	}
}
