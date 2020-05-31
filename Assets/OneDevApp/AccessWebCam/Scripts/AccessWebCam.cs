using System;
using UnityEngine;
using UnityEngine.UI;

namespace OneDevApp
{
    //Delegate for no of camera available
    public delegate void CameraDevicesDelegate(int _noofCameraDevices);

    public class AccessWebCam : MonoInstance<AccessWebCam>
    {
        #region Public variables
        // Event Declaration
        public event CameraDevicesDelegate OnCameraDevicesEvent;

        #endregion

        #region Private variables
        private int currentCamIndex = -1;           //Camera index to cycle among cameras
        private bool camAvailable = false;          //To update the aspect ratio and RawImage
        private bool forceFrontFacing = false;      //Enables only front facing camera
        private WebCamTexture cameraTexture;        //Webcam Texture to rendered the live video
        private RawImage cameraPreviewImage;        //RawImage to render the webcam Texture on UI
        private AspectRatioFitter aspectRatiofit;   //AspectRatio for the UI camera preview

        #endregion

        #region Public helper functions

        void OnEnable()
        {
            currentCamIndex = -1;
            camAvailable = false;
        }

        void OnDisable()
        {
            camAvailable = false;
            StopWebCam();
        }

        void Update()
        {
            if (!camAvailable)
                return;

            if (aspectRatiofit)
            {
                float ratio = (float)cameraTexture.width / (float)cameraTexture.height;
                aspectRatiofit.aspectRatio = ratio; // Set the aspect ratio
            }

            int orient = -cameraTexture.videoRotationAngle;
            cameraPreviewImage.rectTransform.localEulerAngles = new Vector3(0, 0, orient);


            float scaleY = cameraTexture.videoVerticallyMirrored ? -1f : 1f; // Find if the camera is mirrored or not
            cameraPreviewImage.rectTransform.localScale = new Vector3(1f, scaleY, 1f); // Swap the mirrored camera
        }

        #endregion

        #region Public helper functions

        /// <summary>
        /// Initialize webcam funtions 
        /// </summary>
        /// <param name="cameraPreviewImage">RawImage to view the camera texture</param>
        /// <param name="aspectRatiofit">AspectRatioFitter to fit the camera UI</param>
        /// <param name="forceFrontFacing">Front facing camera only</param>
        public void InitializeWebCam(RawImage cameraPreviewImage, AspectRatioFitter aspectRatiofit, bool forceFrontFacing = false)
        {
            this.cameraPreviewImage = cameraPreviewImage;
            this.aspectRatiofit = aspectRatiofit;
            this.forceFrontFacing = forceFrontFacing;

            // Start the camera 
            StopNStartWebcam();
        }

        /// <summary>
        /// Stop and Start the camera 
        /// </summary>
        public void StopNStartWebcam()
        {
            if (cameraTexture != null)
            {
                // stop the camera
                StopWebCam();
            }

            // Start the camera 
            StarteWebCam();
        }

        /// <summary>
        /// Stop the camera 
        /// </summary>
        private void StopWebCam()
        {
            if (cameraTexture != null && cameraTexture.isPlaying)
            {
                cameraPreviewImage.texture = null;
                cameraTexture.Stop();
                cameraTexture = null;
                camAvailable = false;
            }
        }

        /// <summary>
        /// Start the camera 
        /// </summary>
        private void StarteWebCam()
        {
            if (cameraPreviewImage == null)
                return;

            //List of all available devices
            WebCamDevice[] devices = WebCamTexture.devices;

            // event calling / invoking the delegates for no of available cameras
            OnCameraDevicesEvent(devices.Length);

            if (devices.Length == 0)
                return;
                        
            if (currentCamIndex == -1)
            {
                for (int i = 0; i < devices.Length; i++)
                {
                    var curr = devices[i];
                    if (forceFrontFacing && !curr.isFrontFacing)
                    {
                        continue;
                    }
                    Debug.Log("Camera Selected::" + curr.name);

                    this.currentCamIndex = i; // Current camera index

                    //WebCamTexture from the current camera device
                    cameraTexture = new WebCamTexture(curr.name, Screen.width, Screen.height);
                    break;
                }
            }
            else
            {
                var curr = devices[currentCamIndex];
                cameraTexture = new WebCamTexture(curr.name, Screen.width, Screen.height);
            }

            Debug.Log("StarteWebCam::currentCamIndex::after:: " + currentCamIndex);
            if (cameraTexture == null)
                return;

            cameraTexture.Play(); // Start the camera to play the video
            cameraPreviewImage.texture = cameraTexture; // Set the texture to UI

            camAvailable = true; // Set the camAvailable for future purposes.
        }


        /// <summary>
        /// Returns the bytes from the camera device
        /// </summary>
        /// <param name="forceCloseCamera">Close camera after capturing the image</param>
        public byte[] CaptureWebCam(bool forceCloseCamera = false)
        {
            byte[] bytes = null;

            try
            {
                Texture2D photo = new Texture2D(cameraTexture.width, cameraTexture.height);
                photo.SetPixels(cameraTexture.GetPixels());
                photo.Apply();
                bytes = photo.EncodeToJPG();
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }

            if (forceCloseCamera)
                StopWebCam();

            return bytes;
        }


        /// <summary>
        /// Switch between availalble camera devices
        /// </summary>
        public void SwitchWebCameras()
        {
            if (cameraTexture != null)
            {
                if (WebCamTexture.devices.Length > 0)
                {
                    if (forceFrontFacing)
                    {
                        WebCamDevice[] devices = WebCamTexture.devices;
                        while (true)
                        {
                            currentCamIndex += 1;
                            currentCamIndex %= WebCamTexture.devices.Length;
                            var curr = devices[currentCamIndex];
                            if (curr.isFrontFacing) break;
                        }
                    }
                    else
                    {
                        Debug.Log("SwitchWebCameras::currentCamIndex::before:: " + currentCamIndex);
                        currentCamIndex += 1;
                        currentCamIndex %= WebCamTexture.devices.Length;
                        Debug.Log("SwitchWebCameras::currentCamIndex::after:: " + currentCamIndex);
                    }

                    StopNStartWebcam();
                }
            }
        }

        /// <summary>
        /// Returns the current webcam texture
        /// </summary>
        public WebCamTexture GetCurrentWebCam()
        {
            return cameraTexture;
        }

        #endregion
    }

}