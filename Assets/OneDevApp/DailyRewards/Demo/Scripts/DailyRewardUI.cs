using OneDevApp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewardUI : MonoBehaviour
{
    public bool showRewardName;

    [Header("UI Elements")]
    public Text textDay;                // Text containing the Day text eg. Day 12
    public Text textReward;             // The Text containing the Reward amount
    public Image imageRewardBackground; // The Reward Image Background
    public Image imageReward;           // The Reward Image
    public Color colorClaim;            // The Color of the background when claimed
    public Color colorAvailable;            // The Color of the background when claimed
    private Color colorUnclaimed;       // The Color of the background when not claimed

    private int day;
    [SerializeField]
    private Reward reward;

    void Awake()
    {
        colorUnclaimed = imageReward.color;
    }

    public void Initialize(int day, Reward reward)
    {
        this.day = day;
        this.reward = reward;

        textDay.text = string.Format("Day {0}", day.ToString());
        if (reward.reward > 0)
        {
            if (showRewardName)
            {
                textReward.text = reward.reward + " " + reward.name;
            }
            else
            {
                textReward.text = reward.reward.ToString();
            }
        }
        else
        {
            textReward.text = reward.name.ToString();
        }
        imageReward.sprite = reward.sprite;

        Refresh();
    }

    // Refreshes the UI
    public void Refresh()
    {
        switch (reward.state)
        {
            case DailyRewardState.UNCLAIMED_AVAILABLE:
                imageRewardBackground.color = colorAvailable;
                break;
            case DailyRewardState.UNCLAIMED_UNAVAILABLE:
                imageRewardBackground.color = colorUnclaimed;
                break;
            case DailyRewardState.CLAIMED:
                imageRewardBackground.color = colorClaim;
                break;
        }
    }
}
