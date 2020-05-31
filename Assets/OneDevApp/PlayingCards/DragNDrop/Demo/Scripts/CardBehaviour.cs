using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace OneDevApp
{
    public class CardBehaviour : BaseDraggable
    {

        /// <summary>
        /// Called everytime when drag is about to start.
        /// Use this function if you dont overriding OnBeginDrag
        /// Useful when drag item about validate to drag or not
        /// </summary>
        protected override void OnItemDragBegin(PointerEventData eventData)
        {
        }

        /// <summary>
        /// Called everytime when drag is stop moving.
        /// Use this function if you dont overriding OnEndDrag
        /// Useful when drag item is succesfully dropped or not
        /// </summary>
        protected override void OnItemDragEnd(PointerEventData eventData)
        {
            if (draggingHandler.GetComponentInParent<BaseDroppable>() == null)
            {
                UpdateParentHolder(draggingHandlerParent);
                this.transform.SetSiblingIndex(ChildIndex);
            }
        }
    }

}