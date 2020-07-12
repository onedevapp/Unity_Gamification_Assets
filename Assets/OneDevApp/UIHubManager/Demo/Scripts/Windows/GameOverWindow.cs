using OneDevApp;

public class GameOverWindow : GenericWindow
{

    public void OnRestartBtnClicked()
    {
        GameService.OnGameStartSignal.Dispatch();
    }

    public void OnMainMenuBtnClicked()
    {
        GameService.OnMainMenuSignal.Dispatch();
    }

}
