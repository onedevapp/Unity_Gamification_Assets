using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneDevApp
{
    public class AchievementManager : MonoInstance<AchievementManager>
    {

        public bool isAuthenticated
        {
            get
            {
                return Social.localUser.authenticated;
            }
        }

        #region Achievement Init
        protected override void Awake()
        {
            InitiatePlayGames();
        }

        private void Login()
        {
            Social.localUser.Authenticate((bool success) =>
            {
                if (success)
                {
                    ((PlayGamesPlatform)Social.Active).SetGravityForPopups(Gravity.BOTTOM);
                }
                else
                {
                    Debug.Log("Fail Login");
                }
            });
        }

        private void InitiatePlayGames()
        {
            PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            .Build();

            PlayGamesPlatform.InitializeInstance(config);
            // recommended for debugging:
            PlayGamesPlatform.DebugLogEnabled = false;
            // Activate the Google Play Games platform
            PlayGamesPlatform.Activate();
        }
        #endregion

        #region Public Methods
        public void Login(Action<bool> afterLoginAction)
        {
            Social.localUser.Authenticate((bool success) =>
            {
                afterLoginAction.Invoke(success);
                if (success)
                {
                    ((PlayGamesPlatform)Social.Active).SetGravityForPopups(Gravity.BOTTOM);
                }
                else
                {
                    Debug.Log("Fail Login");
                }
            });
        }

        public void UnlockAchievement(string id)
        {
            if (isAuthenticated)
            {
                Social.ReportProgress(id, 100, success => { });
            }
            else
            {
                Login();
            }
        }

        public void IncrementAchievement(string id, int stepsToIncrement)
        {
            if (isAuthenticated)
            {
                PlayGamesPlatform.Instance.IncrementAchievement(id, stepsToIncrement, success => { });
            }
            else
            {
                Login();
            }
        }

        public void ShowAchievementsUI()
        {
            if (isAuthenticated)
            {
                Social.ShowAchievementsUI();
            }
            else
            {
                Login();
            }
        }
        #endregion /Achievements
    }

}