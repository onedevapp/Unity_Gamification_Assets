using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace OneDevApp
{
    public class RectScreenShot : MonoBehaviour
    {
        #region Public variables
        [Tooltip("The RectTransform which need to captured if null, then Full screenshot will be capture")]
        public RectTransform rectTransform; // Assign the UI element which you wanna capture

        #endregion


        #region Private variables
        private int width; // width of the object to capture
        private int height; // height of the object to capture
        private Transform rectTransformParent;  //RectTransform parent to reasign after usage
        private GameObject screenShotGO;    //Tempory GameObject to copy recttransform

        //RectTransform values to reasign after usage
        private Vector2 offsetMinValues;
        private Vector2 offsetMaxValues;
        private Vector3 localScaleValues;
        #endregion

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (rectTransform != null)
                {
                    if (rectTransform.root.GetComponent<CanvasScaler>().uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize)
                    {
                        if (rectTransform.root.GetComponent<CanvasScaler>().screenMatchMode != CanvasScaler.ScreenMatchMode.MatchWidthOrHeight ||
                            rectTransform.root.GetComponent<CanvasScaler>().matchWidthOrHeight != 0.5f)
                        {
                            Debug.LogWarning("UI may not look the same due to Canvas Scaler either screenMatchMode was not set MatchWidthOrHeight or MatchWidthOrHeight is not set to 0.5f");
                        }
                        createCanvasWithRectTransform();
                    }
                    else if (rectTransform.root.GetComponent<CanvasScaler>().uiScaleMode == CanvasScaler.ScaleMode.ConstantPixelSize)
                    {
                        StartCoroutine(takeScreenShot());
                    }
                    else
                    {
                        Debug.LogWarning("Canvas Scaler mode not supported.");
                    }
                }
                else
                {
                    Debug.Log("Rect transform is null to capture the screenshot, hence fullscreen has been taken.");
                    ScreenCapture.CaptureScreenshot("FullPageScreenShot.png");
                }
            }

        }

        private void createCanvasWithRectTransform()
        {
            rectTransformParent = rectTransform.parent; //Assigning Parent transform to reasign after usage

            //Copying RectTransform values to reasign after switching parent
            offsetMinValues = rectTransform.offsetMin;
            offsetMaxValues = rectTransform.offsetMax;
            localScaleValues = rectTransform.localScale;

            //Creating secondary CANVAS with required fields
            screenShotGO = new GameObject("ScreenShotGO");
            screenShotGO.transform.parent = null;
            Canvas canvas = screenShotGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler canvasScalar = screenShotGO.AddComponent<CanvasScaler>();
            canvasScalar.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            screenShotGO.AddComponent<GraphicRaycaster>();

            rectTransform.SetParent(screenShotGO.transform);    //Assigning capture recttransform to temporary parent gameobject

            //Reasigning recttransform values
            rectTransform.offsetMin = offsetMinValues;
            rectTransform.offsetMax = offsetMaxValues;
            rectTransform.localScale = localScaleValues;

            Canvas.ForceUpdateCanvases();   // Forcing all canvas to update the UI

            StartCoroutine(takeScreenShot());   //Once everything was set ready, Capture the screenshot
        }

        private IEnumerator takeScreenShot()
        {
            yield return new WaitForEndOfFrame(); // it must be a coroutine 

            //Calcualtion for the width and height of the screenshot from recttransform
            width = System.Convert.ToInt32(rectTransform.rect.width);
            height = System.Convert.ToInt32(rectTransform.rect.height);

            //Calcualtion for the starting position of the recttransform to be captured
            Vector2 temp = rectTransform.transform.position;
            var startX = temp.x - width / 2;
            var startY = temp.y - height / 2;

            // Read the pixels from the texture
            var tex = new Texture2D(width, height, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(startX, startY, width, height), 0, 0);
            tex.Apply();

            // split the process up--ReadPixels() and the GetPixels() call inside of the encoder are both pretty heavy
            yield return 0;

            var bytes = tex.EncodeToPNG();
            Destroy(tex);

            //Writing bytes to a file
            File.WriteAllBytes(Application.dataPath + "/ScreenShotRect/ScreenShot.png", bytes);

            //In case of ScaleMode was not ScaleWithScreenSize, parent will not be assigned then no need to revert the changes
            if (rectTransformParent != null)
            {
                //Reasigning gameobject to its original parent group
                rectTransform.SetParent(rectTransformParent);

                //Reasigning recttransform values
                rectTransform.offsetMin = offsetMinValues;
                rectTransform.offsetMax = offsetMaxValues;
                rectTransform.localScale = localScaleValues;

                //Destorying temporary created gameobject after usage
                if (screenShotGO)
                    Destroy(screenShotGO);
            }

            Debug.Log("Picture taken");
        }

    }

}