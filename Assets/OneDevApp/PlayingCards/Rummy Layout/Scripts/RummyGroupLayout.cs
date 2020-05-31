using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace OneDevApp
{
    public class RummyGroupLayout :  BaseDroppable, IPointerEnterHandler
    {
        /// <summary>
        /// Called everytime when drag is stop moving before OnEndDrag is called
        /// Useful to vaidate drop item
        /// </summary>
        public override void OnDrop(PointerEventData eventData)
        {
            GameObject droppedObject = eventData.pointerDrag;

            /// On Drop, if parent transform is an instance of BaseDroppable class then update parent transform
            BaseDraggable item = droppedObject.GetComponent<BaseDraggable>();
            if(item != null)
                item.UpdateParentHolder(this.transform);
        }

        /// <summary>
        /// Called everytime when mouse enter this gameobject
        /// Its updates the card ref holders to newly pointing rummy group layout
        /// </summary>
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (eventData.pointerDrag == null) return;

            GameObject dragObject = eventData.pointerDrag;

            /// On pointing to this instance of BaseDroppable class then update the card ref holders
            RummyCardBehaviour item = dragObject.GetComponent<RummyCardBehaviour>();
            if (item != null)
                item.UpdateRefHolder(this.transform);
        }
    }
}