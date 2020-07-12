using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OneDevApp
{
    public abstract class WindowManager : MonoBehaviour
    {
        [SerializeField]
        private List<WindowItem> windows;
        private int currentWindowID;

        // Update is called once per frame
        protected virtual void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnEscKeyPressed();
            }
        }

        public int GetCurrentWindowID()
        {
            return currentWindowID;
        }

        public BaseWindow GetWindow(int value)
        {
            return windows[value].window;
        }

        public virtual void OpenWindow(WindowState value, bool closeLastWindow = false)
        {
            int _tempVal = windows.FindIndex(x => x.state == value);
            OpenWindow(_tempVal, closeLastWindow);
        }

        private void OpenWindow(int value, bool closeLastWindow = false)
        {
            if (value < 0 || value >= windows.Count)
                return;

            if (closeLastWindow)
            {
                DoWindowsTransition(value);
            }
            else
            {
                if (GetWindow(value).windowType == WindowType.POPUP_SCREEN)
                {
                    if (GetWindow(currentWindowID).windowType == WindowType.POPUP_SCREEN)
                        DoWindowsTransition(value);
                    else
                        DoWindowTransition(value);
                }
                else
                {
                    DoWindowsTransition(value);
                }
            }
        }

        public virtual void CloseAllWindows()
        {
            var total = windows.Count;

            for (var i = 0; i < total; i++)
            {
                var window = windows[i];
                if (window.window.gameObject.activeSelf)
                    window.window.Close();
            }
        }

        protected virtual void OnEscKeyPressed()
        {
            GetWindow(currentWindowID).OnEscKeyPressed();
        }

        private void ToggleWindowsVisability(int value)
        {
            GetWindow(value).Open();            
            GetWindow(currentWindowID).Close();
        }

        private void DoWindowsTransition(int value)
        {
            CustomUIPanel currentPanel = GetWindow(currentWindowID).customUIWindowPanel;
            CustomUIPanel nextPanel = GetWindow(value).customUIWindowPanel;

            if (nextPanel.panelContainer == null)
            {
                Debug.LogError("You didn't set the panel that you want to go to. Set it in inspector for " + GetWindow(value).name);
                return;
            }

            //If we've set the panels transition on then let it do the transition based on each panel settings
            if (nextPanel.enablePanelTransition)
            {
                //We are just initializing this. We will modify based on transition type.
                Vector2 nextPanel_anchorMinOutOfScreen = Vector2.one;
                Vector2 nextPanel_anchorMaxOutOfScreen = Vector2.one;

                //Lets set the min and max anchors of our next panel out of the screen based on panel settings
                //This is the point where our next panel appears!
                //We set where the panel appears by setting the transition type. (for example: Up to down - the animation starts from the top of the screen and go to the middle)
                switch (nextPanel.transitionType)
                {
                    case TransitionType.FROM_DOWN_TO_UP:
                        nextPanel_anchorMinOutOfScreen = new Vector2(0f, -1f);
                        nextPanel_anchorMaxOutOfScreen = new Vector2(1f, 0f);
                        break;

                    case TransitionType.FROM_UP_TO_DOWN:
                        nextPanel_anchorMinOutOfScreen = new Vector2(0f, 1f);
                        nextPanel_anchorMaxOutOfScreen = new Vector2(1f, 2f);
                        break;

                    case TransitionType.FROM_LEFT_TO_RIGHT:
                        nextPanel_anchorMinOutOfScreen = new Vector2(-1f, 0f);
                        nextPanel_anchorMaxOutOfScreen = new Vector2(0f, 1f);
                        break;

                    case TransitionType.FROM_RIGHT_TO_LEFT:
                        nextPanel_anchorMinOutOfScreen = new Vector2(1f, 0f);
                        nextPanel_anchorMaxOutOfScreen = new Vector2(2f, 1f);
                        break;

                    default:

                        break;
                }

                //Now lets set where the current panel goes based on settings that we have
                Vector2 currentPanel_anchorMin = Vector2.one;
                Vector2 currentPanel_anchorMax = Vector2.one;

                //Based on what we have chose for the current panel lets move it
                switch (currentPanel.transitionType)
                {
                    case TransitionType.FROM_DOWN_TO_UP:
                        currentPanel_anchorMin = new Vector2(0f, -1f);
                        currentPanel_anchorMax = new Vector2(1f, 0f);
                        break;

                    case TransitionType.FROM_UP_TO_DOWN:
                        currentPanel_anchorMin = new Vector2(0f, 1f);
                        currentPanel_anchorMax = new Vector2(1f, 2f);
                        break;

                    case TransitionType.FROM_LEFT_TO_RIGHT:
                        currentPanel_anchorMin = new Vector2(-1f, 0f);
                        currentPanel_anchorMax = new Vector2(0f, 1f);
                        break;

                    case TransitionType.FROM_RIGHT_TO_LEFT:
                        currentPanel_anchorMin = new Vector2(1f, 0f);
                        currentPanel_anchorMax = new Vector2(2f, 1f);
                        break;

                    default:

                        break;
                }

                //Lets create the animation sequence using dotween.
                Sequence animationSequence = DOTween.Sequence();

                //Make the animation sequence independent from unity's time scale.
                animationSequence.SetUpdate(true);

                //Now that we have defined our anchors min and max lets moeve the current panel
                //We will have to move in the same time both anchors (min and max)
                //Also set the ease and duration for the animation based on current settings.
                animationSequence.Append(currentPanel.panelContainer.DOAnchorMax(currentPanel_anchorMax, currentPanel.transitionDuration).SetEase(currentPanel.transitionEase));
                animationSequence.Join(currentPanel.panelContainer.DOAnchorMin(currentPanel_anchorMin, currentPanel.transitionDuration).SetEase(currentPanel.transitionEase).OnComplete(() =>
                {
                    //Lets set the next panel start position and activate the gameobject holder.
                    nextPanel.panelContainer.anchorMax = nextPanel_anchorMaxOutOfScreen;
                    nextPanel.panelContainer.anchorMin = nextPanel_anchorMinOutOfScreen;
                    
                    GetWindow(value).Open();

                    if (!currentPanel.enablePanelTransition)    //If Panel transition is diasbled for current panel
                    {
                        //Lets set the current panel start position and activate the gameobject holder.
                        currentPanel_anchorMin = new Vector2(0f, 0f);
                        currentPanel_anchorMax = new Vector2(1f, 1f);
                        Debug.Log("currentPanel.enablePanelTransition::" + currentPanel.enablePanelTransition);
                        animationSequence.Append(currentPanel.panelContainer.DOAnchorMax(currentPanel_anchorMax, currentPanel.transitionDuration).SetEase(currentPanel.transitionEase));
                        animationSequence.Append(currentPanel.panelContainer.DOAnchorMin(currentPanel_anchorMin, currentPanel.transitionDuration).SetEase(currentPanel.transitionEase));
                    }

                    //We will also have to deactive the current panel and activate the new one.
                    GetWindow(currentWindowID).Close();

                    //Now set the next panel to be the current panel. 
                    currentWindowID = value;
                }));

                //We are always moving the next panel's anchors to Vector2.one (1,1) for max anchors and Vector2.zero (0, 0) for min anchors
                //So we know that they will always be in the middle of the screen
                animationSequence.Append(nextPanel.panelContainer.DOAnchorMax(Vector2.one, nextPanel.transitionDuration).SetEase(nextPanel.transitionEase));
                animationSequence.Join(nextPanel.panelContainer.DOAnchorMin(Vector2.zero, nextPanel.transitionDuration).SetEase(nextPanel.transitionEase));
            }
            else
            {
                ToggleWindowsVisability(value);
                currentWindowID = value;
            }
        }

        private void DoWindowTransition(int value)
        {
            CustomUIPanel nextPanel = GetWindow(value).customUIWindowPanel;

            if (nextPanel.panelContainer == null)
            {
                Debug.LogError("You didn't set the panel that you want to go to. Set it in inspector for " + GetWindow(value).name);
                return;
            }

            //If we've set the panels transition on then let it do the transition based on each panel settings
            if (nextPanel.enablePanelTransition)
            {
                //We are just initializing this. We will modify based on transition type.
                Vector2 nextPanel_anchorMinOutOfScreen = Vector2.one;
                Vector2 nextPanel_anchorMaxOutOfScreen = Vector2.one;

                //Lets set the min and max anchors of our next panel out of the screen based on panel settings
                //This is the point where our next panel appears!
                //We set where the panel appears by setting the transition type. (for example: Up to down - the animation starts from the top of the screen and go to the middle)
                switch (nextPanel.transitionType)
                {
                    case TransitionType.FROM_DOWN_TO_UP:
                        nextPanel_anchorMinOutOfScreen = new Vector2(0f, -1f);
                        nextPanel_anchorMaxOutOfScreen = new Vector2(1f, 0f);
                        break;

                    case TransitionType.FROM_UP_TO_DOWN:
                        nextPanel_anchorMinOutOfScreen = new Vector2(0f, 1f);
                        nextPanel_anchorMaxOutOfScreen = new Vector2(1f, 2f);
                        break;

                    case TransitionType.FROM_LEFT_TO_RIGHT:
                        nextPanel_anchorMinOutOfScreen = new Vector2(-1f, 0f);
                        nextPanel_anchorMaxOutOfScreen = new Vector2(0f, 1f);
                        break;

                    case TransitionType.FROM_RIGHT_TO_LEFT:
                        nextPanel_anchorMinOutOfScreen = new Vector2(1f, 0f);
                        nextPanel_anchorMaxOutOfScreen = new Vector2(2f, 1f);
                        break;

                    default:

                        break;
                }


                //Lets create the animation sequence using dotween.
                Sequence animationSequence = DOTween.Sequence();

                //Make the animation sequence independent from unity's time scale.
                animationSequence.SetUpdate(true);

                //Lets set the next panel start position and activate the gameobject holder.
                nextPanel.panelContainer.anchorMax = nextPanel_anchorMaxOutOfScreen;
                nextPanel.panelContainer.anchorMin = nextPanel_anchorMinOutOfScreen;
                
                GetWindow(value).Open();

                //Now set the next panel to be the current panel. 
                currentWindowID = value;

                //We are always moving the next panel's anchors to Vector2.one (1,1) for max anchors and Vector2.zero (0, 0) for min anchors
                //So we know that they will always be in the middle of the screen
                animationSequence.Append(nextPanel.panelContainer.DOAnchorMax(Vector2.one, nextPanel.transitionDuration).SetEase(nextPanel.transitionEase));
                animationSequence.Join(nextPanel.panelContainer.DOAnchorMin(Vector2.zero, nextPanel.transitionDuration).SetEase(nextPanel.transitionEase));
            }
            else
            {
                GetWindow(value).Open();
                currentWindowID = value;
            }
        }
    }
}