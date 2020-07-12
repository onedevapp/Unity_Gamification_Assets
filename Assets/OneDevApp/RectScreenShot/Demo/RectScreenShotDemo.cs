using OneDevApp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectScreenShotDemo : MonoBehaviour
{
    string fileName = "/OneDevApp/RectScreenShot/ScreenShot.png";


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RectScreenShot.Instance.CaputureScreenShot(fileName);
        }

    }
}
