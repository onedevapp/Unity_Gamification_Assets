using UnityEngine;

namespace OneDevApp.LoggerUtil
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
            Debug.LogWarning("FirstLog");
            LoggerUtils.LogWarning(this, "FirstLog");
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                tapCount++;
                LoggerUtils.LogSuccess(this, "Success");

            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                tapCount++;
                Debug.Log("Tap Count ::" + tapCount);
                LoggerUtils.Log(this, "Tap Count ::" + tapCount);

            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                tapCount++;
                Debug.LogError("Pressed Escape!");
                LoggerUtils.LogError(this, "Pressed Escape!");

            }
        }
    }

}