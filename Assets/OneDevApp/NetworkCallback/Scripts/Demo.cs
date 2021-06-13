using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneDevApp.WebApi_Demo
{
    public class Demo : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            List<KeyValuePojo> urlParam = new List<KeyValuePojo>();
            urlParam.Add(new KeyValuePojo("postId", "1"));
            WebApiManager.Instance.GetNetWorkCall(NetworkCallType.GET_METHOD, "https://jsonplaceholder.typicode.com/comments", urlParam, (bool isSuccess, string error, string body)=> {
                if (isSuccess)
                {
                    Debug.Log(body);
                }
                else
                {
                    Debug.Log(error);
                }
            }, 30); 
            
            WebApiManager.Instance.GetDownloadImage("https://cdn.pixabay.com/photo/2021/05/25/14/23/girl-6282604_960_720.jpg",  (bool isSuccess, string error, Texture2D imageTex) => {
                if (isSuccess)
                {
                    //convert imageTex to sprite or any
                }
                else
                {
                    Debug.Log(error);
                }
            }, 30);
        }

    }
}
