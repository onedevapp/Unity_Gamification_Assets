using DG.Tweening;
using UnityEngine;

namespace OneDevApp
{
    [System.Serializable]
    public class CustomUIPanel
    {
        public bool enablePanelTransition = true;
        public RectTransform panelContainer; //The panel container
        [Range(0, 1)]
        public float transitionDuration = 0.35f; //The transition duration
        public Ease transitionEase = Ease.OutBounce; //The transition ease
        public TransitionType transitionType = TransitionType.FROM_UP_TO_DOWN; //Transition type (the point where the panel stars)
    }

    public enum TransitionType
    {
        FROM_RIGHT_TO_LEFT, //Stars from the right part of the scene and goes to left (stops in middle of the screen)
        FROM_LEFT_TO_RIGHT, //Stars from the left part of the scene and goes to right (stops in middle of the screen)
        FROM_UP_TO_DOWN, //Stars from the top part of the scene and goes to bottom (stops in middle of the screen)
        FROM_DOWN_TO_UP //Stars from the bottom part of the scene and goes to top (stops in middle of the screen)
    }
}