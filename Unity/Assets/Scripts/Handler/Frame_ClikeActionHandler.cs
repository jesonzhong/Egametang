using UnityEngine;

namespace Model
{
    [MessageHandler((int)Opcode.Frame_ClickAction)]
    public class Frame_ClikeActionHandler : AMHandler<Frame_ClickAction>
    {
        protected override void Run(Frame_ClickAction message)
        {
            Unit unit = Game.Scene.GetComponent<UnitComponent>().Get(message.Id);
            AnimatorComponent animatorComponent = unit.GetComponent<AnimatorComponent>();
            if (animatorComponent != null)
            {
                string actName = "attack_2";
                string effectName = "Effects/EffectCommonAttack";
                switch (message.ActionID)
                {
                        case 1000:
                            actName = "attack_2";
                            effectName = "Effects/EffectCommonAttack";
                            break;
                        case 1001:
                            actName = "skill_1";
                            effectName = "Effects/EffectSkill1";
                            break;
                        case 1002:
                            actName = "skill_2";
                            effectName = "Effects/EffectSkill3";
                            break;
                        case 1003:
                            actName = "skill_3";
                            effectName = "Effects/EffectSkill2";
                            break;
                }
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
                    if (message.ActionID == 1001)
                    {
                        hurtDis = 15;
                        hurt = 0.1f;
                    }
                    if (message.ActionID == 1002)
                    {
                        hurtDis = 20;
                        hurt = 0.15f;
                    }
                    if (message.ActionID == 1003)
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