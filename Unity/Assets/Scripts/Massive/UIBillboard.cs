using UnityEngine;

namespace TanksMP
{
    [ExecuteInEditMode]
    public class UIBillboard : MonoBehaviour
    {
        private Transform camTrans;
        
        private Transform trans;

        void OnEnable()
        {
            if (Camera.main != null)
            {
                camTrans = Camera.main.transform;
                trans = transform;
            }
        }

        void LateUpdate()
        {
            if (camTrans != null)
            {
                transform.LookAt(trans.position + camTrans.rotation * Vector3.forward, camTrans.rotation * Vector3.up);
            }
        }
    }
}