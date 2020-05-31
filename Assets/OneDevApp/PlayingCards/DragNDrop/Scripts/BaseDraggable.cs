using UnityEngine;
using UnityEngine.EventSystems;

namespace OneDevApp
{
    public abstract class BaseDraggable : MonoBehaviour, IInitializePotentialDragHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {

        /// ============================= Delegates & Events ============================ ///

        #region Delegates & Events

        /// <summary>
        /// The delegate for card drag item.
        /// </summary>
        public delegate void DragEvent(BaseDraggable item);

        /// <summary>
        /// The event for card on start dragging
        /// </summary>
        public static event DragEvent OnItemDragStartEvent;

        /// <summary>
        /// The event for card on end dragging
        /// </summary>
        public static event DragEvent OnItemDragEndEvent;

        #endregion

        /// =========================== Cache ================================= ///

        #region Cache

        /// <summary>
        /// The pointer for position.
        /// </summary>
        protected Vector3 pointer;

        /// <summary>
        /// The position update.
        /// </summary>
        Vector3 positionUpdate;

        /// <summary>
        /// The drag item transform.
        /// </summary>
        internal Transform draggingHandler;

        /// <summary>
        /// The drag item parent transform to revert.
        /// </summary>
        internal Transform draggingHandlerParent;

        /// <summary>
        /// The drag offset position.
        /// </summary>
        internal Vector3 offset;

        /// <summary>
        /// The drag item position.
        /// </summary>
        [HideInInspector]
        public int ChildIndex = -1;

        /// <summary>
        /// Canvas for drag operation, static to avoid multiple objects.
        /// </summary>
        internal static Canvas canvas;

        /// <summary>
        /// Drag canvas gameobject name
        /// </summary>
        private string canvasName = "DragCanvas";

        /// <summary>
        /// Drag canvas sort order.
        /// </summary>
        private int canvasSortOrder = 100;

        /// <summary>
        /// Main canvas render for drag item.
        /// </summary>
        private RenderMode GameCanvasRenderMode;

        #endregion

        /// <summary>
        /// Awake this instance.
        /// </summary>
        protected virtual void Awake()
        {
            /// Assinging main canvas render mode.
            Canvas parentCanvas = GetComponentInParent<Canvas>();
            GameCanvasRenderMode = parentCanvas.renderMode;

            ///Creating dummy drag canvas
            if (canvas == null)
            {
                GameObject canvasObj = new GameObject(canvasName);
                canvas = canvasObj.AddComponent<Canvas>();

                if (GameCanvasRenderMode == RenderMode.ScreenSpaceOverlay)
                {
                    canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                }
                else
                {
                    canvas.renderMode = RenderMode.ScreenSpaceCamera;
                    canvas.worldCamera = Camera.main;
                }

                canvas.sortingOrder = canvasSortOrder;
            }
        }

        /// =========================== DragHandler ==================== ///

        #region DragHandler
        /// <summary>
        /// OnInitializePotentialDrag is first method called everytime when drag is initiated.
        /// offset value is used while render mode is screen space overlay only
        /// </summary>
        public virtual void OnInitializePotentialDrag(PointerEventData eventData)
        {
            if (GameCanvasRenderMode == RenderMode.ScreenSpaceOverlay)
                offset = gameObject.transform.position - new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f);

            ///save the selected card SiblingIndex [Child Index for its parent]
            ChildIndex = transform.GetSiblingIndex();
        }

        /// <summary>
        /// OnBeginDrag is second method called everytime when drag is about to start.
        /// </summary>
        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.pointerDrag == null) return;  //if Selected Card is not null

            if (GameCanvasRenderMode == RenderMode.ScreenSpaceOverlay)
            {
                draggingHandler = this.transform;

                draggingHandlerParent = draggingHandler.parent;

                draggingHandler.SetParent(canvas.transform);
            }
            else
            {
                pointer = GetWorldPosition();

                pointer.x = transform.position.x - pointer.x;

                pointer.y = transform.position.y - pointer.y;

                pointer.z = 0;

                draggingHandler = this.transform;

                draggingHandlerParent = draggingHandler.parent;

                draggingHandler.SetParent(canvas.transform);
            }

            ///Invoke OnItemDragBegin method
            OnItemDragBegin(eventData);

            GetComponent<CanvasGroup>().blocksRaycasts = false;

            /// Notify all items about drag start for raycast disabling
            OnItemDragStartEvent?.Invoke(this);
        }

        /// <summary>
        /// OnDrag is called everytime when drag is moving continueosly.
        /// </summary>
        public virtual void OnDrag(PointerEventData eventData)
        {
            if (eventData.pointerDrag == null) return;

            if (GameCanvasRenderMode == RenderMode.ScreenSpaceOverlay)
            {
                //Vector3 newPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f);

                draggingHandler.position = Input.mousePosition + offset;
            }
            else
            {
                positionUpdate = GetWorldPosition();

                positionUpdate.x = positionUpdate.x + pointer.x;

                positionUpdate.y = positionUpdate.y + pointer.y;

                positionUpdate.z = 0;

                draggingHandler.position = positionUpdate;
            }

        }

        /// <summary>
        /// OnEndDrag is called everytime when drag is stop moving.
        /// </summary>
        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (eventData.pointerDrag == null) return;

            /// Notify all cells about item drag end
            OnItemDragEndEvent?.Invoke(this);

            ///Invoke OnItemDragEnd method
            OnItemDragEnd(eventData);

            GetComponent<CanvasGroup>().blocksRaycasts = true;

            draggingHandler = null; //make draggingHandler null
        }

        /// <summary>
        /// Updates the card parent.
        /// </summary>
        /// <param name="param">Parameter.</param>
        public virtual void UpdateParentHolder(Transform parent)
        {
            this.transform.SetParent(parent, false);
            this.transform.localScale = Vector3.one;
        }

        protected abstract void OnItemDragBegin(PointerEventData eventData);
        protected abstract void OnItemDragEnd(PointerEventData eventData);

        #endregion

        /// <summary>
        /// Gets the world position.
        /// </summary>
        /// <returns>The world position.</returns>
        public static Vector3 GetWorldPosition()
        {
            Vector3 valueReturn = Input.mousePosition;

            valueReturn = Camera.main.ScreenToWorldPoint(valueReturn);

            return valueReturn;
        }
    }

}