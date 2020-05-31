using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace OneDevApp
{
    public class CardFilpBehaviour : MonoBehaviour, IPointerClickHandler
    {
        /// =========================== Variable ================================= ///

        #region Variable
        /// <summary>
        /// The is card unlock.
        /// </summary>
        public bool IsCardUnlocked;

        /// <summary>
        /// Front of card
        /// </summary>
        public GameObject mFront;

        /// <summary>
        /// Back of card
        /// </summary>
        public GameObject mBack;

        /// <summary>
        /// The time to flip the card
        /// </summary>
        public float mTime = 0.2f;

        /// <summary>
        /// true represents that the flip is being performed and must not be interrupted
        /// </summary>
        private bool isActive = false;

        #endregion

        /// =========================== System funtions ==================== ///

        #region System funcitons
        private void Start()
        {
            if (IsCardUnlocked)
            {
                /// If you start from the front, rotate the back 90 degrees so that you can't see the back.
                mFront.transform.eulerAngles = Vector3.zero;
                mBack.transform.eulerAngles = new Vector3(0, 90, 0);
            }
            else
            {
                /// Starting from the back, the same thing
                mFront.transform.eulerAngles = new Vector3(0, 90, 0);
                mBack.transform.eulerAngles = Vector3.zero;
            }
        }

        #endregion

        /// =========================== Helper funtions ==================== ///

        #region Helper functions
        /// <summary>
        /// Turn over to the back
        /// </summary>
        public IEnumerator ToBack()
        {
            if (!isActive)
            {
                isActive = true;
                mFront.transform.DORotate(new Vector3(0, 90, 0), mTime);
                for (float i = mTime; i >= 0; i -= Time.deltaTime)
                    yield return 0;
                mBack.transform.DORotate(new Vector3(0, 0, 0), mTime);
                isActive = false;
            }

        }

        /// <summary>
        /// Turn to the front
        /// </summary>
        public IEnumerator ToFront()
        {
            if (!isActive)
            {
                isActive = true;
                mBack.transform.DORotate(new Vector3(0, 90, 0), mTime);
                for (float i = mTime; i >= 0; i -= Time.deltaTime)
                    yield return 0;
                mFront.transform.DORotate(new Vector3(0, 0, 0), mTime);
                isActive = false;
            }
        }

        /// <summary>
        /// Unlocks the card.
        /// </summary>
        /// <param name="IsUnlock">If set to <c>true</c> is unlock.</param>
        public void UnlockCard(bool IsUnlock)
        {
            if (IsUnlock)
            {
                StartCoroutine(ToFront());
            }
            else
            {
                StartCoroutine(ToBack());
            }
        }
        #endregion

        /// =========================== Callback funtions ==================== ///

        #region Callback functions
        public void OnPointerClick(PointerEventData eventData)
        {
            IsCardUnlocked = !IsCardUnlocked;
            UnlockCard(IsCardUnlocked);
        }
        #endregion
    }

}