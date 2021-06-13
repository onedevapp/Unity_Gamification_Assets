using Facebook.Unity;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace OneDevApp
{
    [Serializable]
    public struct FacebookDetailsResult
    {
        public string accessToken;
        public string profileUserId;
        public string profileName;
        public string profileFirstName;
        public Sprite profilePic;
    }

    public class FacebookManager : MonoInstance<FacebookManager>
    {
        #region Public Variables
        public Action OnFBInit;

        public FacebookDetailsResult facebookDetails;
        public bool isLoggedIn
        {
            get
            {
                return FB.IsLoggedIn;
            }
        }
        #endregion


        #region FB Init
        protected override void Awake()
        {
            base.Awake();
            if (!FB.IsInitialized)
            {
                FB.Init(() =>
                {
                    if (FB.IsInitialized)
                        SetInit();
                    else
                        Debug.LogError("FB Couldn't initialize");
                },
                isGameShown =>
                {
                    if (!isGameShown)
                        Time.timeScale = 0;
                    else
                        Time.timeScale = 1;
                });
            }
            else
                SetInit();
        }

        private void SetInit()
        {
            FB.ActivateApp();
            if (OnFBInit != null) OnFBInit.Invoke();
        }

        #endregion

        #region Login / Logout
        public void FacebookLogout()
        {
            if (FB.IsLoggedIn)
            {
                FB.LogOut();
            }
        }

        public void FacebookLogIn(Action<FacebookDetailsResult> success, Action<string> error)
        {
            if (FB.IsLoggedIn)
            {
                InitSession(success, error);
            }
            else
            {
                var permissions = new List<string>() { "public_profile", "user_friends", "gaming_user_picture", "gaming_profile", "email" };
                FB.LogInWithReadPermissions(permissions, (result) =>
                {
                    if (result.Error == null)
                    {
                        InitSession(success, error);
                    }
                    else
                    {
                        if (error != null)
                            error.Invoke(result.Error);
                    }
                });
            }
        }

        private void InitSession(Action<FacebookDetailsResult> success, Action<string> error)
        {
            facebookDetails = new FacebookDetailsResult();
            //get profile picture
            GetProfilePicFB("me", (Sprite pic) => { 
                facebookDetails.profilePic = pic;
                //get username
                GetPublicDetailsFB("me", (IDictionary<string, object> result) => {

                    facebookDetails.profileUserId = AccessToken.CurrentAccessToken.UserId;
                    facebookDetails.accessToken = AccessToken.CurrentAccessToken.TokenString;

                    facebookDetails.profileFirstName = result["first_name"].ToString();
                    facebookDetails.profileName = result["name"].ToString();

                    if (success != null)
                        success.Invoke(facebookDetails);
                }, (string errorMessage) => {
                    if (error != null)
                        error.Invoke(errorMessage);
                });
            }, (string errorMessage) => {
                if (error != null)
                    error.Invoke(errorMessage);
            });
            

        }
        #endregion

        #region User Details       
        private void GetPublicProfilePicFB(string facebookID, Action<Sprite> onSuccess, Action<string> onError)
        {
            ///Need to upadte to OCT 2020 version, which requires access token to get the profile pic
            //https://graph.facebook.com/oauth/access_token?client_id={your-app-id}&client_secret={your-app-secret}&grant_type=client_credentials
            //https://graph.facebook.com/{profile_id}/picture?type=large&access_token={app_access_token}
            StartCoroutine(getPublicProfilePicFB(facebookID, onSuccess, onError));
        }

        private IEnumerator getPublicProfilePicFB(string facebookID, Action<Sprite> onSuccess, Action<string> onError)
        {

            using (UnityWebRequest wwwAccessToken = UnityWebRequest.Get("https://graph.facebook.com/oauth/access_token?client_id=" + ConstantIds.FB_APP_ID + "&client_secret=" + ConstantIds.FB_APP_SECRET + "&grant_type=client_credentials"))
            {
                wwwAccessToken.timeout = 30;
                //Send request
                yield return wwwAccessToken.SendWebRequest();

                while (!wwwAccessToken.isDone)
                    yield return wwwAccessToken;

                while (!wwwAccessToken.downloadHandler.isDone)
                    yield return null;

                if (wwwAccessToken.result == UnityWebRequest.Result.Success)
                {

                    JSONNode result = JSONNode.Parse(wwwAccessToken.downloadHandler.text);

                    using (UnityWebRequest www = UnityWebRequestTexture.GetTexture("https://graph.facebook.com/"+ facebookID + "/picture?type=large&access_token="+ result["access_token"]))
                    {
                        www.timeout = 30;
                        //Send request
                        yield return www.SendWebRequest();

                        while (!www.isDone)
                            yield return www;

                        while (!www.downloadHandler.isDone)
                            yield return null;

                        if (www.result == UnityWebRequest.Result.Success)
                        {

                            var imageTex = ((DownloadHandlerTexture)www.downloadHandler).texture;
                            var _tex = DoReScaleTex(imageTex, 512, 512);
                            Sprite sprite = CreateSpriteFromTex(imageTex, imageTex.width, imageTex.height);
                            onSuccess.Invoke(sprite);
                        }
                        else
                        {
                            if (onError != null)
                                onError.Invoke(www.error);
                        }
                    }
                }
                else
                {
                    if (onError != null)
                        onError.Invoke(wwwAccessToken.error);
                }
            }
        }

        public void GetProfilePicFB(string facebookID, Action<Sprite> onSuccess, Action<string> onError, int width = 512, int height = 512)
        {
            if (FB.IsLoggedIn)
            {
                FB.API(GetPictureURL(facebookID), HttpMethod.GET, (result) =>
                {
                    if (result.Error != null)
                    {
                        if (onError != null)
                            onError.Invoke(result.Error);
                    }
                    else
                    {
                        //byte[] _bytes = result.Texture.EncodeToPNG();
                        //System.IO.File.WriteAllBytes(Application.persistentDataPath+"/"+facebookID+"_12.png", _bytes);
                        if (onSuccess != null)
                        {
                            var _tex = DoReScaleTex(result.Texture, width, height);
                            Sprite sprite = CreateSpriteFromTex(result.Texture, result.Texture.width, result.Texture.height);
                            onSuccess.Invoke(sprite);
                        }
                    }
                });
            }
            else
            {
                GetPublicProfilePicFB(facebookID, onSuccess, onError);
            }
        }

        public void GetPublicDetailsFB(string facebookID, Action<IDictionary<string, object>> onSuccess, Action<string> onError)
        {
            FB.API(GetPublicDetailsURL(facebookID), HttpMethod.GET, (result) =>
            {
                if (result.Error != null)
                {
                    if (onError != null)
                        onError.Invoke(result.Error);
                }
                else
                {
                    if (onSuccess != null)
                        onSuccess.Invoke(result.ResultDictionary);
                }
            });

        }
        #endregion


        #region Sharing
        public void FacebookShareLink(string shareLinkUri, string shareLinkTitle, string shareLinkDesc, string shareLinkLogo)
        {
            if (string.IsNullOrEmpty(shareLinkUri))
            {
                Debug.Log("shareLinkUri cant be empty");
                return;
            }
            if (string.IsNullOrEmpty(shareLinkTitle))
            {
                Debug.Log("shareLinkTitle cant be empty");
                return;
            }
            if (string.IsNullOrEmpty(shareLinkDesc))
            {
                Debug.Log("shareLinkDesc cant be empty");
                return;
            }
            if (string.IsNullOrEmpty(shareLinkLogo))
            {
                Debug.Log("shareLinkLogo cant be empty");
                return;
            }

            FB.ShareLink(new System.Uri(shareLinkUri),
                shareLinkTitle,
                shareLinkDesc,
                new System.Uri(shareLinkLogo));
        }

        public void FacebookFeedShare(string feedShareLink, string feedShareLinkTitle, string feedShareLinkCaption, string feedShareLinkDesc, string feedShareLinkPic, Action<string> onError)
        {
            if (string.IsNullOrEmpty(feedShareLink))
            {
                Debug.Log("feedShareLink cant be empty");
                return;
            }
            if (string.IsNullOrEmpty(feedShareLinkTitle))
            {
                Debug.Log("feedShareLinkTitle cant be empty");
                return;
            }
            if (string.IsNullOrEmpty(feedShareLinkCaption))
            {
                Debug.Log("feedShareLinkCaption cant be empty");
                return;
            }
            if (string.IsNullOrEmpty(feedShareLinkDesc))
            {
                Debug.Log("feedShareLinkDesc cant be empty");
                return;
            }
            if (string.IsNullOrEmpty(feedShareLinkPic))
            {
                Debug.Log("feedShareLinkPic cant be empty");
                return;
            }

            FB.FeedShare(
                string.Empty, //toId
                new System.Uri(feedShareLink), //link
                feedShareLinkTitle, //linkName
                feedShareLinkCaption, //linkCaption
                feedShareLinkDesc, //linkDescription
                new System.Uri(feedShareLinkPic), //picture
                string.Empty, //mediaSource
                (result) =>
                    {
                        if (result.Error != null)
                        {
                            if (onError != null)
                                onError.Invoke(result.Error);
                        }
                    } //callback
                );
        }

        #endregion

        #region Inviting
        public void FacebookAppRequest(string appRequestMsg, string appRequestTitle)
        {
            FB.AppRequest(message: appRequestMsg, title: appRequestTitle);
        }
        #endregion

        #region Friends
        public void GetFacebookFriends(Action<List<object>> onSuccess, Action<string> onError)
        {
            //get friends list
            FB.API("me/friends", HttpMethod.GET, (result) =>
            {
                if (result.Error != null)
                {
                    if (onError != null)
                        onError.Invoke(result.Error);
                }
                else
                {
                    IDictionary<string, object> data = result.ResultDictionary;
                    List<object> scores = (List<object>)data["data"];

                    if (onSuccess != null)
                        onSuccess.Invoke(scores);
                }
            });

            //FB.API("/me/invitable_friends?limit=" + _Limit, HttpMethod.GET, GetFriendsListCallBack);
        }



        #endregion

        #region Scores
        public void SetFacebookScore(string currentScore, Action onSuccess, Action<string> onError)
        {
            var scoreData = new Dictionary<string, string>() { { "score", currentScore } };

            FB.API("/me/scores", HttpMethod.POST, (result) =>
            {
                if (result.Error != null)
                {
                    if (onError != null)
                        onError.Invoke(result.Error);
                }
                else
                {
                    if (onSuccess != null)
                        onSuccess.Invoke();
                }
            }, scoreData);
        }

        public void GetFacebookScores(Action<List<object>> onSuccess, Action<string> onError)
        {
            FB.API("/app/scores?fields=score,user.limit(30)", HttpMethod.GET, (result) =>
            {
                if (result.Error != null)
                {
                    if (onError != null)
                        onError.Invoke(result.Error);
                }
                else
                {
                    IDictionary<string, object> data = result.ResultDictionary;
                    List<object> scores = (List<object>)data["data"];

                    if (onSuccess != null)
                        onSuccess.Invoke(scores);
                }
            });
        }



        #endregion

        #region Helper Functions
        public Sprite CreateSpriteFromTex(Texture2D spriteTexture, float width = 128f, float height = 128f)
        {
            return Sprite.Create(spriteTexture, new Rect(0, 0, width, height), Vector2.zero);
        }

        public Texture2D DoReScaleTex(Texture2D tex, int width, int height)
        {
            Texture2D scaled = new Texture2D(width, height, TextureFormat.BGRA32, true);
            Graphics.ConvertTexture(tex, scaled);
            return scaled;
        }

        private string GetPublicDetailsURL(string facebookID)
        {
            string url = string.Format("/{0}", facebookID);
            string query = "?fields=id,name,first_name,email";
            if (!string.IsNullOrEmpty(query)) url += (query);
            return url;
        }

        private string GetPictureURL(string facebookID, int? width = null, int? height = null, string type = "large")
        {
            string url = string.Format("/{0}/picture", facebookID);
            string query = width != null ? "&width=" + width.ToString() : "";
            query += height != null ? "&height=" + height.ToString() : "";
            query += type != null ? "&type=" + type : "";
            if (!string.IsNullOrEmpty(query)) url += ("?g" + query);
            return url;
        }

        public Dictionary<string, string> RandomFriend(List<object> friends)
        {
            var fd = ((Dictionary<string, object>)(friends[UnityEngine.Random.Range(0, friends.Count - 1)]));
            var friend = new Dictionary<string, string>();
            friend["id"] = (string)fd["id"];
            friend["first_name"] = (string)fd["first_name"];
            return friend;
        }
        #endregion
    }

}