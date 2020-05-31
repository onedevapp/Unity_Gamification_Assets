using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace OneDevApp
{
    public class RummyCardBehaviour : BaseDraggable
    {
        /// =========================== Variable ================================= ///

        #region Variable

        /// <summary>
        /// Prefab for space operation on card items.
        /// </summary>
        public GameObject dummyCardPrefab;

        #endregion

        /// =========================== Cache ================================= ///

        #region Cache
        /// <summary>
        /// GameObject for space operation, static to avoid multiple objects.
        /// </summary>
        public static GameObject dummyCardObj;

        /// <summary>
        /// Ref to cards.
        /// </summary>
        private RummyCardBehaviour previousCard, nextCard;

        /// <summary>
        /// Ref to cards parent layout.
        /// </summary>
        private Transform parentGroupLayout;

        /// <summary>
        /// Child count of parent transform for space operation on first and last cards.
        /// </summary>
        int childCount;

        #endregion

        /// =========================== System funtions ==================== ///

        #region System funcitons
        protected override void Awake()
        {
            base.Awake();

            /// Creating dummy card for space operation
            if (dummyCardObj == null)
            {
                dummyCardObj = Instantiate(dummyCardPrefab, canvas.transform);
                dummyCardObj.name = "DummyCard";
            }
        }

        #endregion

        /// <summary>
        /// OnItemDragBegin is second method called everytime when drag is about to start from OnBeginDrag method.
        /// </summary>
        protected override void OnItemDragBegin(PointerEventData eventData)
        {
            dummyCardObj.SetActive(true); //activate dummy card
            
            parentGroupLayout = draggingHandlerParent;

            ///set card alpha back to semi transparent
            this.GetComponent<CanvasGroup>().alpha = 0.75f;

            UpdateRefHolder(parentGroupLayout);
        }

        /// <summary>
        /// OnItemDragEnd is called everytime when drag is stop moving from OnEndDrag method.
        /// </summary>
        protected override void OnItemDragEnd(PointerEventData eventData)
        {
            /// On Drop, if parent transform is not an instance of BaseDroppable class then revert to original parent transform
            if (draggingHandler.GetComponentInParent<BaseDroppable>() == null)
            {
                UpdateParentHolder(draggingHandlerParent);
            }
        }

        /// <summary>
        /// OnDrag is called everytime when drag is moving continueosly from OnDrag method.
        /// </summary>
        public override void OnDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);

            if (Mathf.Abs(transform.position.y - dummyCardObj.transform.position.y) < 160)
            {
                CheckWithNextCard();    //check for next card
                CheckWithPreviousCard();    //check for previous card
            }
        }

        /// <summary>
        /// Check for next card
        /// </summary>
        void CheckWithNextCard()
        {
            if (nextCard != null)
            {
                if (transform.position.x > nextCard.transform.position.x)
                {
                    int index = nextCard.transform.GetSiblingIndex();
                    nextCard.transform.SetSiblingIndex(dummyCardObj.transform.GetSiblingIndex());
                    dummyCardObj.transform.SetSiblingIndex(index);

                    previousCard = nextCard;
                    if (index + 1 < childCount)
                    {
                        nextCard = parentGroupLayout.GetChild(index + 1).GetComponent<RummyCardBehaviour>();
                    }
                    else
                    {
                        nextCard = null;
                    }
                }
            }
        }

        /// <summary>
        /// Check for previous card
        /// </summary>
        void CheckWithPreviousCard()
        {
            if (previousCard != null)
            {
                if (transform.position.x < previousCard.transform.position.x)
                {
                    int index = previousCard.transform.GetSiblingIndex();
                    previousCard.transform.SetSiblingIndex(dummyCardObj.transform.GetSiblingIndex());
                    dummyCardObj.transform.SetSiblingIndex(index);

                    nextCard = previousCard;
                    if (index - 1 >= 0)
                    {
                        previousCard = parentGroupLayout.GetChild(index - 1).GetComponent<RummyCardBehaviour>();
                    }
                    else
                    {
                        previousCard = null;
                    }
                }
            }
        }

        /// <summary>
        /// Updates the data card.
        /// </summary>
        /// <param name="param">Parameter.</param>
        public override void UpdateParentHolder(Transform parent)
        {
            base.UpdateParentHolder(parent);

            ///set selectedCard SetSiblingIndex at
            if (Mathf.Abs(transform.position.y - dummyCardObj.transform.position.y) < 160)
            {
                ///select transform child position
                transform.SetSiblingIndex(dummyCardObj.transform.GetSiblingIndex());

                ///Disable parent object if no child is available
                if (draggingHandlerParent.childCount == 0) draggingHandlerParent.gameObject.SetActive(false);
            }
            else
            {
                ///revert to original position
                this.transform.transform.SetSiblingIndex(ChildIndex);
            }

            dummyCardObj.transform.SetParent(canvas.transform);

            dummyCardObj.SetActive(false);    //Deactivate Dummy card

            ///set card alpha back to 1
            this.GetComponent<CanvasGroup>().alpha = 1f;

            parentGroupLayout = null;
        }

        /// <summary>
        /// Updates the new parent group on mouse pointing.
        /// </summary>
        /// <param name="param">Parameter.</param>
        public void UpdateRefHolder(Transform newParentGroupLayout)
        {
            if (parentGroupLayout != null)
            {
                int _childIndex = ChildIndex;

                ///Check for whether its new group 
                if (parentGroupLayout != newParentGroupLayout)
                {
                    int _tempCurrentGroupLayoutIndex = parentGroupLayout.transform.GetSiblingIndex();
                    int _tempNewGroupLayoutIndex = newParentGroupLayout.transform.GetSiblingIndex();

                    ///When card moving to right parent, then -1 will be index else 1
                    _childIndex = _tempCurrentGroupLayoutIndex - _tempNewGroupLayoutIndex;

                    if (_childIndex <= 0)
                    {
                        ///If mvoing right, then set dummy card at 0 position
                        _childIndex = 0;
                    }
                    else
                    {
                        ///If mvoing left, then set dummy card at last position
                        _childIndex = newParentGroupLayout.childCount;
                    }

                    parentGroupLayout = newParentGroupLayout;
                }

                ///set dummy card parent
                if (dummyCardObj.transform.parent != parentGroupLayout)
                {
                    dummyCardObj.transform.SetParent(parentGroupLayout); 
                }
                
                ///set dummy card index
                dummyCardObj.transform.SetSiblingIndex(_childIndex);   

                childCount = parentGroupLayout.childCount;

                if (childCount <= 1) return;

                ///check if selectedIndex is less than total childCount
                if (_childIndex + 1 < childCount)
                {
                    ///set the next card of the selected card
                    nextCard = parentGroupLayout.GetChild(_childIndex + 1).GetComponent<RummyCardBehaviour>();
                }

                if (_childIndex - 1 >= 0)
                {
                    ///set the previous card of selected card
                    previousCard = parentGroupLayout.GetChild(_childIndex - 1).GetComponent<RummyCardBehaviour>();
                }
            }
        }
    }

}