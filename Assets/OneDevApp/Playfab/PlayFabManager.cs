using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OneDevApp
{
    public class PlayFabManager : MonoInstance<PlayFabManager>
    {
        public bool ToGeneratePlayStreamEvent = false;
        public string _playFabPlayerIdCache;

        #region Registration   
        public void RegisterWithCredentails(RegisterDetailsRequest registerDetails, Action successAction, Action<PlayFabError> failureAction)
        {
            _playFabPlayerIdCache = string.Empty;

            RegisterPlayFabUserRequest request = new RegisterPlayFabUserRequest()
            {
                
                DisplayName = registerDetails.nickName,
                Username = registerDetails.mobileNo,
                Email = registerDetails.emailId,
                Password = registerDetails.passWord,
                RequireBothUsernameAndEmail = true
            };

            PlayFabClientAPI.RegisterPlayFabUser(request,
                (result) =>
                {
                    PlayFabClientAPI.LinkCustomID(new LinkCustomIDRequest()
                    {
                        CustomId = registerDetails.mobileNo,
                        ForceLink = false
                    }, (response) =>
                    {

                        PlayFabClientAPI.AddOrUpdateContactEmail(new AddOrUpdateContactEmailRequest()
                        {
                            EmailAddress = registerDetails.emailId,
                        }, (response) => { 
                        }, (failure) => { });

                    }, (failure) => { 
                    });

                    if (successAction != null)
                        successAction.Invoke();
                }, (error) =>
                {

                    if (failureAction != null)
                        failureAction.Invoke(error);
                });

        }
        #endregion

        #region Login
        public void LoginWithFacebook(FacebookDetailsRequest facebookDetails, bool createAccount, Action successAction, Action<PlayFabError> failureAction)
        {
            _playFabPlayerIdCache = string.Empty;
                        
            LoginWithDeviceId(false, () => {
                LinkFBToLogin(facebookDetails, (linkSuccess) => {                   
    #if !UNITY_EDITOR
                    UnLinkDeviceId((removeResult)=> {
                        if (successAction != null)
                            successAction.Invoke();
                    }, error=> {
                        if (successAction != null)
                            successAction.Invoke();
                    });
    #else
                    if (successAction != null)
                        successAction.Invoke();
    #endif

                }, (linkError) => {

                    if (failureAction != null)
                        failureAction.Invoke(linkError);
                });
            }, (error) => {
                SilentFBLogin(facebookDetails, createAccount, successAction, failureAction);
            });
            
        }
        public void LinkFBToLogin(FacebookDetailsRequest facebookDetails, Action<LinkFacebookAccountResult> successAction, Action<PlayFabError> failureAction)
        {
            LinkFacebookAccountRequest request = new LinkFacebookAccountRequest()
            {
                AccessToken = facebookDetails.accessToken,
                ForceLink = true
            };

            PlayFabClientAPI.LinkFacebookAccount(request,
                (result) =>
                {
                    OnFBLoginCreateNew(facebookDetails, ()=> { successAction.Invoke(result); });

                }, (error) =>
                {
                    if (failureAction != null)
                        failureAction.Invoke(error);
                });
        }

        private void SilentFBLogin(FacebookDetailsRequest facebookDetails, bool createAccount, Action successAction, Action<PlayFabError> failureAction)
        {
            LoginWithFacebookRequest request = new LoginWithFacebookRequest()
            {
                CreateAccount = createAccount,
                AccessToken = facebookDetails.accessToken
            };

            PlayFabClientAPI.LoginWithFacebook(request,
                (result) =>
                {
                    if (result.NewlyCreated)
                    {
                        OnFBLoginCreateNew(facebookDetails, ()=> { successAction.Invoke(); });
                    }
                    else
                    {
                        if (successAction != null)
                            successAction.Invoke();
                    }

                }, (error) =>
                {
                    if (failureAction != null)
                        failureAction.Invoke(error);
                });
        }
        void OnFBLoginCreateNew(FacebookDetailsRequest facebookDetails, Action successAction)
        {
            PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest() { DisplayName = facebookDetails.profileName }, (response) => {

                if (successAction != null)
                    successAction.Invoke();
            }, (error) => {
                if (successAction != null)
                    successAction.Invoke();
            }, null);
        }

        public void LoginWithCredentails(string emailId, string password, Action successAction, Action<PlayFabError> failureAction)
        {
            _playFabPlayerIdCache = string.Empty;

            LoginWithEmailAddressRequest request = new LoginWithEmailAddressRequest()
            {
                Email = emailId,
                Password = password
            };

            PlayFabClientAPI.LoginWithEmailAddress(request,
                (result) =>
                {
                    if (successAction != null)
                        successAction.Invoke();

                }, (error) =>
                {
                    if (failureAction != null)
                        failureAction.Invoke(error);
                });
        }
        public void LoginWithMobileNo(string mobileNo, Action successAction, Action<PlayFabError> failureAction)
        {
            _playFabPlayerIdCache = string.Empty;

            LoginWithCustomIDRequest request = new LoginWithCustomIDRequest()
            {                
                CreateAccount = false,
                CustomId = mobileNo
            };

            PlayFabClientAPI.LoginWithCustomID(request,
                (result) =>
                {
                    if (successAction != null)
                        successAction.Invoke();
                    
                }, (error) =>
                {
                    if (failureAction != null)
                        failureAction.Invoke(error);
                });

        }
        public void LoginWithDeviceId(bool createAccount, Action successAction, Action<PlayFabError> failureAction)
        {
            _playFabPlayerIdCache = string.Empty;
            string deviceId = PlayFabSettings.DeviceUniqueIdentifier;

            LoginWithAndroidDeviceIDRequest request = new LoginWithAndroidDeviceIDRequest()
            {
                CreateAccount = createAccount,
                AndroidDeviceId = deviceId
            };

            PlayFabClientAPI.LoginWithAndroidDeviceID(request,
                (result) =>
                {
                    if (result.NewlyCreated)
                    {
                        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest() { DisplayName = "Guest_" + result.PlayFabId }, (response) => {
                            if (successAction != null)
                                successAction.Invoke();
                        }, (error) => {
                            if (failureAction != null)
                                failureAction.Invoke(error);
                        }, null);
                    }
                    else
                    {
                        if (successAction != null)
                            successAction.Invoke();
                    }
                }, (error) =>
                {
                    if (failureAction != null)
                        failureAction.Invoke(error);
                });

        }
        public void UnLinkDeviceId(Action<UnlinkAndroidDeviceIDResult> successAction, Action<PlayFabError> failureAction)
        {
            var request = new UnlinkAndroidDeviceIDRequest() { AndroidDeviceId = PlayFabSettings.DeviceUniqueIdentifier };
            PlayFabClientAPI.UnlinkAndroidDeviceID(request, (result) => {
                if (successAction != null)
                    successAction.Invoke(result);
            }, (error)=> {
                if (failureAction != null)
                    failureAction.Invoke(error);
            });
        }
        #endregion

        #region ResetForgot
        public void SendAccountRecoveryEmail(string emailId, Action successAction, Action<PlayFabError> failureAction)
        {
            SendAccountRecoveryEmailRequest request = new SendAccountRecoveryEmailRequest()
            {
                
                Email = emailId,
                EmailTemplateId = ConstantIds.PFAB_EmailTemplateIdKey.Trim()
            };

            PlayFabClientAPI.SendAccountRecoveryEmail(request,
                (result) =>
                {
                    if (successAction != null)
                        successAction.Invoke();
                }, (error) =>
                {
                    if (failureAction != null)
                        failureAction.Invoke(error);
                });

        }
