using PlayFab.ClientModels;
using System;
using UnityEngine;

namespace OneDevApp
{

    [Serializable]
    public struct RegisterDetailsRequest
    {
        public string emailId;
        public string passWord;
        public string rePassWord;
        public string nickName;
        public string mobileNo;
        public string referralCode;
        public bool termsToggle;
        public bool ageLimitToggle;
    }

    public struct PlayFabControllerError
    {
        public int errorCode;
        public string errorMessage;
    }

}