using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OneDevApp
{
    public class WarpLayoutClosableText : MonoBehaviour
    {
        [SerializeField]
        private Text ItemValue;

        public void InitView(string value)
        {
            ItemValue.text = value;
        }

        public void DeleteItem()
        {
            gameObject.SetActive(false);
            GetComponentInParent<WrapLayout>().RemoveItem(ItemValue.text);
            Destroy(this);
        }
    }

}