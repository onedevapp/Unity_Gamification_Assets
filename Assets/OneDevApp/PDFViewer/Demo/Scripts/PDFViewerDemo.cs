using OneDevApp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PDFViewerDemo : MonoBehaviour
{
    public Image pdfImageViewer;
    public Text PageCount;
    public GameObject errorText;
    public GameObject nextBtn;
    public GameObject prevBtn;

    private string folderPath = "temp/pdf";
    private string _fileName = "";
    private int currentImage = 0;
    private List<FileInfo> filesInfo;

    private void OnEnable()
    {
        PDFConvertManager.OnPdf2ImageConvertEvent += OnPdf2ImageConverted;
        
#if UNITY_EDITOR
        _fileName = Path.Combine(Application.dataPath, "Demo/PDFs/test.pdf");
#else
        _fileName = Path.Combine(Path.GetDirectoryName(Application.dataPath), "Demo/PDFs/test.pdf");
#endif

        PDFConvertManager.Instance.ConvertPDF2Image(_fileName);
    }

    private void OnDisable()
    {
        try
        {
            if (File.Exists(_fileName))
            {
                File.Delete(_fileName);
            }

            if (filesInfo != null)
            {
                foreach (FileInfo file in filesInfo)
                {
                    string imageFile = file.ToString();
                    if (File.Exists(imageFile))
                    {
                        File.Delete(imageFile);
                    }
                }

                filesInfo.Clear();
            }

            _fileName = string.Empty;

        }
        catch (Exception e)
        {
            Debug.Log("OnDisable:" + e);
        }
        PDFConvertManager.OnPdf2ImageConvertEvent -= OnPdf2ImageConverted;
    }

    private void OnPdf2ImageConverted(List<FileInfo> filesInfo)
    {
        if (filesInfo.Count > 0)
        {
            this.filesInfo = filesInfo;
            //assign 1st image to image view
            currentImage = 0;
            showPDFImage();
            errorText.SetActive(false);
            pdfImageViewer.gameObject.SetActive(true);
        }
        else
        {
            pdfImageViewer.gameObject.SetActive(false);
            errorText.SetActive(true);
        }
    }

    public void showPDFImage()
    {
        if (filesInfo.Count > currentImage)
        {
            string imageFile = filesInfo[currentImage].ToString();
            Debug.Log("showPDFImage:imageFile:" + imageFile);
            pdfImageViewer.sprite = LoadNewSprite(imageFile);
            
            nextBtn.gameObject.SetActive(filesInfo.Count > currentImage + 1);
            prevBtn.gameObject.SetActive(filesInfo.Count > currentImage + 1);

            string pagecount = (currentImage + 1) + "/" + (this.filesInfo.Count);
            PageCount.text = "Page :" + pagecount;            
        }
    }

    public void OnNext()
    {
        //show next pdf image with count
        StartCoroutine(NextPage());
    }

    IEnumerator NextPage()
    {
        yield return new WaitForSeconds(1f);
        currentImage++;
        showPDFImage();
        yield return null;
    }

    public void onPrev()
    {
        if (currentImage > 0)
        {
            StartCoroutine(PrevPage());
        }
    }

    IEnumerator PrevPage()
    {
        yield return new WaitForSeconds(1f);
        currentImage--;
        showPDFImage();
        yield return null;
    }

    public Sprite LoadNewSprite(string FilePath, float PixelsPerUnit = 100.0f)
    {

        // Load a PNG or JPG image from disk to a Texture2D, assign this texture to a new sprite and return its reference

        Sprite NewSprite;// = new Sprite();
        try
        {
            Texture2D SpriteTexture = LoadTexture(FilePath);
            NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0, 0), PixelsPerUnit);
        }
        catch (Exception e)
        {
            Debug.Log("LoadNewSprite::File not found " + FilePath);
            Debug.Log("LoadNewSprite::" + e);
            return null;
        }
        return NewSprite;
    }

    public Texture2D LoadTexture(string FilePath)
    {

        // Load a PNG or JPG file from disk to a Texture2D
        // Returns null if load fails

        Texture2D Tex2D;
        byte[] FileData;

        if (File.Exists(FilePath))
        {
            FileData = File.ReadAllBytes(FilePath);
            Tex2D = new Texture2D(2, 2);           // Create new "empty" texture
            if (Tex2D.LoadImage(FileData))           // Load the imagedata into the texture (size is set automatically)
                return Tex2D;                 // If data = readable -> return texture
        }
        return null;                     // Return null if load failed
    }
}
