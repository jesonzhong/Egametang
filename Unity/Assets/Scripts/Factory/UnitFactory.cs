using UnityEngine;

namespace Model
{
    public static class UnitFactory
    {
        public static Unit Create(long id)
        {
            Unit unit = UnitComponent.Instance.Get(id);
            if (unit == null)
            {
                //GameObject hudPrefab = AssetManager.Instance.GetPrefabFullPath("assets/bundles/characters/HUD", false);
                //GameObject prefab = AssetManager.Instance.GetPrefabFullPath("assets/bundles/characters/avatar/prefab104/Avatar_104", false);
                
                GameObject hudPrefab = Resources.Load("characters/HUD") as GameObject;
                GameObject prefab = Resources.Load("characters/avatar/prefab104/Avatar_104") as GameObject;
                
                unit = EntityFactory.CreateWithId<Unit>(id);
                unit.GameObject = PoolManager.Spawn(prefab, Vector3.zero, Quaternion.Euler(Vector3.zero));
                unit.HudGameObject = PoolManager.Spawn(hudPrefab, Vector3.zero, Quaternion.Euler(Vector3.zero));
                GameObject parent = GameObject.Find($"/Global/Unit");
                unit.GameObject.transform.SetParent(parent.transform, false);
                UIUtil.SetParent(unit.HudGameObject.transform, unit.GameObject.transform);
                unit.AddComponent<AnimatorComponent>();
                unit.AddComponent<MoveComponent>();
                unit.AddComponent<HUDComponent>();
                UnitComponent.Instance.Add(unit);
            }

            return unit;
        }
    }
}