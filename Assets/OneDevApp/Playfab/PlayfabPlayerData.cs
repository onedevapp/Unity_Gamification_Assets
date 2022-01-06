using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;

namespace OneDevApp
{
    [Serializable]
    public class PlayfabPlayerData
    {
        #region private variables

        public Action onPlayerDataChanged;
        public Action onPlayerLeveledUp;

        #region profile title data
        [SerializeField] private string NickName;
        [SerializeField] private string AvatarUrl;
        [SerializeField] private int PlayerLevel;
        [SerializeField] private int GamesPlayed;
        [SerializeField] private int GamesWon;

        [SerializeField] private double CoinVal;
        [SerializeField] private int TrophyVal;
        [SerializeField] private int PlayerXP;
        #endregion

        #region title data
        [SerializeField] private float AppVersion;
        #endregion

        #endregion


        #region Title Data Getter

        public float GetAppVersion()
        {
            return AppVersion;
        }
        #endregion

        #region Details Setter
        public void SetNickName(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                Debug.LogError("set name");
                NickName = value;
                PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest() { DisplayName = NickName }, (response) => {
                }, (error) => { }, null);
            }
            else
                Debug.LogError("not set name"+value);

        }

        #endregion

        #region Details Getter

        public string GetNickName()
        {
            return NickName;
        }

        public string GetAvatarUrl()
        {
            return AvatarUrl;
        }
        #endregion

        #region Stats Getter

        public int GetPlayerLevel()
        {
            return PlayerLevel;
        }
        public int GetGamesPlayed()
        {
            return GamesPlayed;
        }
        public int GetGamesWon()
        {
            return GamesWon;
        }
        public int GetXPVal()
        {
            return PlayerXP;
        }
        public int GetTrophyVal()
        {
            return TrophyVal;
        }
        public int GetPlayerXPVal()
        {
            return PlayerXP;
        }
        #endregion

        #region Helper

        private void UpdateDataToPlayFab(string key, string value, UserDataPermission permission = UserDataPermission.Public)
        {
            Dictionary<string, string> dataToPlayFab = new Dictionary<string, string>();
            dataToPlayFab.Add(key, value);
            PlayFabManager.Instance.UpdatePlayerData(permission, dataToPlayFab, () => {
             
            }, (error) => {

            });
        }

        public void LoadFromJson(string jsonData, Action onComplete)
        {
            JsonUtility.FromJsonOverwrite(jsonData, this);
            onComplete.Invoke();
        }

        public void SetPlayerDetailsData(string jsonData, Action onComplete)
        {
            var _cacheUrl = AvatarUrl;
            var _cacheLevel = PlayerLevel;

            JsonUtility.FromJsonOverwrite(jsonData, this);

            if (PlayerLevel > _cacheLevel && _cacheLevel >= 1)
            {
                if (onPlayerLeveledUp != null)
                    onPlayerLeveledUp.Invoke();
            }
            else
            {
                if (onPlayerDataChanged != null)
                    onPlayerDataChanged.Invoke();
            }

            onComplete.Invoke();

        }

        #endregion

    }
}
