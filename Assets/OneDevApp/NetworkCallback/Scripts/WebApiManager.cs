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
        public delegate void ReqCallback(bool isNetworkError, bool isHttpError, string error, string body);

        public void GetJsonNetWorkCall(string uri, string bodyJsonString, ReqCallback callback)
        {
            GetNetWorkCall(NetworkCallType.POST_METHOD_USING_JSONDATA, uri, bodyJsonString, null, callback);
        }

        public void GetNetWorkCall(NetworkCallType callType, string uri, List<KeyValuePojo> parameters, ReqCallback callback)
        {
            string bodyJsonString = string.Empty;

            if (callType == NetworkCallType.POST_METHOD_USING_JSONDATA)
                bodyJsonString = getEncodedParams(parameters);

            GetNetWorkCall(callType, uri, bodyJsonString, parameters, callback);
        }

        private void GetNetWorkCall(NetworkCallType callType, string uri, string bodyJsonString, List<KeyValuePojo> parameters, ReqCallback callback)
        {
            switch (callType)
            {
                case NetworkCallType.GET_METHOD:
                    StartCoroutine(RequestGetMethod(uri, parameters, callback));
                    break;
                case NetworkCallType.POST_METHOD_USING_JSONDATA:
                    StartCoroutine(PostRequestUsingJSON(uri, bodyJsonString, callback));
                    break;
                case NetworkCallType.POST_METHOD_USING_FORMDATA:
                    StartCoroutine(PostRequestUsingForm(uri, parameters, callback));
                    break;
            }
        }


        private IEnumerator RequestGetMethod(string url, List<KeyValuePojo> parameters, ReqCallback callback)
        {
            string getParameters = getEncodedParams(parameters);

            using (UnityWebRequest www = UnityWebRequest.Get(url + getParameters))
            {
                //Send request
                yield return www.SendWebRequest();
                //Return result
                callback(www.isNetworkError, www.isHttpError, www.error, www.downloadHandler.text);
            }
        }

        private IEnumerator PostRequestUsingJSON(string url, string bodyJsonString, ReqCallback callback)
        {
            using (UnityWebRequest www = UnityWebRequest.Post(url, bodyJsonString))
            {
                byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(bodyJsonString);
                www.uploadHandler = new UploadHandlerRaw(bodyRaw);
                www.SetRequestHeader("Content-Type", "application/json");
                yield return www.SendWebRequest();
                callback(www.isNetworkError, www.isHttpError, www.error, www.downloadHandler.text);
            }
        }

        private IEnumerator PostRequestUsingForm(string url, List<KeyValuePojo> parameters, ReqCallback callback)
        {
            WWWForm bodyFormData = new WWWForm();
            foreach (KeyValuePojo items in parameters)
            {
                bodyFormData.AddField(items.keyId, items.value);
            }

            using (UnityWebRequest www = UnityWebRequest.Post(url, bodyFormData))
            {
                yield return www.SendWebRequest();
                callback(www.isNetworkError, www.isHttpError, www.error, www.downloadHandler.text);
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
