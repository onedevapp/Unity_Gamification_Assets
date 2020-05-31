using OneDevApp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPrefsDemo : MonoBehaviour
{
    public Toggle musicToggle;

    public void OnEnable()
    {
        musicToggle.onValueChanged.AddListener(onMusicToggleValueChange);

        if (!EncryptedPlayerPrefs.Instance.HasKey("music"))
        {
            EncryptedPlayerPrefs.Instance.SetBool("music", true);
            musicToggle.isOn = true;
        }
        else
        {
            if (EncryptedPlayerPrefs.Instance.GetBool("music"))
            {
                musicToggle.isOn = false;
            }
            else
            {
                musicToggle.isOn = true;
            }

            Debug.Log("Music is "+ musicToggle.isOn + " from EncryptedPlayerPrefs");
        }
    }

    private void OnDisable()
    {
        musicToggle.onValueChanged.RemoveAllListeners();
    }

    public void onMusicToggleValueChange(bool value)
    {
        EncryptedPlayerPrefs.Instance.SetBool("music", value);
        Debug.Log("Music is " + value + " from UI");
    }

}
