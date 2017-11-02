using System;
using Model;
using UnityEngine;

namespace Hotfix
{
    [UIFactory((int)UIType.Rooms)]
    public class UIRoomsFactory : IUIFactory
    {
        public UI Create(Scene scene, UIType type, GameObject gameObject)
        {
            try
            {
                GameObject bundleGameObject = ((GameObject)Resources.Load("UI")).Get<GameObject>("UIRooms");
                GameObject uirooms = UnityEngine.Object.Instantiate(bundleGameObject);
                uirooms.layer = LayerMask.NameToLayer(LayerNames.UI);
                UI ui = new UI(scene, type, null, uirooms);

                ui.AddComponent<UIRoomsComponent>();
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