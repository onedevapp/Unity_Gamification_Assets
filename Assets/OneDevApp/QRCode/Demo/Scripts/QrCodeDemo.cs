using OneDevApp;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QrCodeDemo : MonoBehaviour
{
    public InputField mInputField;
    public Toggle mQrCodeType;
    public Image mQrCodeImage;


    public void onButtonClicked()
    {
        if (mQrCodeType)
        {
            if(!string.IsNullOrEmpty(mInputField.text))
                mQrCodeImage.sprite = Texture2Sprite(QRCodeManager.Instance.GenerateQRCode(mInputField.text));
        }
        else
        {
            string filePath = Application.persistentDataPath + "/qrcode.png";
            mInputField.text = QRCodeManager.Instance.DecodeQRValues(filePath);
        }
    }


    // Assign this texture to a new sprite and return its reference
    public Sprite Texture2Sprite(Texture2D SpriteTexture, float PixelsPerUnit = 100.0f)
    {
        Sprite NewSprite = null;
        try
        {
            NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0, 0), PixelsPerUnit);
        }
        catch (Exception e)
        {
            Debug.Log("Texture2Sprite:: " + e.ToString());
            return null;
        }

        return NewSprite;
    }
}
