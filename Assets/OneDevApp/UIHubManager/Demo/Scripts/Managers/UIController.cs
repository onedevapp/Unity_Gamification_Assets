using OneDevApp;

public class UIController : WindowManager
{
    protected IGameService GameService { get; private set; }

    protected void Start()
    {
        GameService = ServiceLocator.GetService<IGameService>();

        GameService.OnMainMenuSignal.AddListener(OnMainMenu);
        GameService.OnGameStartSignal.AddListener(OnGameStart);
        GameService.OnGamePauseSignal.AddListener(OnGamePause);
        GameService.OnShopSignal.AddListener(OnShop);
        GameService.OnGameEndSignal.AddListener(OnGameEnd);
        GameService.OnQuitSignal.AddListener(OnQuit);
        GameService.OnSettingsSignal.AddListener(OnSettings);

        GetWindow(0).Open();
    }

    protected void OnDestroy()
    {
        GameService.OnMainMenuSignal.RemoveListener(OnMainMenu);
        GameService.OnGameStartSignal.RemoveListener(OnGameStart);
        GameService.OnGamePauseSignal.RemoveListener(OnGamePause);
        GameService.OnShopSignal.RemoveListener(OnShop);
        GameService.OnGameEndSignal.RemoveListener(OnGameEnd);
        GameService.OnQuitSignal.RemoveListener(OnQuit);
        GameService.OnSettingsSignal.RemoveListener(OnSettings);
    }

    void OnGameEnd()
    {
        OpenWindow(WindowState.RESULT);
    }

    void OnMainMenu()
    {
        OpenWindow(WindowState.MAINMENU);
    }

    void OnGameStart()
    {
        OpenWindow(WindowState.PLAYING);
    }

    void OnGamePause()
    {
        OpenWindow(WindowState.PAUSE);
    }

    void OnShop()
    {
        OpenWindow(WindowState.SHOP);
    }

    void OnSettings()
    {
        OpenWindow(WindowState.SETTINGS);
    }

    void OnQuit()
    {

    }
}
