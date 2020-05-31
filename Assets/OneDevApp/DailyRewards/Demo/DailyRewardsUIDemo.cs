using OneDevApp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewardsUIDemo : MonoBehaviour
{
    [Header("Panel Reward")]
    public GameObject panelRewardList;          // Rewards list panel
    public Button buttonClaim;                  // Claim Button
    public Button buttonCloseWindow;            // Close Button on the upper right corner
    public Text textTimeDue;                    // Text showing how long until the next claim
    public GameObject dailyRewardPrefab;        // The Grid that contains the rewards
    public Transform dailyRewardsParentGo;      // The Scroll Rect

    [Header("Panel Reward Message")]
    public GameObject panelReward;              // Rewards panel
    public Text textReward;                     // Reward Text to show an explanatory message to the player
    public Button buttonCloseReward;            // The Button to close the Rewards Panel
    public Image imageReward;                   // The image of the reward

    [Header("Panel Reward Error")]
    public GameObject panelError;               // Rewards error panel
    public Text textRewardError;                // Text showing how long until the next claim

    private bool readyToClaim;                  // Update flag

    DailyRewardsManager rewardsManager;

    private void Awake()
    {
        rewardsManager = DailyRewardsManager.Instance;
    }

    // Start is called before the first frame update
    void Start()
    {

        buttonClaim.onClick.AddListener(() =>
        {
            if (readyToClaim)
            {
                readyToClaim = false;
                buttonClaim.interactable = false;
                rewardsManager.ClaimPrize();
                UpdateUI();
            }
        });

        buttonCloseWindow.onClick.AddListener(() =>
        {
            panelReward.SetActive(false);
            panelRewardList.SetActive(false);
        });


        //UpdateUI();
    }


    void OnEnable()
    {
        rewardsManager.onClaimPrize += OnClaimPrize;
        rewardsManager.onInitialize += OnInitialize;
        rewardsManager.onReadyClaimPrize += onReadyClaimPrize;
        panelRewardList.SetActive(false);
        InitializeDailyRewardsUI();
    }

    void OnDisable()
    {
        if (rewardsManager != null)
        {
            rewardsManager.onClaimPrize -= OnClaimPrize;
            rewardsManager.onInitialize -= OnInitialize;
            rewardsManager.onReadyClaimPrize -= onReadyClaimPrize;
        }
    }

    // Initializes the UI List based on the rewards size
    private void InitializeDailyRewardsUI()
    {
        buttonClaim.interactable = false;

        for (int i = 0; i < rewardsManager.RewardsList.Count; i++)
        {
            int day = i + 1;
            var reward = rewardsManager.GetReward(i);

            GameObject dailyRewardGo = GameObject.Instantiate(dailyRewardPrefab) as GameObject;

            DailyRewardUI dailyRewardUI = dailyRewardGo.GetComponent<DailyRewardUI>();
            dailyRewardUI.transform.SetParent(dailyRewardsParentGo);
            dailyRewardGo.transform.localScale = Vector2.one;

            dailyRewardUI.Initialize(day, reward);
        }
    }

    private void OnInitialize(bool error, string errorMessage)
    {
        Debug.Log("OnInitialize::error::" + error);
        if (error)
        {
            Debug.Log("OnInitialize::errorMessage::" + errorMessage);
            textRewardError.text = errorMessage;
            ToggleErrorPanel(true);
        }
        else
        {
            ToggleErrorPanel(false);
            panelRewardList.SetActive(true);
            UpdateUI();
        }
    }

    private void OnClaimPrize(Reward reward)
    {
        buttonClaim.interactable = false;
        panelReward.SetActive(true);

        var name = reward.name;
        var rewardQt = reward.reward;

        imageReward.sprite = reward.sprite;

        if (rewardQt > 0)
        {
            textReward.text = string.Format("You got {0} {1}!", reward.reward, name);
        }
        else
        {
            textReward.text = string.Format("You got {0}!", name);
        }
    }

    private void onReadyClaimPrize()
    {
        Debug.Log("onReadyClaimPrize");

        buttonClaim.interactable = true;
        readyToClaim = true;
    }

    private void UpdateUI()
    {
        foreach (DailyRewardUI dailyRewardUIItem in dailyRewardsParentGo.GetComponentsInChildren<DailyRewardUI>())
        {
            dailyRewardUIItem.Refresh();
        }

        CheckTimeDifference();
    }

    private void CheckTimeDifference()
    {
        if (readyToClaim)
        {
            textTimeDue.text = "You can claim your reward!";
        }
        else
        {
            string formattedTs = rewardsManager.GetFormattedTime(rewardsManager.GetTimeDifference());

            textTimeDue.text = string.Format("Come back in {0} for your next reward", formattedTs);
        }
    }


    void ToggleErrorPanel(bool setActive)
    {
        if (panelError != null)
        {
            panelError.SetActive(setActive);
        }
    }

    private void Update()
    {
        CheckTimeDifference();

        /*if (Input.GetKey(KeyCode.Escape))
        {
            PlayerPrefs.DeleteAll();

            Application.LoadLevel(Application.loadedLevelName);
        }*/
    }

    private DateTime GetLastRewardTime()
    {
        if (PlayerPrefs.HasKey("LastRewardTime"))
        {
            return DateTime.ParseExact(PlayerPrefs.GetString("LastRewardTime"), "O", CultureInfo.InvariantCulture);
        }
        else
            return DateTime.Now;
    }

    public void AddDebugDay()
    {
        string lastClaimedStr = DateTime.Now.AddDays(-1).ToString("O");
        PlayerPrefs.SetString("LastRewardTime", lastClaimedStr);
    }
}
