using OneDevApp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo : MonoBehaviour
{
    public void onButtonClicked(string sceneName)
    {
        SceneLoader.Instance.LoadScene(sceneName);
    }
}
