using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OneDevApp
{
    public class WrapLayout : MonoBehaviour
    {
        [SerializeField]
        private InputField m_ItemInput;

        [SerializeField]
        private GameObject m_CloseableTextPrefab;

        [SerializeField]
        private Transform m_WarpLayoutParentGO;

        public Transform WarpLayoutParentGO
        {

            get { return m_WarpLayoutParentGO; }
            private set { m_WarpLayoutParentGO = value; }
        }

        [SerializeField]
        private List<string> ItemsInList;

        public List<string> WarpLayoutItems
        {
            get { return ItemsInList; }
            set
            {
                ItemsInList = new List<string>(value);
                SetAllItem();
            }
        }

        public void SetAllItem()
        {
            foreach (Transform child in m_WarpLayoutParentGO)
            {
                Destroy(child.gameObject);
            }

            if (WarpLayoutItems.Count > 0)
            {
                foreach (string value in WarpLayoutItems)
                {
                    InstantiateItem(value);
                }
            }

            UpdateViews();

            m_ItemInput.text = string.Empty;
        }

        public void InsertItem()
        {
            if (m_ItemInput.text.Length <= 0) return;

            string value = m_ItemInput.text.ToUpper();

            if (WarpLayoutItems.Contains(value)) return;

            WarpLayoutItems.Add(value);
            InstantiateItem(value);
            UpdateViews();
            m_ItemInput.text = string.Empty;
        }

        void InstantiateItem(string value)
        {
            GameObject item = Instantiate(m_CloseableTextPrefab, m_WarpLayoutParentGO, false);
            item.transform.localScale = Vector3.one;
            item.GetComponent<WarpLayoutClosableText>().InitView(value);

            if (m_ItemInput.text.Length > 24)
                item.GetComponent<LayoutElement>().preferredWidth = 290;
        }

        public void UpdateViews()
        {
            Canvas.ForceUpdateCanvases();
            WarpLayoutGroup flowGroup = GetComponentInChildren<WarpLayoutGroup>();
            flowGroup.enabled = false;
            flowGroup.enabled = true;
        }

        public void RemoveItem(string value)
        {
            WarpLayoutItems.Remove(value);
            UpdateViews();
        }
    }
}