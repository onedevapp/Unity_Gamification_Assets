using OneDevApp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo : MonoBehaviour
{
    public GameObject sceneLoader;

    private void Start()
    {
        if (!GameObject.FindObjectOfType<SceneLoader>())
        {
            Instantiate(sceneLoader);
        }
    }

    public void onButtonClicked(string sceneName)
    {
        SceneLoader.Instance.LoadScene(sceneName);
    }
}
