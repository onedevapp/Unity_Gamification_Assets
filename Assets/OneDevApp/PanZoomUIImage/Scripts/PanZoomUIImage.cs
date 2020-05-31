using UnityEngine;
using UnityEngine.EventSystems;

namespace OneDevApp
{
    /// <summary>
    /// To Zoom an image, we made some calculation and funtions below but to Pan an image, IScrollHandler will handle automatically.
    /// </summary>
    public class PanZoomUIImage : MonoBehaviour, IScrollHandler
    {
        #region Private variables

        // Initial scale of the UI image to restict zoom
        private Vector3 initialScale;

        [SerializeField]
        [Tooltip("The value which multiples on zoom")]
        [Range(0.0f, 1.0f)]
        private float zoomSpeed = 0.1f;
        [SerializeField]
        [Tooltip("The Max level of zoom")]
        [Range(0.0f, 10f)]
        private float maxZoom = 10f;

        #endregion


        private void Awake()
        {
            initialScale = transform.localScale;
        }
        
        /// <summary>
        /// This functions automatically calls from IScrollHandler while scrolling on MOUSE WHEEL, this doesn't works on Android or IOS
        /// </summary>
        public void OnScroll(PointerEventData eventData)
        {
            ChangeZoom(eventData.scrollDelta.y);
        }

        /// <summary>
        /// This works on both Android or IOS, detects touch movement to zoom
        /// </summary>
        private void Update()
        {
            if (Input.touchCount == 2)
            {
                // Store both touches.
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                // Find the position in the previous frame of each touch.
                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                // Find the magnitude of the vector (the distance) between the touches in each frame.
                float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                // Find the difference in the distances between each frame.
                float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
                                
                ChangeZoom(-deltaMagnitudeDiff);
            }
        }

        #region Public helper functions

        /// <summary>
        /// Calcualtion for zooming an image
        /// </summary>
        /// <param name="scrollWheel">zoom value</param>
        private void ChangeZoom(float scrollWheel)
        {
            var delta = Vector3.one * (scrollWheel * zoomSpeed);
            var desiredScale = transform.localScale + delta;

            desiredScale = ClampDesiredScale(desiredScale);

            transform.localScale = desiredScale;

        }

        /// <summary>
        /// Calucaltion to clamp the zoom
        /// </summary>
        /// <param name="desiredScale">vector3 to clamp the zoom</param>
        private Vector3 ClampDesiredScale(Vector3 desiredScale)
        {
            desiredScale = Vector3.Max(initialScale, desiredScale);
            desiredScale = Vector3.Min(initialScale * maxZoom, desiredScale);
            return desiredScale;
        }
        #endregion
    }

}