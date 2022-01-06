
namespace OneDevApp
{

    public enum PFErrorCode
    {
        General_Http_Error = -1,
        EmailId_Empty = 99000,
        EmailId_Invalid = 99001,
        EmailId_NotVerified = 99002,
        Password_Empty = 99003,
        Password_Invalid_Len = 99004,
        NickName_Invalid_Len = 99005,
        NickName_Empty = 99006,
        MobileNo_Empty = 99007,
        MobileNo_Invalid_Len = 99008,
        Referral_Empty = 99009,
        Referral_Invalid_Len = 99010,
        Referral_Invalid = 99011,
        Facebook_ID_Invalid = 99012,
        FB_ProfileUserId_Empty = 99013,
        OTP_Empty = 99014,
        OTP_Invalid = 99015,
        OTP_Expired = 99016,
        PlayFabID_Empty = 99017,
        Terms_NotAccepted = 99018,
        AgeLimit_NotAccepted = 99019,
        Photon_Custom_Auth_Failed = 99020

    }

    public enum ProfilePicType
    {
        None = 0,
        Index,
        Url,
        Facebook,
        FBIndex,
    }

    public enum OnOffOption
    {
        OFF = 0,
        ON,
    }

    public enum LoginType
    {
        None = 0,
        MobileNo,
        EmailId,
        Facebook,
        DeviceId
    }

    public enum PFFriendIdType { PlayFabId, Username, Email, DisplayName };
}