using UnityEngine;

namespace OneDevApp.LoggerUtil_Demo
{


    public class Demo : MonoBehaviour
    {
        private int tapCount = 0;

        private void Awake()
        {
#if UNITY_EDITOR
            LoggerUtils.ToogleLogOnDevice(true);
            LoggerUtils.SetLogProfile(LogProfile.UnityDebug);
#endif
        }

        // Start is called before the first frame update
        void Start()
        {
           
            LoggerUtils.LogWarning("FirstLog");
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                tapCount++;
                LoggerUtils.Log("Tap Count ::"+tapCount);

            }
        }
    }

}