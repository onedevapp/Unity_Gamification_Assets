using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OneDevApp
{
    public class ScrollableText : MonoBehaviour
    {
        [SerializeField] private ScrollRect scrollRect = null;
        [SerializeField] private RectTransform container = null;
        [SerializeField] private bool scrollToTop = true;
        [SerializeField] private float ScrollDuration = 12f;

        private float currentPos = 0f;
        private IEnumerator coroutine;

        private void OnEnable()
        {
            coroutine = AutoScroll();
            StartCoroutine(coroutine);
        }


        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                StopCoroutine(coroutine);
            }
        }

        private IEnumerator AutoScroll()
        {
            yield return new WaitForEndOfFrame(); // wait for the next frame before continuing the loop

            float startPos = 0f;
            float endPos = 0f;
            LayoutRebuilder.ForceRebuildLayoutImmediate(container);

            if (scrollToTop)
            {
                startPos = 1f;
                endPos = 0f;
                if (currentPos > 0) startPos = currentPos;
            }
            else
            {
                startPos = 0f;
                endPos = 1f;
                if (currentPos > 0) endPos = currentPos;
            }

            // keep track of when the scroll started, when it should finish, and how long it has been running
            var startTime = Time.time;
            var endTime = Time.time + ScrollDuration;
            var elapsedTime = 0f;

            // set the currentPos to the start position
            currentPos = startPos;

            scrollRect.verticalNormalizedPosition = currentPos;

            yield return new WaitForSeconds(2f); // wait for the x sec before continuing the loop

            // loop repeatedly until the previously calculated end time
            while (Time.time <= endTime)
            {
                elapsedTime = Time.time - startTime; // update the elapsed time
                var percentage = 1 / (ScrollDuration / elapsedTime); // calculate how far along the timeline we are
                if (startPos > endPos) // if we are scroll top to bottom
                {
                    currentPos = startPos - percentage; // calculate the new position
                }
                else // if we are scroll bottom to top
                {
                    currentPos = startPos + percentage; // calculate the new position
                }

                scrollRect.verticalNormalizedPosition = currentPos;
                yield return new WaitForEndOfFrame(); // wait for the next frame before continuing the loop
            }
        }

    }

}