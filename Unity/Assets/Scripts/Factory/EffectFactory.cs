using UnityEngine;

namespace Model
{
    public static class EffectFactory
    {
        public static Effect Create(long id, string effectName, Transform parent)
        {
            //UnitComponent unitComponent = Game.Scene.GetComponent<UnitComponent>();
                    
            Effect effectEntity = EntityFactory.CreateWithId<Effect>(id);
            if (effectEntity.GameObject != null)
            {
                effectEntity.GameObject.transform.SetParent(parent, false);
                effectEntity.GameObject.SetActive(false);
                effectEntity.GameObject.SetActive(true);
            }
            else
            {
                GameObject prefab = ((GameObject)Resources.Load(effectName));
                effectEntity.GameObject = UnityEngine.Object.Instantiate(prefab);
                effectEntity.GameObject.transform.SetParent(parent, false);
            }


            return effectEntity;
        }
    }
}