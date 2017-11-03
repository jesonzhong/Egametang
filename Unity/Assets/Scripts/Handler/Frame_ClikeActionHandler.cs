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
                switch (message.ActionID)
                {
                        case 1000:
                            animatorComponent.Play("attack_2");
                            break;
                        case 1001:
                            animatorComponent.Play("skill_1");
                            break;
                        case 1002:
                            animatorComponent.Play("skill_2");
                            break;
                        case 1003:
                            animatorComponent.Play("skill_3");
                            break;
                }
                
            }
        }
    }
}