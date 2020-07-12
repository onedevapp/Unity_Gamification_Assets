using OneDevApp;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HourlyRewardsUIDemo : MonoBehaviour
{
    [Header("Panel Reward")]
    public Button buttonClaim;                  // Claim Button
    public Text textTimeDue;                    // Text showing how long until the next claim
    public Image imageReward;                   // The image of the reward
    public Sprite spriteRewardLocked;           // The sprite image of the locked reward
    public Sprite spriteRewardUnLocked;         // The sprite image of the unlocked reward

    [Header("Panel Reward Error")]
    public GameObject panelError;               // Rewards error panel
    public Text textRewardError;                // Text showing how long until the next claim

    private bool readyToClaim;                  // Update flag
    private bool isRewardToBeClaimed;                  // Update flag

    HourlyRewardsManager rewardsManager;

    public DailyRewardState state = DailyRewardState.UNCLAIMED_UNAVAILABLE;
    
    // Start is called before the first frame update
    void Start()
    {
        rewardsManager = HourlyRewardsManager.Instance;

        isRewardToBeClaimed = false;

        rewardsManager.onClaimPrize += OnClaimPrize;
        rewardsManager.onReadyClaimPrize += onReadyClaimPrize;
        rewardsManager.onInitialize += OnInitialize;

        buttonClaim.interactable = false;

        buttonClaim.onClick.AddListener(() =>
        {
            if (readyToClaim)
            {
                readyToClaim = false;
                buttonClaim.interactable = false;
                rewardsManager.ClaimPrize();
            }
        });

        rewardsManager.Initialize();
    }

    private void UpdateUI(DailyRewardState state)
    {
        this.state = state;
        switch (state)
        {
            case DailyRewardState.UNCLAIMED_AVAILABLE:
                imageReward.sprite = spriteRewardLocked;
                break;
            case DailyRewardState.UNCLAIMED_UNAVAILABLE:
                imageReward.sprite = spriteRewardLocked;
                break;
            case DailyRewardState.CLAIMED:
                imageReward.sprite = spriteRewardUnLocked;
                break;
        }
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

    private void OnInitialize(DailyRewardState state, bool error, string errorMessage)
    {
        this.state = state;
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
            UpdateUI(state);
        }
    }

    private void onReadyClaimPrize()
    {
        Debug.Log("onReadyClaimPrize");

        buttonClaim.interactable = true;
        readyToClaim = true;
    }

    private void OnClaimPrize(DailyRewardState state)
    {
        this.state = state;

        isRewardToBeClaimed = true;

        buttonClaim.interactable = false;

        textTimeDue.text = string.Format("You got 1000 coins!");

        UpdateUI(state);

        Invoke("RevertRewardToBeClaimed", 5f);  //Or one can use two text to show
    }

    void RevertRewardToBeClaimed()
    {
        isRewardToBeClaimed = false;
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


    private void CheckTimeDifference()
    {
        if (readyToClaim)
        {
            textTimeDue.text = "You can claim your reward!";
        }
        else if(!isRewardToBeClaimed)
        {
            string formattedTs = rewardsManager.GetFormattedTime(rewardsManager.GetTimeDifference());

            textTimeDue.text = string.Format("Come back in {0} for your next reward", formattedTs);
        }
    }

    public void AddDebugHour()
    {
        string lastClaimedStr = DateTime.Now.AddHours(-1).AddSeconds(20).ToString("O");
        PlayerPrefs.SetString("LastRewardTimeHR", lastClaimedStr);
        Application.LoadLevel(Application.loadedLevelName);
    }
}
