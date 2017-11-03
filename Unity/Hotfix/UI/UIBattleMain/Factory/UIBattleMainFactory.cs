using System;
using Model;
using UnityEngine;

namespace Hotfix
{
    [UIFactory((int)UIType.BattleMain)]
    public class UIBattleMainFactory : IUIFactory
    {
        public UI Create(Scene scene, UIType type, GameObject gameObject)
        {
	        try
	        {
				GameObject bundleGameObject = ((GameObject)Resources.Load("UI")).Get<GameObject>("UIBattleMain");
				GameObject battleMain = UnityEngine.Object.Instantiate(bundleGameObject);
                battleMain.layer = LayerMask.NameToLayer(LayerNames.UI);
				UI ui = EntityFactory.Create<UI, Scene, UI, GameObject>(scene, null, battleMain);

				ui.AddComponent<UIBattleMainComponent>();
				return ui;
	        }
	        catch (Exception e)
	        {
				Log.Error(e.ToStr());
		        return null;
	        }
		}
    }
}