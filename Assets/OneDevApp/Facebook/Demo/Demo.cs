using Facebook.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OneDevApp.FB_Demo
{
    public class Demo : MonoBehaviour
    {
        public Text m_LoginBtnText;
        public Text m_UserNameText;
        public Image m_UserProfileImage;
        public GameObject m_FriendListItem;
        public GameObject m_ScoreListItem;

        public GameObject m_ListPanel;
        public Transform m_ListItemParent;

        private void Start()
        {
            m_LoginBtnText.text = "Initializing";
            FacebookManager.Instance.OnFBInit += OnFBInit;
        }

        private void OnFBInit()
        {
            ToggleUIElementsInteractable(false);
            
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


        public void ToggleFacebookLogin()
        {
            if (FacebookManager.Instance.isLoggedIn)
            {
                FacebookManager.Instance.FacebookLogout();
                ToggleUIElementsInteractable(false);
                m_UserProfileImage.sprite = null;
                m_UserNameText.text =string.Empty;
            }
            else
            {
                //OnFBInit();
                FacebookManager.Instance.FacebookLogIn(result => {
                    m_UserProfileImage.sprite = result.profilePic;
                    m_UserNameText.text = "Hello " + result.profileName;

                    ToggleUIElementsInteractable(true);
                }, error => {
                    ToggleUIElementsInteractable(false);
                });
            }
        }

        public void FacebookShareLink()
        {
            FacebookManager.Instance.FacebookShareLink(ConstantIds.FB_SHARELINK,
                ConstantIds.FB_SHARELINK_TITLE,
                ConstantIds.FB_SHARELINK_DESC,
                ConstantIds.FB_SHARELINK_LOGO);
        }

        public void FacebookFeedShare()
        {
            FacebookManager.Instance.FacebookFeedShare(ConstantIds.FB_FEEDSHARELINK, //link
                ConstantIds.FB_FEEDSHARELINK_TITLE, //linkName
                ConstantIds.FB_FEEDSHARELINK_CAPTION, //linkCaption
                ConstantIds.FB_FEEDSHARELINK_DESC, //linkDescription
                ConstantIds.FB_FEEDSHARELINK_PICTURE, //picture
                //string.Empty, //mediaSource
                (errorMessage) =>
                {
                    Debug.Log("FacebookFeedShare::"+errorMessage);
                } //callback
                );
        }

        public void FacebookAppRequest()
        {
            FacebookManager.Instance.FacebookAppRequest(ConstantIds.FB_APPREQUEST_MSG, ConstantIds.FB_APPREQUEST_TITLE);
        }

        public void FacebookSetScore()
        {
            FacebookManager.Instance.SetFacebookScore(UnityEngine.Random.Range(100, 1000).ToString(), ()=> {
                Debug.Log("FacebookSetScore::posted successfully");
            }, errorMessage => {
                Debug.Log("FacebookSetScore::" + errorMessage);
            });
        }

        public void FacebookGetScoresList()
        {
            FacebookManager.Instance.GetFacebookScores(result=> {
                int srNo = 1;
                foreach (object obj in result)
                {
                    Dictionary<string, object> dictio = (Dictionary<string, object>)obj;
                    InstantiateScoreItem(srNo.ToString(), dictio["name"].ToString(), dictio["id"].ToString(), dictio["score"].ToString());
                    srNo++;
                }
            }, error=> {
                m_ListPanel.SetActive(false);
                Debug.Log("Error during scores list");
            });
        }

        void InstantiateScoreItem(string srNo, string userName, string userId, string userScore)
        {
            GameObject ScoreRowItem = Instantiate(m_ScoreListItem) as GameObject;
            ScoreRowItem.transform.SetParent(m_ListItemParent, false);

            ScoreRowItem.transform.Find("Srno").GetComponent<Text>().text = srNo;
            ScoreRowItem.transform.Find("FriendName").GetComponent<Text>().text = userName;
            ScoreRowItem.transform.Find("FriendScore").GetComponent<Text>().text = userScore;

            ScoreRowItem.transform.Find("FriendAvatar").GetComponent<Image>();

            FacebookManager.Instance.GetProfilePicFB(userId, result => {
                ScoreRowItem.transform.Find("FriendAvatar").GetComponent<Image>().sprite = result;
            }, error => {
                Debug.Log("Error during profile picture for user -" + userName);
            });
        }


        public void FacebookGetFriendsList()
        {
            FacebookManager.Instance.GetFacebookFriends(result=> {
                m_ListPanel.SetActive(true);
                foreach (object obj in result)
                {
                    Dictionary<string, object> dictio = (Dictionary<string, object>)obj;
                    InstantiateFriendItem(dictio["name"].ToString(), dictio["id"].ToString());
                }
            }, error=> {
                m_ListPanel.SetActive(false);
                Debug.Log("Error during friends list");
            });
        }


        void InstantiateFriendItem(string userName, string userId)
        {
            GameObject FriendRowItem = Instantiate(m_FriendListItem) as GameObject;
            FriendRowItem.transform.SetParent(m_ListItemParent, false);

            FriendRowItem.transform.Find("FriendName").GetComponent<Text>().text = userName;

            FacebookManager.Instance.GetProfilePicFB(userId, result => { 
                FriendRowItem.transform.Find("FriendAvatar").GetComponent<Image>().sprite = result; 
            }, error => {
                Debug.Log("Error during profile picture for user -" + userName);
            });
        }
    }

}