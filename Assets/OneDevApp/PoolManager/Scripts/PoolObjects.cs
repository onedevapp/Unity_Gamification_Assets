using UnityEngine;

namespace OneDevApp
{
    public class PoolObjects : MonoBehaviour
    {
        //is this pool object active or not
        public bool active; 

        // Disables a pool object.
        public void DisablePoolObject()
        {
            this.active = false;
            this.gameObject.SetActive(false);
        }

        // Enables a pool object.
        public void ActivatePoolObject()
        {
            this.active = true;
            this.gameObject.SetActive(true);
        }
    }

}