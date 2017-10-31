using UnityEngine;

namespace Model
{
    public sealed class Effect : Entity
    {
        public GameObject GameObject;
        public long UnitId { get; set; }

        public override void Dispose()
        {
            if (this.Id == 0)
            {
                return;
            }

            base.Dispose();
        }
    }
}