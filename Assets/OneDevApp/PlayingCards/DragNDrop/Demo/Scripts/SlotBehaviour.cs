using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace OneDevApp
{
    public class SlotBehaviour : BaseDroppable
    {

        /// <summary>
        /// Called everytime when drag is stop moving before OnEndDrag is called
        /// Useful to vaidate drop item
        /// </summary>
        public override void OnDrop(PointerEventData eventData)
        {
            GameObject droppedObject = eventData.pointerDrag;
            BaseDraggable item = droppedObject.GetComponent<BaseDraggable>();
            item.UpdateParentHolder(this.transform);
        }
    }

}