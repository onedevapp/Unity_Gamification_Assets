using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OneDevApp
{
    public class RummyMainLayout : BaseDroppable
    {
        /// =========================== Variable ================================= ///

        #region Variable
        /// <summary>
        /// Prefab for new group layout.
        /// </summary>
        public GameObject groupLayoutPrefab;

        /// <summary>
        /// No of max groups can create
        /// </summary>
        public int MaxGroupLayout = 4;

        /// <summary>
        /// Ref to all group layout created
        /// </summary>
        private List<GameObject> groupLayouts = new List<GameObject>();

        #endregion

        /// =========================== System funtions ==================== ///

        #region System funcitons
        private void Awake()
        {
            for (int i = 0; i < MaxGroupLayout; i++)
            {
                GameObject groupLayout = Instantiate(groupLayoutPrefab, this.transform.GetChild(0));
                groupLayouts.Add(groupLayout);

                groupLayout.SetActive(false);
            }
        }

        #endregion

        /// =========================== Callback funtions ==================== ///

        #region Callback functions
        /// <summary>
        /// Called everytime when drag is started
        /// Enables new group layout for card separation
        /// </summary>
        public override void OnAnyItemDragStart(BaseDraggable item)
        {
            if (item.draggingHandlerParent.childCount == 1) return;

            GameObject groupLayout = groupLayouts.Where(x => !(x.activeSelf)).FirstOrDefault();
            if (groupLayout != null)
            {
                groupLayout.SetActive(true);
                groupLayout.transform.SetAsLastSibling();
            }
        }

        /// <summary>
        /// Called everytime when drag is stop moving
        /// Disables new group layout or any layout with no child
        /// </summary>
        public override void OnAnyItemDragEnd(BaseDraggable item)
        {
            foreach (GameObject groupLayout in groupLayouts)
            {
                if (groupLayout.transform == item.draggingHandlerParent) continue;

                if (groupLayout.transform.childCount == 0) groupLayout.SetActive(false);
            }
        }

        #endregion

        /// <summary>
        /// OnDrop is called everytime when drag is stop moving and before OnEndDrag is called.
        /// </summary>
        public override void OnDrop(PointerEventData eventData)
        {
        }
    }
}