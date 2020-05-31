using System;
using System.IO;
using System.Net;
using UnityEngine;

namespace OneDevApp
{
    public enum FTPType
    {
        USING_FTP,
        USING_WEBCLIENT
    }

    public class FTPManager : MonoInstance<FTPManager>
    {
        public byte[] DownloadFile(FTPType fTPType, string ftpUrl, string savePath, string userName = "", string password = "")
        {
            switch (fTPType)
            {
                case FTPType.USING_WEBCLIENT:
                    return downloadWithWEB(ftpUrl, savePath, userName, password);
                default:
                case FTPType.USING_FTP:
                    return downloadWithFTP(ftpUrl, savePath, userName, password);
            }
        }

        private byte[] downloadWithWEB(string ftpUrl, string savePath, string userName = "", string password = "")
        {
            Debug.Log("FTP URL :: " + ftpUrl);
            WebClient client = new WebClient();
            Uri uri = new Uri(ftpUrl);

            //If username or password is NOT null then use Credential
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
            {
                Debug.Log("FTP userName :: " + userName);
                client.Credentials = new NetworkCredential(userName, password);
            }

            byte[] bytesData = client.DownloadData(uri);

            if (!string.IsNullOrEmpty(savePath))
            {
                Debug.Log("FTP savePath :: " + savePath);
                SaveFile(bytesData, savePath);
            }

            return bytesData;
        }

        private byte[] downloadWithFTP(string ftpUrl, string savePath, string userName = "", string password = "")
        {
            Debug.Log("FTP URL :: " + ftpUrl);

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(ftpUrl));

            request.UsePassive = true;
            request.UseBinary = true;
            request.KeepAlive = true;

            //If username or password is NOT null then use Credential
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
            {
                Debug.Log("FTP userName :: " + userName);
                request.Credentials = new NetworkCredential(userName, password);
            }

            request.Method = WebRequestMethods.Ftp.DownloadFile;

            byte[] bytesData = downloadAsbyteArray(request.GetResponse());

            if (!string.IsNullOrEmpty(savePath))
            {
                Debug.Log("FTP savePath :: " + savePath);
                SaveFile(bytesData, savePath);
            }

            return bytesData;
        }

        byte[] downloadAsbyteArray(WebResponse request)
        {
            using (Stream input = request.GetResponseStream())
            {
                byte[] buffer = new byte[16 * 1024];
                using (MemoryStream ms = new MemoryStream())
                {
                    int read;
                    while (input.CanRead && (read = input.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, read);
                    }
                    return ms.ToArray();
                }
            }
        }

        void SaveFile(byte[] bytesData, string savePath)
        {
            //Create Directory if it does not exist
            if (!Directory.Exists(Path.GetDirectoryName(savePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(savePath));
            }

            File.WriteAllBytes(savePath, bytesData);
        }
    }
}
