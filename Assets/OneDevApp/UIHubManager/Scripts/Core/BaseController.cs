using System;
using System.Collections;
using UnityEngine;

namespace OneDevApp
{
    public abstract class BaseController : MonoBehaviour, IGameService
    {
        public CustomActions OnMainMenuSignal { get; set; }
        public CustomActions OnGameStartSignal { get; set; }
        public CustomActions OnGameEndSignal { get; set; }
        public CustomActions OnGamePauseSignal { get; set; }
        public CustomActions OnShopSignal { get; set; }
        public CustomActions OnSettingsSignal { get; set; }
        public CustomActions OnQuitSignal { get; set; }

        public GameState CurrentState { get; set; }

        public bool IsPlaying
        {
            get { return CurrentState == GameState.PLAYING; }
        }

        public virtual void NewGame()
        {
            CurrentState = GameState.PLAYING;
        }

        public virtual void NewRound()
        {
            Reset();
            CurrentState = GameState.PLAYING;
        }

        public virtual void Quit()
        {
            CurrentState = GameState.MAINMENU;

            OnGameEndSignal.Dispatch();
        }

        protected void Awake()
        {
            OnMainMenuSignal = new CustomActions();
            OnGameStartSignal = new CustomActions();
            OnGameEndSignal = new CustomActions();
            OnGamePauseSignal = new CustomActions();
            OnShopSignal = new CustomActions();
            OnSettingsSignal = new CustomActions();
            OnQuitSignal = new CustomActions();

            ServiceLocator.AddService<IGameService>(this);
        }

        protected void Start()
        {

            Reset();

            CurrentState = GameState.MAINMENU;
        }

        protected void OnDestroy()
        {
            ServiceLocator.RemoveService<IGameService>();
        }

        public virtual void Reset()
        {
        }
    }
}