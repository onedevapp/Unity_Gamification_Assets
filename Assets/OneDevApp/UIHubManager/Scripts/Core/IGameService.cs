using System.Collections;

namespace OneDevApp
{
    public interface IGameService
    {
        CustomActions OnMainMenuSignal { get; }
        CustomActions OnGameStartSignal { get; }
        CustomActions OnGameEndSignal { get; }
        CustomActions OnGamePauseSignal { get; }
        CustomActions OnShopSignal { get; }
        CustomActions OnSettingsSignal { get; }
        CustomActions OnQuitSignal { get; }

        GameState CurrentState { get; }
        bool IsPlaying { get; }

        void NewGame();
        void NewRound();
        void Quit();
    }
}
