// you need to import https://github.com/playgameservices/play-games-plugin-for-unity
using UnityEngine;
using System;
using System.Collections;
//gpg
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
//for encoding
using System.Text;

namespace OneDevApp
{
    public class PlayCloudDataManager : MonoInstance<PlayCloudDataManager>
    {

        public bool isProcessing
        {
            get;
            private set;
        }
        public string loadedData
        {
            get;
            private set;
        }
        private const string m_saveFileName = "game_save_data";
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
            // enables saving game progress.
            .EnableSavedGames()
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

        public void SaveToCloud(string dataToSave)
        {
            if (isAuthenticated)
            {
                loadedData = dataToSave;
                isProcessing = true;
                ((PlayGamesPlatform)Social.Active).SavedGame.OpenWithAutomaticConflictResolution(m_saveFileName, DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, OnFileOpenToSave);
            }
            else
            {
                Login();
            }
        }

        public void LoadFromCloud(Action<string> afterLoadAction)
        {
            if (isAuthenticated && !isProcessing)
            {
                StartCoroutine(LoadFromCloudRoutin(afterLoadAction));
            }
            else
            {
                Login();
            }
        }

        public void DeleteGameData(string filename)
        {
            if (isAuthenticated)
            {
                // Open the file to get the metadata.
                ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
                savedGameClient.OpenWithAutomaticConflictResolution(filename, DataSource.ReadCacheOrNetwork,
                    ConflictResolutionStrategy.UseLongestPlaytime, OnDeleteSavedGame);
            }
            else
            {
                Login();
            }
        }

        private void OnDeleteSavedGame(SavedGameRequestStatus status, ISavedGameMetadata game)
        {
            if (status == SavedGameRequestStatus.Success)
            {
                ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
                savedGameClient.Delete(game);
            }
            else
            {
                Debug.LogWarning("Error deleting Saved Game" + status);
            }
        }


        private void ProcessCloudData(byte[] cloudData)
        {
            if (cloudData == null)
            {
                Debug.Log("No Data saved to the cloud");
                return;
            }

            string progress = BytesToString(cloudData);
            loadedData = progress;
        }

        private IEnumerator LoadFromCloudRoutin(Action<string> loadAction)
        {
            isProcessing = true;
            Debug.Log("Loading game progress from the cloud.");

            ((PlayGamesPlatform)Social.Active).SavedGame.OpenWithAutomaticConflictResolution(
                m_saveFileName, //name of file.
                DataSource.ReadCacheOrNetwork,
                ConflictResolutionStrategy.UseLongestPlaytime,
                OnFileOpenToLoad);

            while (isProcessing)
            {
                yield return null;
            }

            loadAction.Invoke(loadedData);
        }

        private void OnFileOpenToSave(SavedGameRequestStatus status, ISavedGameMetadata metaData)
        {
            if (status == SavedGameRequestStatus.Success)
            {

                byte[] data = StringToBytes(loadedData);

                SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();

                builder.WithUpdatedDescription("Saved game at " + DateTime.Now);

                SavedGameMetadataUpdate updatedMetadata = builder.Build();

                ((PlayGamesPlatform)Social.Active).SavedGame.CommitUpdate(metaData, updatedMetadata, data, OnGameSave);
            }
            else
            {
                Debug.LogWarning("Error opening Saved Game" + status);
            }
        }


        private void OnFileOpenToLoad(SavedGameRequestStatus status, ISavedGameMetadata metaData)
        {
            if (status == SavedGameRequestStatus.Success)
            {
                ((PlayGamesPlatform)Social.Active).SavedGame.ReadBinaryData(metaData, OnGameLoad);
            }
            else
            {
                Debug.LogWarning("Error opening Saved Game" + status);
            }
        }


        private void OnGameLoad(SavedGameRequestStatus status, byte[] bytes)
        {
            if (status != SavedGameRequestStatus.Success)
            {
                Debug.LogWarning("Error Saving" + status);
            }
            else
            {
                ProcessCloudData(bytes);
            }

            isProcessing = false;
        }

        private void OnGameSave(SavedGameRequestStatus status, ISavedGameMetadata metaData)
        {
            if (status != SavedGameRequestStatus.Success)
            {
                Debug.LogWarning("Error Saving" + status);
            }

            isProcessing = false;
        }

        private byte[] StringToBytes(string stringToConvert)
        {
            return Encoding.UTF8.GetBytes(stringToConvert);
        }

        private string BytesToString(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }
    }

}