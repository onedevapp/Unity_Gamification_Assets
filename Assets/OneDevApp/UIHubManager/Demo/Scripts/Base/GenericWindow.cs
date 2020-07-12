using OneDevApp;

public class GenericWindow : BaseWindow
{
    public WindowState previousWindow;

    public override void OnEscKeyPressed()
    {
        if (windowType == WindowType.POPUP_SCREEN)
        {
            Close();
        }
        else if (previousWindow != WindowState.NONE)
        {
            if (windowType == WindowType.FULL_SCREEN)
            {
                windowManager.OpenWindow(previousWindow);
            }
            else if (windowType == WindowType.HOME_SCREEN)
            {
                //Show Close Dialog or Toast to press again to close
            }
        }
    }
}
