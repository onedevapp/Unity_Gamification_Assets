using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace OneDevApp
{
    // delegate Declaration
    public delegate void OnPdf2ImageDelegate(List<FileInfo> files);

    public class PDFConvertManager : MonoInstance<PDFConvertManager>
    {
        // event Declaration
        public static event OnPdf2ImageDelegate OnPdf2ImageConvertEvent;

        public string folderPath;

        public void ConvertPDF2Image(string fileName, string fileFormat = "jpeg", int width = 600, int height = 800, int lastPage = 3, int firstPage = 1, int Quality = 100)
        {
            try
            {
                PDFConvert converter = new PDFConvert();
                string outputFilePrefix = Path.GetFileNameWithoutExtension(fileName);
                string outputFileFolder = Path.GetDirectoryName(Application.persistentDataPath) + (string.IsNullOrEmpty(folderPath) ? "" : Path.DirectorySeparatorChar + folderPath);
                string outputFilePath = outputFileFolder + Path.DirectorySeparatorChar + outputFilePrefix + @"_%01d." + fileFormat;

                if (!Directory.Exists(outputFileFolder))
                {
                    Directory.CreateDirectory(outputFileFolder);
                }

                Debug.Log(outputFilePath);
                Debug.Log("ConvertPDF2Image");

                if(converter.Convert(fileName,
                                 outputFilePath,
                                 firstPage,
                                 lastPage,
                                 fileFormat,
                                 width,
                                 height))
                    StartCoroutine(DoDelaySearchFiles(outputFilePrefix, fileFormat, outputFileFolder));
                else
                    OnPdf2ImageConvertEvent(new List<FileInfo>());
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex.ToString());
            }
        }

        private IEnumerator DoDelaySearchFiles(string mfileName, string mfileFormat, string mfilePath)
        {
            yield return new WaitForSeconds(2f);

            DirectoryInfo dir = new DirectoryInfo(mfilePath);
            List<FileInfo> returnFileInfos = new List<FileInfo>();
            int refreshAttempts = 0;

            while (refreshAttempts < 2)
            {
                refreshAttempts++;

                foreach (FileInfo f in dir.GetFiles("*." + mfileFormat).ToList())
                {
                    if (f.Name.StartsWith(mfileName))
                        returnFileInfos.Add(f);
                }

                if (returnFileInfos.Count > 0) refreshAttempts = 3;

                yield return new WaitForSeconds(2f);
            }

            // event calling / invoking
            OnPdf2ImageConvertEvent(returnFileInfos);
            Debug.Log("DoDelaySearchFiles");
            yield break;
        }


    }
}