#endregion

        #region PlayerDetails
        public void UpdatePlayerProfilePic(string ImageUrl, Action successAction, Action<PlayFabError> failureAction)
        {
            UpdateAvatarUrlRequest request = new UpdateAvatarUrlRequest()
            {
                ImageUrl = ImageUrl
            };

            PlayFabClientAPI.UpdateAvatarUrl(request,
                result =>
                {

                    if (successAction != null)
                        successAction.Invoke();
                }, error =>
                {
                    if (failureAction != null)
                        failureAction.Invoke(error);
                });
        }
        public void GetPlayerCombinedInfo(GetPlayerCombinedInfoRequest request, Action<GetPlayerCombinedInfoResult> successAction, Action<PlayFabError> failureAction)
        {
            PlayFabClientAPI.GetPlayerCombinedInfo(request, successAction, failureAction);
        }
        public void UpdatePlayerData(UserDataPermission permission, Dictionary<string, string> playerData, Action successAction, Action<PlayFabError> failureAction)
        {
            UpdateUserDataRequest request = new UpdateUserDataRequest()
            {
                Data = playerData,
                Permission = permission
            };

            PlayFabClientAPI.UpdateUserData(request, (result) =>
            {
                if (successAction != null)
                    successAction.Invoke();
            }, (error) =>
            {
                if (failureAction != null)
                    failureAction.Invoke(error);
            }, null);

        }
        public void FetchTitleData(List<string> keys, Action<GetTitleDataResult> successAction, Action<PlayFabError> failureAction)
        {
            GetTitleDataRequest request = new GetTitleDataRequest()
            {
                Keys = keys
            };

            PlayFabClientAPI.GetTitleData(request, (result) =>
            {
                if (successAction != null)
                    successAction.Invoke(result);

            }, (error) =>
            {
                if (failureAction != null)
                    failureAction.Invoke(error);
            }, null);

        }

        public void FetchPlayerData(string playFabId, List<string> keys, Action<GetUserDataResult> successAction, Action<PlayFabError> failureAction)
        {
            GetUserDataRequest request = new GetUserDataRequest()
            {
                Keys = keys,                
                PlayFabId = playFabId
            };

            PlayFabClientAPI.GetUserData(request, (result) =>
            {
                if (successAction != null)
                    successAction.Invoke(result);

            }, (error) =>
            {
                if (failureAction != null)
                    failureAction.Invoke(error);
            }, null);

        }
        public void FetchPlayerReadOnlyData(List<string> keys, Action<GetUserDataResult> successAction, Action<PlayFabError> failureAction)
        {
            GetUserDataRequest request = new GetUserDataRequest()
            {
                Keys = keys
            };

            PlayFabClientAPI.GetUserReadOnlyData(request, (result) =>
            {
                if (successAction != null)
                    successAction.Invoke(result);

            }, (error) =>
            {
                if (failureAction != null)
                    failureAction.Invoke(error);
            }, null);

        }
        #endregion

        #region Leaderboard
        public void UpdatePlayerStatistics(string statisticName, int value, Action<GetPlayerStatisticsResult> successAction)
        {
            PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
            {
                // request.Statistics is a list, so multiple StatisticUpdate objects can be defined if required.
                Statistics = new List<StatisticUpdate> {
                    new StatisticUpdate { StatisticName = statisticName, Value = value },
                }
            },
            result => {
                PlayFabClientAPI.GetPlayerStatistics(
                new GetPlayerStatisticsRequest {
                    StatisticNames = new List<string>
                    { statisticName },
                    },
                result => {
                    if (successAction != null)
                        successAction.Invoke(result);
                },
                error => Debug.LogError(error.GenerateErrorReport())
                );
            },
            error => { Debug.LogError(error.GenerateErrorReport()); });
        }

        public void GetLeaderboardData(string statisticName, int maxResultsCount, Action<GetLeaderboardResult> successAction, Action<PlayFabError> failureAction)
        {
            var profileConstraints = new PlayerProfileViewConstraints { ShowDisplayName = true, ShowAvatarUrl = true };
            var requestLeaderBoard = new GetLeaderboardRequest { StartPosition = 0, StatisticName = statisticName, MaxResultsCount = maxResultsCount, ProfileConstraints = profileConstraints };
            PlayFabClientAPI.GetLeaderboard(requestLeaderBoard, successAction, failureAction);
        }

        public void GetFriendLeaderboardData(string statisticName, int maxResultsCount, Action<List<PlayerLeaderboardEntry>> successAction, Action<PlayFabError> failureAction, bool showLocations = false)
        {
            GetFriendLeaderboardRequest request = new GetFriendLeaderboardRequest()
            {
                IncludeFacebookFriends = true,
                MaxResultsCount = maxResultsCount,
                StartPosition = 0,
                StatisticName = statisticName,
                ProfileConstraints = new PlayerProfileViewConstraints()
                {
                    ShowDisplayName = true,
                    ShowAvatarUrl = true,
                    ShowLastLogin = true,
                    ShowStatistics = true,
                    ShowLocations = showLocations,
                },
            };

            PlayFabClientAPI.GetFriendLeaderboard(request, result => {
                if (successAction != null)
                    successAction.Invoke(result.Leaderboard);
            }, error => {
                if (failureAction != null)
                    failureAction.Invoke(error);
            });
        }
        #endregion

        #region FriendsList

        public void GetFriendsList(bool includeFBFriends, Action<List<FriendInfo>> successAction, Action<PlayFabError> failureAction, bool showLocations = false)
        {         
            GetFriendsListRequest request = new GetFriendsListRequest();

            request.IncludeFacebookFriends = includeFBFriends;
            request.ProfileConstraints = new PlayerProfileViewConstraints();

            request.ProfileConstraints.ShowDisplayName = true;
            request.ProfileConstraints.ShowLastLogin = true;
            request.ProfileConstraints.ShowStatistics = true;
            request.ProfileConstraints.ShowAvatarUrl = true;
            request.ProfileConstraints.ShowLocations = showLocations;

            PlayFabClientAPI.GetFriendsList(request, result => {
                Debug.Log(result.ToJson().ToString());
                if (successAction != null)
                    successAction.Invoke(result.Friends);
            }, error=> {

                if (failureAction != null)
                    failureAction.Invoke(error);
            });
        }

        public void AddPlayerToFriendsList(string playerPlayFabId, Action<bool> successAction, Action<PlayFabError> failureAction, PFFriendIdType idType = PFFriendIdType.PlayFabId)
        {
            var request = new AddFriendRequest();
            switch (idType)
            {
                case PFFriendIdType.PlayFabId:
                    request.FriendPlayFabId = playerPlayFabId;
                    break;
                case PFFriendIdType.Username:
                    request.FriendUsername = playerPlayFabId;
                    break;
                case PFFriendIdType.Email:
                    request.FriendEmail = playerPlayFabId;
                    break;
                case PFFriendIdType.DisplayName:
                    request.FriendTitleDisplayName = playerPlayFabId;
                    break;
            }
            // Execute request and update friends when we are done
            PlayFabClientAPI.AddFriend(request, result => {
                if (successAction != null)
                    successAction.Invoke(result.Created);
            }, error=> {

                if (failureAction != null)
                    failureAction.Invoke(error);
            });
        }
        public void RemovePlayerFromFriendsList(string playerPlayFabId, Action successAction, Action<PlayFabError> failureAction)
        {
            PlayFabClientAPI.RemoveFriend(new RemoveFriendRequest
            {
                FriendPlayFabId = playerPlayFabId
            }, result => {
                if (successAction != null)
                    successAction.Invoke();
            }, error => {
                if (failureAction != null)
                    failureAction.Invoke(error);
            });
        }
        #endregion

        #region Helper
        public void GetCurrentTime(Action<DateTime> successAction, Action<PlayFabError> failureAction)
        {
            PlayFabClientAPI.GetTime(new GetTimeRequest(), (result) => {
                if (successAction != null)
                    successAction.Invoke(result.Time);
            }, failureAction);
        }
        #endregion

        #region Logout
        public void ForgetAllCredentials()
        {
            _playFabPlayerIdCache = string.Empty;
            PlayFabClientAPI.ForgetAllCredentials();
        }
        #endregion

    }

}