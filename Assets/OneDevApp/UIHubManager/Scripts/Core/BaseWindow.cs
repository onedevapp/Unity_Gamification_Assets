using UnityEngine;
using UnityEngine.EventSystems;

namespace OneDevApp
{
    public enum WindowType
    {
        HOME_SCREEN = 0,
        FULL_SCREEN = 1,
        POPUP_SCREEN = 2
    }

    public abstract class BaseWindow : BaseBehaviour
    {
        public WindowType windowType = WindowType.FULL_SCREEN;
        public CustomUIPanel customUIWindowPanel; //Main menu panel with its settings
        public GameObject firstSelected;

        //[HideInInspector]
        public WindowManager windowManager;

        protected EventSystem eventSystem
        {
            get { return GameObject.Find("EventSystem").GetComponent<EventSystem>(); }
        }

        protected override void Awake()
        {
            windowManager = GameObject.FindObjectOfType<WindowManager>();
            Close();
        }

        public virtual void OnFocus()
        {
            eventSystem.SetSelectedGameObject(firstSelected);
        }

        protected virtual void Display(bool value)
        {
            gameObject.SetActive(value);
        }

        public virtual void Open()
        {
            Display(true);
            OnFocus();
        }

        public virtual void Close()
        {
            Display(false);
        }

        public virtual void OnEscKeyPressed()
        {
        }
    }
}