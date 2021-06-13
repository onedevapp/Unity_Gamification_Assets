using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace OneDevApp
{
    [System.Serializable]
    public class KeyValuePojo
    {
        public string keyId;
        public string value;

        public KeyValuePojo() { }

        public KeyValuePojo(string keyId, string value)
        {
            this.keyId = keyId;
            this.value = value;
        }
    }

    public enum NetworkCallType
    {
        GET_METHOD,
        POST_METHOD_USING_JSONDATA,
        POST_METHOD_USING_FORMDATA
    }

    public class WebApiManager : MonoInstance<WebApiManager>
    {
        public delegate void ReqCallback(bool isSuccess, string error, string body);
        public delegate void ReqCallbackTex(bool isSuccess, string error, Texture2D imageTex);
        private const int timeOut = 45;

        public void GetJsonNetWorkCall(string uri, string bodyJsonString, ReqCallback callback, int timeout = timeOut)
        {
            GetNetWorkCall(NetworkCallType.POST_METHOD_USING_JSONDATA, uri, bodyJsonString, null, callback, timeout);
        }

        public void GetNetWorkCall(NetworkCallType callType, string uri, List<KeyValuePojo> parameters, ReqCallback callback, int timeout = timeOut)
        {
            string bodyJsonString = string.Empty;

            if (callType == NetworkCallType.POST_METHOD_USING_JSONDATA)
                bodyJsonString = getEncodedParams(parameters);

            GetNetWorkCall(callType, uri, bodyJsonString, parameters, callback, timeout);
        }
        public void GetDownloadImage(string uri, ReqCallbackTex callback, int timeout = timeOut)
        {
            StartCoroutine(DownloadImage(uri, callback, timeout));
        }

        private void GetNetWorkCall(NetworkCallType callType, string uri, string bodyJsonString, List<KeyValuePojo> parameters, ReqCallback callback, int timeout = timeOut)
        {
            switch (callType)
            {
                case NetworkCallType.GET_METHOD:
                    StartCoroutine(RequestGetMethod(uri, parameters, callback, timeout));
                    break;
                case NetworkCallType.POST_METHOD_USING_JSONDATA:
                    StartCoroutine(PostRequestUsingJSON(uri, bodyJsonString, callback, timeout));
                    break;
                case NetworkCallType.POST_METHOD_USING_FORMDATA:
                    StartCoroutine(PostRequestUsingForm(uri, parameters, callback, timeout));
                    break;
            }
        }


        private IEnumerator RequestGetMethod(string url, List<KeyValuePojo> parameters, ReqCallback callback, int timeout = timeOut)
        {
            string getParameters = getEncodedParams(parameters);

            using (UnityWebRequest www = UnityWebRequest.Get(url + getParameters))
            {
                www.timeout = timeout;
                //Send request
                yield return www.SendWebRequest();

                while (!www.isDone)
                    yield return www;

                while (!www.downloadHandler.isDone)
                    yield return null;

                //Return result

#if UNITY_2020_1_OR_NEWER
                callback(www.result == UnityWebRequest.Result.Success, www.error, www.downloadHandler.text);
#else
                callback(www.isDone, www.error, www.downloadHandler.text);
#endif
            }
        }

        private IEnumerator PostRequestUsingJSON(string url, string bodyJsonString, ReqCallback callback, int timeout = timeOut)
        {
            using (UnityWebRequest www = UnityWebRequest.Post(url, bodyJsonString))
            {
                www.timeout = timeout;

                byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(bodyJsonString);
                www.uploadHandler = new UploadHandlerRaw(bodyRaw);
                www.SetRequestHeader("Content-Type", "application/json");
                yield return www.SendWebRequest();

                while (!www.isDone)
                    yield return www;

                while (!www.downloadHandler.isDone)
                    yield return null;
                callback(www.isDone, www.error, www.downloadHandler.text);

                //Return result
#if UNITY_2020_1_OR_NEWER
                callback(www.result == UnityWebRequest.Result.Success, www.error, www.downloadHandler.text);
#else
                callback(www.isDone, www.error, www.downloadHandler.text);
#endif
            }
        }

        private IEnumerator PostRequestUsingForm(string url, List<KeyValuePojo> parameters, ReqCallback callback, int timeout = timeOut)
        {
            WWWForm bodyFormData = new WWWForm();
            foreach (KeyValuePojo items in parameters)
            {
                bodyFormData.AddField(items.keyId, items.value);
            }

            using (UnityWebRequest www = UnityWebRequest.Post(url, bodyFormData))
            {
                www.timeout = timeout;
                yield return www.SendWebRequest();

                while (!www.isDone)
                    yield return www;

                while (!www.downloadHandler.isDone)
                    yield return null;

                //Return result
#if UNITY_2020_1_OR_NEWER
                callback(www.result == UnityWebRequest.Result.Success, www.error, www.downloadHandler.text);
#else
                callback(www.isDone, www.error, www.downloadHandler.text);
#endif
            }
        }

        private IEnumerator DownloadImage(string url, ReqCallbackTex callback, int timeout = timeOut)
        {
            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
            {
                www.timeout = timeout;
                yield return www.SendWebRequest();

                while (!www.isDone)
                    yield return www;

                while (!www.downloadHandler.isDone)
                    yield return null;

                //Return result
#if UNITY_2020_1_OR_NEWER
                callback(www.result == UnityWebRequest.Result.Success, www.error, ((DownloadHandlerTexture)www.downloadHandler).texture);
#else
                callback(www.isDone, www.error, ((DownloadHandlerTexture)www.downloadHandler).texture);
#endif

            }
        }

        public string getEncodedParams(List<KeyValuePojo> parameters)
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePojo items in parameters)
            {
                string value =  UnityWebRequest.EscapeURL(items.value);

                if (sb.Length > 0)
                {
                    sb.Append("&");
                }

                sb.Append(items.keyId + "=" + value);
            }
            if (sb.Length > 0)
            {
                sb.Insert(0, "?");
            }
            return sb.ToString();
        }

        public bool HasInternet()
        {
            NetworkReachability reachability = Application.internetReachability;

            switch (reachability)
            {
                case NetworkReachability.ReachableViaCarrierDataNetwork:
                    return true;
                case NetworkReachability.ReachableViaLocalAreaNetwork:
                    return true;
            }

            return false;
        }

        public string getJsonParams(List<KeyValuePojo> parameters)
        {
            var entries = parameters.Select(d =>
            string.Format("\"{0}\": \"{1}\",", d.keyId, d.value));
            return "{" + entries.ToString().Remove(entries.ToString().Length - 1) + "}";
        }
    }
}
