using OneDevApp;
using System;
using UnityEngine;
using UnityEngine.UI;

public class WebCamDemo : MonoBehaviour
{
    public RawImage background;
    public AspectRatioFitter fit;
    public Button captureImageBtn;
    public GameObject switchBtn;
    public Image scannedImage;
    public GameObject CameraPreviewGO;
    public GameObject ScannedImageGO;
    public Sprite cameraImage;
    public Sprite reloadImage;
    private bool IsToReload = true;
    public bool ShowCapturedPreview = true;

    // Use this for initialization
    void Start()
    {
        IsToReload = true;
        switchBtn.SetActive(false);
        Invoke("delayInti", 1f);
        AccessWebCam.Instance.OnCameraDevicesEvent += GetCameraDevices;
    }

    private void OnDisable()
    {
        AccessWebCam.Instance.OnCameraDevicesEvent -= GetCameraDevices;
        CameraPreviewGO.SetActive(true);
        ScannedImageGO.SetActive(false);
        scannedImage.sprite = null;
    }

    void delayInti()
    {
        OnCameraBtnClicked();
    }

    public void GetCameraDevices(int _noofCameraDevices)
    {
        switchBtn.SetActive(_noofCameraDevices > 1);
    }


    public void OnCameraSwitchBtnClicked()
    {
        AccessWebCam.Instance.SwitchWebCameras();
    }

    public void OnCameraBtnClicked()
    {
        if (IsToReload)
        {
            captureImageBtn.image.sprite = cameraImage;
            switchBtn.SetActive(true);
            InitializeViews();
        }
        else
        {
            captureImageBtn.image.sprite = reloadImage;
            switchBtn.SetActive(false);
            byte[] cameraBytes = AccessWebCam.Instance.CaptureWebCam(ShowCapturedPreview);

            if (ShowCapturedPreview)
            {
                CameraPreviewGO.SetActive(false);
                ScannedImageGO.SetActive(true);
                scannedImage.sprite = LoadNewSprite(cameraBytes);
            }
        }

        IsToReload = !IsToReload;
    }

    void InitializeViews()
    {
        CameraPreviewGO.SetActive(true);
        ScannedImageGO.SetActive(false);
        captureImageBtn.image.sprite = cameraImage;

        AccessWebCam.Instance.InitializeWebCam(background, fit);
    }

    public Sprite LoadNewSprite(byte[] FileData, float PixelsPerUnit = 100.0f)
    {
        // Load a PNG or JPG image from disk to a Texture2D, assign this texture to a new sprite and return its reference

        Sprite NewSprite = null;
        try
        {
            Texture2D SpriteTexture = new Texture2D(2, 2);
            if (SpriteTexture.LoadImage(FileData))
                NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0, 0), PixelsPerUnit);
        }
        catch (Exception e)
        {
            Debug.Log("LoadNewSprite:: " + e.ToString());
            return null;
        }
        return NewSprite;
    }
}
