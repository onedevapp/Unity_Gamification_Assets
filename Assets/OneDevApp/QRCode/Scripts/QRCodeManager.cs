using System;
using System.IO;
using UnityEngine;
using ZXing;
using ZXing.QrCode;

namespace OneDevApp
{
    public class QRCodeManager : MonoInstance<QRCodeManager>
    {

        #region Public helper functions
        /// <summary>
        /// To decode qr code from webcam texture
        /// </summary>
        /// <param name="camTexture">webcam texure</param>
        public string DecodeQRValues(WebCamTexture camTexture)
        {
            return Decode(camTexture.GetPixels32(), camTexture.width, camTexture.height);
        }

        /// <summary>
        /// To decode qr code from a file
        /// </summary>
        /// <param name="filePath">texure file path</param>
        public string DecodeQRValues(string filePath)
        {
            if (!File.Exists(filePath)) return string.Empty;

            Texture2D fileTexture = LoadTexture(filePath);
            return Decode(fileTexture.GetPixels32(), fileTexture.width, fileTexture.height);
        }

        /// <summary>
        /// To decode qr code from a bytes
        /// </summary>
        /// <param name="filePath">texure file path</param>
        public string DecodeQRValues(byte[] fileData)
        {
            Texture2D fileTexture = LoadTexture(fileData);
            return Decode(fileTexture.GetPixels32(), fileTexture.width, fileTexture.height);
        }

        /// <summary>
        /// To generate qr code
        /// </summary>
        /// <param name="textToEncode">string to encode in the qr code</param>
        /// <param name="width">optional, width of the qr code</param>
        /// <param name="height">optional, height of the qr code</param>
        public Texture2D GenerateQRCode(string textToEncode, int width = 256, int height = 256)
        {
            var encoded = new Texture2D(width, height);
            var color32 = Encode(textToEncode, encoded.width, encoded.height);
            encoded.SetPixels32(color32);
            encoded.Apply();
            return encoded;
        }

        #endregion

        #region helper functions

        /// <summary>
        ///  To encode qr code
        /// </summary>
        /// <param name="textToEncode">string to encode in the qr code</param>
        /// <param name="width">width of the qr code</param>
        /// <param name="height">height of the qr code</param>
        private Color32[] Encode(string textForEncoding, int width, int height)
        {
            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    Height = height,
                    Width = width
                }
            };
            return writer.Write(textForEncoding);
        }

        /// <summary>
        /// To decode qr code
        /// </summary>
        /// <param name="camTexture">webcam texure</param>
        private string Decode(Color32[] texture, int width, int height)
        {
            string returnValue = string.Empty;
            try
            {
                BarcodeReader barcodeReader = new BarcodeReader();
                // decode the current frame
                var result = barcodeReader.Decode(texture, width, height);
                if (result != null)
                {
                    Debug.Log("Decoded Text from QR:" + result.Text);
                    Debug.Log(result.BarcodeFormat.ToString());
                    returnValue = result.Text;
                }
                else
                {
                    Debug.LogWarning("result is null");
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex.Message);
            }

            return returnValue;
        }

        // Load a PNG or JPG image from bytes to a Texture2D
        public Texture2D LoadTexture(byte[] FileData)
        {
            Texture2D Tex2D = new Texture2D(2, 2);  // Create new "empty" texture
            if (Tex2D.LoadImage(FileData))           // Load the imagedata into the texture (size is set automatically)
                return Tex2D;                 // If data = readable -> return texture

            return null;
        }

        // Load a PNG or JPG file from disk to a Texture2D
        public Texture2D LoadTexture(string FilePath)
        {
            byte[] FileData;

            if (File.Exists(FilePath))
            {
                FileData = File.ReadAllBytes(FilePath);
                return LoadTexture(FileData);
            }

            return null;                     // Return null if load failed
        }
        #endregion
    }
}
