using Facebook.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OneDevApp
{
    public class FacebookManager : MonoInstance<FacebookManager>
    {
        #region Public Variables
        public Text m_LoginBtnText;
        public Text m_UserNameText;
        public Image m_UserProfileImage;
        public GameObject m_FriendListItem;
        public GameObject m_ScoreListItem;

        public GameObject m_ListPanel;
        public Transform m_ListItemParent;
        #endregion


        #region FB Init
        protected override void Awake()
        {
            if (!FB.IsInitialized)
            {
                FB.Init(() =>
                {
                    if (FB.IsInitialized)
                        SetInit();
                    else
                        Debug.LogError("Couldn't initialize");
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
            Debug.Log("FB Init done");
            FB.ActivateApp();
            ToggleFBMenus(FB.IsLoggedIn);
        }

        private void ToggleFBMenus(bool isLoggedIn)
        {
            if (isLoggedIn)
            {
                //get profile picture
                FB.API(GetPictureURL("me", 128, 128), HttpMethod.GET, GetProfilePictureCallBack);

                //get username
                FB.API("/me?fields=id,first_name", HttpMethod.GET, GetUserNameCallBack);
            }

            ToggleUIElementsInteractable(isLoggedIn);
        }

        private void ToggleUIElementsInteractable(bool isInteractable)
        {
            if (isInteractable)
            {
                m_LoginBtnText.text = "Logout";
            }
            else
            {
                m_LoginBtnText.text = "Login";
            }
        }

        #endregion

        #region Login / Logout
        public void ToggleFacebookLogin()
        {
            if (FB.IsLoggedIn)
            {
                FB.LogOut();
            }
            else
            {
                var permissions = new List<string>() { "public_profile", "email", "user_friends" };
                FB.LogInWithReadPermissions(permissions, LogInAuthCallback);
            }
        }

        void LogInAuthCallback(IResult result)
        {
            if (result.Error == null)
            {
                Debug.Log("FB login has no error.");
                ToggleFBMenus(FB.IsLoggedIn);
            }
            else
            {
                Debug.Log("Error during login: " + result.Error);
            }
        }
        #endregion

        #region User Details
        void GetProfilePictureCallBack(IGraphResult result)
        {
            if (result.Error != null)
            {
                Debug.Log("Error during profile picture");
                return;
            }
            else
            {
                m_UserProfileImage.sprite = CreateSprite(result.Texture);
            }
        }

        void GetUserNameCallBack(IGraphResult result)
        {
            if (result.Error != null)
            {
                Debug.Log("Error during username");
                return;
            }
            else
            {
                IDictionary<string, object> profile = result.ResultDictionary;
                m_UserNameText.text = "Hello " + profile["first_name"];
            }
        }

        #endregion


        #region Sharing
        public void FacebookShareLink()
        {
            FB.ShareLink(new System.Uri(ConstantIds.fb_sharelink),
                ConstantIds.fb_sharelink_title,
                ConstantIds.fb_sharelink_desc,
                new System.Uri(ConstantIds.fb_sharelink_logo));
        }

        public void FacebookFeedShare()
        {
            FB.FeedShare(
                string.Empty, //toId
                new System.Uri(ConstantIds.fb_feedsharelink), //link
                ConstantIds.fb_feedsharelink_title, //linkName
                ConstantIds.fb_feedsharelink_caption, //linkCaption
                ConstantIds.fb_feedsharelink_desc, //linkDescription
                new System.Uri(ConstantIds.fb_feedsharelink_picture), //picture
                string.Empty, //mediaSource
                LogCallback //callback
                );
        }

        void LogCallback(IResult result)
        {
            if (result.Error != null)
            {
                Debug.Log("Share didn't work");

            }
            else
            {
                Debug.Log("Share worked");
            }

        }

        #endregion

        #region Inviting
        public void FacebookAppRequest()
        {
            FB.AppRequest(message: ConstantIds.fb_apprequest_msg, title: ConstantIds.fb_apprequest_title);
        }
        #endregion

        #region Friends
        public void GetFacebookFriends()
        {
            //get friends list
            FB.API("me/friends", HttpMethod.GET, GetFriendsListCallBack);

            //FB.API("/me/invitable_friends?limit=" + _Limit, HttpMethod.GET, GetFriendsListCallBack);
        }


        void GetFriendsListCallBack(IGraphResult result)
        {
            if (result.Error != null)
            {
                m_ListPanel.SetActive(false);
                Debug.Log("Error during friends list");
                return;
            }
            else
            {
                m_ListPanel.SetActive(true);
                IDictionary<string, object> data = result.ResultDictionary;
                List<object> friends = (List<object>)data["data"];
                foreach (object obj in friends)
                {
                    Dictionary<string, object> dictio = (Dictionary<string, object>)obj;
                    InstantiateFriendItem(dictio["name"].ToString(), dictio["id"].ToString());
                }
            }
        }

        void InstantiateFriendItem(string userName, string userId)
        {
            GameObject FriendRowItem = Instantiate(m_FriendListItem) as GameObject;
            FriendRowItem.transform.SetParent(m_ListItemParent, false);

            FriendRowItem.transform.Find("FriendName").GetComponent<Text>().text = userName;

            FB.API(GetPictureURL(userId, 100, 100), HttpMethod.GET, delegate (IGraphResult result)
            {
                if (result.Error != null)
                {
                    Debug.Log("Error during profile picture for user -" + userName);
                    return;
                }
                else
                {
                    FriendRowItem.transform.Find("FriendAvatar").GetComponent<Image>().sprite = CreateSprite(result.Texture);
                }
            });
        }

        #endregion

        #region Scores
        public void SetFacebookScore()
        {
            var scoreData = new Dictionary<string, string>() { { "score", Random.Range(10, 200).ToString() } };

            FB.API("/me/scores", HttpMethod.POST, delegate (IGraphResult result)
            {
                Debug.Log("Score submit result: " + result.RawResult);
            }, scoreData);
        }

        public void GetFacebookScores()
        {
            FB.API("/app/scores?fields=score,user.limit(30)", HttpMethod.GET, GetScoresCallback);
        }

        private void GetScoresCallback(IResult result)
        {
            if (result.Error != null)
            {
                m_ListPanel.SetActive(false);
                Debug.Log("Error during scores list");
                return;
            }
            else
            {
                m_ListPanel.SetActive(true);
                IDictionary<string, object> data = result.ResultDictionary;
                List<object> scores = (List<object>)data["data"];
                int srNo = 1;
                foreach (object obj in scores)
                {
                    Dictionary<string, object> dictio = (Dictionary<string, object>)obj;
                    InstantiateScoreItem(srNo.ToString(), dictio["name"].ToString(), dictio["id"].ToString(), dictio["score"].ToString());
                    srNo++;
                }
            }
        }

        void InstantiateScoreItem(string srNo, string userName, string userId, string userScore)
        {
            GameObject ScoreRowItem = Instantiate(m_ScoreListItem) as GameObject;
            ScoreRowItem.transform.SetParent(m_ListItemParent, false);

            ScoreRowItem.transform.Find("Srno").GetComponent<Text>().text = srNo;
            ScoreRowItem.transform.Find("FriendName").GetComponent<Text>().text = userName;
            ScoreRowItem.transform.Find("FriendScore").GetComponent<Text>().text = userScore;

            ScoreRowItem.transform.Find("FriendAvatar").GetComponent<Image>();

            FB.API(GetPictureURL(userId, 100, 100), HttpMethod.GET, delegate (IGraphResult result)
            {
                if (result.Error != null)
                {
                    Debug.Log("Error during profile picture for user -" + userName);
                    return;
                }
                else
                {
                    ScoreRowItem.transform.Find("FriendAvatar").GetComponent<Image>().sprite = CreateSprite(result.Texture);
                }
            });
        }

        #endregion

        #region Helper Functions
        public Sprite CreateSprite(Texture2D spriteTexture, float width = 128f, float height = 128f)
        {
            return Sprite.Create(spriteTexture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
        }

        public string GetPictureURL(string facebookID, int? width = null, int? height = null, string type = null)
        {
            string url = string.Format("/{0}/picture", facebookID);
            string query = width != null ? "&width=" + width.ToString() : "";
            query += height != null ? "&height=" + height.ToString() : "";
            query += type != null ? "&type=" + type : "";
            if (query != "") url += ("?g" + query);
            return url;
        }

        public Dictionary<string, string> RandomFriend(List<object> friends)
        {
            var fd = ((Dictionary<string, object>)(friends[Random.Range(0, friends.Count - 1)]));
            var friend = new Dictionary<string, string>();
            friend["id"] = (string)fd["id"];
            friend["first_name"] = (string)fd["first_name"];
            return friend;
        }
        #endregion
    }

}