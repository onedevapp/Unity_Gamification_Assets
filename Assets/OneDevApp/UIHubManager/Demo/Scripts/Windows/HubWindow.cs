using OneDevApp;

public class HubWindow : GenericWindow
{
    public void OnGameOverBtnClicked()
    {
        GameManager.Instance.gameController.CurrentState = GameState.RESULT;

        GameService.OnGameEndSignal.Dispatch();
    }

    public void OnPauseBtnClicked()
    {
        GameManager.Instance.gameController.CurrentState = GameState.PAUSE;

        GameService.OnGamePauseSignal.Dispatch();
    }
}
