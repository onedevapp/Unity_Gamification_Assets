using System;
using System.Collections;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.Networking;

#if UNITY_ANDROID && !UNITY_EDITOR
using UnityEngine.Android;
#endif

namespace OneDevApp
{
    /// <summary>
	/// The Geo data for a user.
	/// You are free to use ip-api.com for non-commercial use. We do not allow commercial use without prior approval.
	/// So if you want to use it in commercial use -> https://signup.ip-api.com/
	/// 
	///Check http://ip-api.com/docs/api:json for commercial and usage limits
	/// 
	/// <code>
	/// {
	/// 	"status": "success",
	/// 	"country": "COUNTRY",
	/// 	"countryCode": "COUNTRY CODE",
	/// 	"region": "REGION CODE",
	/// 	"regionName": "REGION NAME",
	/// 	"city": "CITY",
	/// 	"zip": "ZIP CODE",
	/// 	"lat": LATITUDE,
	/// 	"lon": LONGITUDE,
	/// 	"timezone": "TIME ZONE",
	/// 	"isp": "ISP NAME",
	/// 	"org": "ORGANIZATION NAME",
	/// 	"as": "AS NUMBER / NAME",
	/// 	"query": "IP ADDRESS USED FOR QUERY"
	/// }
	/// </code>
	/// 
	/// </summary>
	public class LocationData
    {
        /// <summary>
        /// The status that is returned if the response was successful.
        /// </summary>
        public string status;
        public string country;
        public string countryCode;
        public string region;
        public string regionName;
        public string city;
        public string zip;
        public string lat;
        public string lon;
        public string timezone;
        public string isp;
        public string org;
        public string query;
    }

    public enum LocationStatusEnum
    {
        UNKNOWN,
        DISABLED_BY_USER,
        USER_NOT_AUTHORIZED,
        FAILED,
        FETCHED
    }

    public class UserDeviceInfo : MonoInstance<UserDeviceInfo>
    {
        LocationStatusEnum _locationStatus = LocationStatusEnum.UNKNOWN;
        public LocationStatusEnum LocationStatus { get { return _locationStatus; } }
        public string GetLatitude { get; private set; }
        public string GetLongitude { get; private set; }

        bool isLocationUpdating = false;

        public string DeviceLocationJson
        {
            get
            {
                return "{location :{\"lat\":" + GetLatitude + ",\"lng\":" + GetLongitude + "}}";
            }
        }
        public string DeviceUniqueIdentifier
        {
            get
            {
                var deviceId = "";
#if UNITY_ANDROID && !UNITY_EDITOR
                AndroidJavaClass up = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = up.GetStatic<AndroidJavaObject> ("currentActivity");
                AndroidJavaObject contentResolver = currentActivity.Call<AndroidJavaObject> ("getContentResolver");
                AndroidJavaClass secure = new AndroidJavaClass ("android.provider.Settings$Secure");
                deviceId = secure.CallStatic<string> ("getString", contentResolver, "android_id");
#else
                deviceId = SystemInfo.deviceUniqueIdentifier;
#endif
                return deviceId;
            }
        }

        public string GetMacAddress
        {
            get
            {
                string result = "";
                foreach (NetworkInterface ninf in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (ninf.NetworkInterfaceType != NetworkInterfaceType.Ethernet) continue;
                    if (ninf.OperationalStatus == OperationalStatus.Up)
                    {
                        result += ninf.GetPhysicalAddress().ToString();
                        break;
                    }
                }
                return result;
            }
        }
        public string GetIMEI
        {
            get
            {
                var deviceIMEI = "";
#if UNITY_ANDROID && !UNITY_EDITOR
                AndroidJavaObject TM = new AndroidJavaObject("android.telephony.TelephonyManager");
                deviceIMEI = TM.Call<string>("getDeviceId");
#endif
                return deviceIMEI;
            }
        }


        public void FetchLocation(Action OnComplete)
        {
            _locationStatus = LocationStatusEnum.UNKNOWN;

#if UNITY_ANDROID && !UNITY_EDITOR

            if (!isLocationEnabledByUser())
            {
                _locationStatus = LocationStatusEnum.DISABLED_BY_USER;
                OnComplete?.Invoke();
            }
            
            if (!HasUserAuthorizedLocationPermission())
            {
                _locationStatus = LocationStatusEnum.USER_NOT_AUTHORIZED;
                OnComplete?.Invoke();
            }

            if (!isLocationUpdating)
            {
                isLocationUpdating = true;
                StartCoroutine(StartLocationUpdate(OnComplete));
            }
#elif UNITY_EDITOR
            //Chennai Location
            GetLatitude = "13°04'N";
            GetLongitude = "80°17'E";

            _locationStatus = LocationStatusEnum.FETCHED;
            OnComplete?.Invoke();
#endif

        }

        IEnumerator StartLocationFromIP(Action<LocationData> Done)
        {
            using (UnityWebRequest www = UnityWebRequest.Get("http://ip-api.com/json"))
            {
                www.timeout = 45;
                yield return www.SendWebRequest();

                while (!www.isDone)
                    yield return www;

                if(www.result == UnityWebRequest.Result.Success)
                {
                    LocationData data = JsonUtility.FromJson<LocationData>(www.downloadHandler.text);
                    Done(data);
                }
                else
                {
                    Done(null);
                }
            }
        }

        IEnumerator StartLocationUpdate(Action OnComplete)
        {
            if (!Input.location.isEnabledByUser)
            {
                isLocationUpdating = false;
                yield break;
            }

            Input.location.Start(1, 0.1f);
            // Wait until service initializes
            int maxWait = 20;
            while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
            {
                yield return new WaitForSeconds(1);
                maxWait--;
            }

            // Service didn't initialize in 20 seconds
            if (maxWait < 1)
            {
                isLocationUpdating = false;
                yield break;
            }

            // Connection has failed
            if (Input.location.status == LocationServiceStatus.Failed)
            {
                isLocationUpdating = false;

                _locationStatus = LocationStatusEnum.FAILED;
                OnComplete?.Invoke();
                yield break;
            }
            else
            {
                GetLatitude = Input.location.lastData.latitude.ToString();
                GetLongitude = Input.location.lastData.longitude.ToString();

                _locationStatus = LocationStatusEnum.FETCHED;
                OnComplete?.Invoke();
            }
            // Stop service if there is no need to query location updates continuously
            Input.location.Stop();
            isLocationUpdating = false;
        }

        bool HasUserAuthorizedLocationPermission()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                return false;
                Permission.RequestUserPermission(Permission.FineLocation);
            }
#endif
            return true;
        }

        bool isLocationEnabledByUser()
        {
            if (Input.location.isEnabledByUser)
            {
                return true;
            }
            return false;
        }
    }

}