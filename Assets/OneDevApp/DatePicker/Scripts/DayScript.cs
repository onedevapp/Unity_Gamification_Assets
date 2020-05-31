using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace OneDevApp
{
    public class DayScript : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Normal color")]
        private Color NormalColor;
        [SerializeField]
        [Tooltip("Pressed color")]
        private Color SelectedColor;
        public DateTime dateTime;
        Text DayText;
        Image Background;
        bool Selected;

        public void Setup(string textValue, DateTime dateTime, bool selected)
        {
            this.dateTime = dateTime;
            DayText = gameObject.GetComponentInChildren<Text>();
            Background = gameObject.GetComponent<Image>();
            DayText.text = textValue;
            SetSelected(selected);
        }

        public void SetSelected(bool sel)
        {
            Selected = sel;
            if (Selected)
            {
                Background.color = SelectedColor;
            }

            else
            {
                Background.color = NormalColor;
            }
        }
    }
}