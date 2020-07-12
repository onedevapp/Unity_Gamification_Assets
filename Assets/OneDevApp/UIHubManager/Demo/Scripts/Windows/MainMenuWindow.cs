using OneDevApp;

public class MainMenuWindow : GenericWindow
{
    public void OnPlayBtnClicked()
    {
        GameService.OnGameStartSignal.Dispatch();
    }

    public void OnShopBtnClicked()
    {
        GameService.OnShopSignal.Dispatch();
    }

    public void OnSettingBtnClicked()
    {
        GameService.OnSettingsSignal.Dispatch();
    }
}
