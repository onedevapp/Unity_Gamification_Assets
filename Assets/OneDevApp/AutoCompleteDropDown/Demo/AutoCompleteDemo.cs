using OneDevApp;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoCompleteDemo : MonoBehaviour
{
    public AutoCompleteDropDown autoComplete;
    public Text displayText;

    private void OnEnable()
    {
        autoComplete.OnSelectionChanged.AddListener(OnSelectionChanged);
    }

    private void OnSelectionChanged(string value, bool isValid, int position)
    {
        if (isValid)
            displayText.text = value;
    }

    public void SetValueToAutoComplete()
    {
        autoComplete.SetAsSelectedItem(3, true);

        //autoComplete.SetInteractable(true);
    }
}
