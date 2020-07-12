using OneDevApp;

public class PauseWindow : GenericWindow
{

    public void OnContinueBtnClicked()
    {
        OnEscKeyPressed();
    }

    public void OnRestartBtnClicked()
    {
        GameService.OnGameStartSignal.Dispatch();
    }

    public void OnMainMenuBtnClicked()
    {
        GameService.OnMainMenuSignal.Dispatch();
    }
}
