using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OneDevApp
{
    public class LocationDemo : MonoBehaviour
    {
        private string m_LongitudeValue;
        private string m_LatitudeValue;
        public GameObject refreshBtn;
        public Text infoText;

        // Start is called before the first frame update
        void Start()
        {
            m_LongitudeValue = string.Empty;
            m_LatitudeValue = string.Empty;

#if UNITY_ANDROID || UNITY_IOS
        // turn on location services, if available 
        if (!Input.location.isEnabledByUser)
        {
            refreshBtn.SetActive(true);
            Input.location.Start();
        }
        else
        {
            refreshBtn.SetActive(false);
            StartCoroutine(GetLocationValues());
        }
#else
            infoText.text = "Unsupported Platform";
            refreshBtn.SetActive(false);
#endif
        }


        public void checkForLocation()
        {
#if UNITY_ANDROID || UNITY_IOS
        if (Input.location.isEnabledByUser)
        {
            StartCoroutine(GetLocationValues());
        }
#endif
        }


#if UNITY_ANDROID || UNITY_IOS
    IEnumerator GetLocationValues()
    {
        // First, check if user has location service enabled
        if (!Input.location.isEnabledByUser)
            yield break;

        // Start service before querying location
        Input.location.Start();

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
            print("Timed out");
            yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            infoText.text = "Unable to determine device location. Please click refreshBtn button";
            refreshBtn.SetActive(true);
            Debug.Log("Unable to determine device location");
            yield break;
        }
        else
        {
            refreshBtn.SetActive(false);
            m_LatitudeValue = Input.location.lastData.latitude.ToString();
            m_LongitudeValue = Input.location.lastData.longitude.ToString();
            // Access granted and location value could be retrieved
            Debug.Log("Location: " + m_LatitudeValue + " " + m_LongitudeValue + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
            Debug.Log("Latitude Value : " + m_LatitudeValue);
            Debug.Log("Longitude Value : " + m_LongitudeValue);

            infoText.text = "Latitude Value : " + m_LatitudeValue + ",      Longitude Value : " + m_LongitudeValue;
        }

        // Stop service if there is no need to query location updates continuously
        Input.location.Stop();
    }
#endif
    }

}