using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneDevApp
{
    public class LeaderboardManager : MonoInstance<LeaderboardManager>
    {
        public bool isAuthenticated
        {
            get
            {
                return Social.localUser.authenticated;
            }
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

        protected override void Awake()
        {
            InitiatePlayGames();
        }


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

        #region Leaderboards
        public void AddScoreToLeaderboard(string leaderboardId, long score)
        {
            if (isAuthenticated)
            {
                Social.ReportScore(score, leaderboardId, success => { });
            }
            else
            {
                Login();
            }
        }

        public void ShowLeaderboardsUI()
        {
            if (isAuthenticated)
            {
                Social.ShowLeaderboardUI();
            }
            else
            {
                Login();
            }
        }

        public void ShowLeaderboardsUI(string leaderboardId)
        {
            if (isAuthenticated)
            {
                PlayGamesPlatform.Instance.ShowLeaderboardUI(leaderboardId);
            }
            else
            {
                Login();
            }
        }
        #endregion /Leaderboards
    }

}