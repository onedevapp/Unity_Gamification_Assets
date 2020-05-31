using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

namespace OneDevApp
{
    public class DailyRewardsManager : MonoInstance<DailyRewardsManager>
    {

        // Delegates
        public delegate void OnInitialize(bool error = false, string errorMessage = ""); // When the timer initializes. Sends an error message in case it happens. Should wait for this delegate if using World Clock API
        public OnInitialize onInitialize;

        public delegate void OnClaimPrize(Reward reward);                 // When the player claims the prize
        public OnClaimPrize onClaimPrize;

        public delegate void OnReadyClaimPrize();                 // When the prize is ready to claim
        public OnReadyClaimPrize onReadyClaimPrize;

        public bool UseOnlineTime;

        [SerializeField]
        private List<Reward> _rewardsList;        // Rewards list 
        public List<Reward> RewardsList { get { return _rewardsList; } private set { _rewardsList = value; } }

        private DateTime lastRewardTime;     // The last time the user clicked in a reward
        private int daysCount = 0;         // The available reward position the player claim

        private bool CanUserGetReward = false;
        private bool isInitialized = false;                 // Update flag

        // Needed Constants
        private const string LAST_REWARD_TIME = "LastRewardTimeDR";
        private const string REWARD_DAYS_COUNT = "RewardDaysCount";
        private const string FMT = "O";

        void Start()
        {
            // Initializes the timer with the current time        
            InitializeDate();
        }

        public string GetFormattedTime(TimeSpan span)
        {
            return string.Format("{0:D2}:{1:D2}:{2:D2}", span.Hours, span.Minutes, span.Seconds);
        }

        // Initializes the current DateTime. If the player is using the World Clock initializes it
        private void InitializeDate()
        {
            isInitialized = false;
            daysCount = 0;
            CanUserGetReward = false;

            if (_rewardsList.Count == 0)
            {
                if (onInitialize != null)
                    onInitialize(true, "No Rewards to show");

                return;
            }

            DateTime now = DateTime.Now;

            if (UseOnlineTime)
            {
                now = GetNetTime();

                if (now == DateTime.MinValue)
                {
                    Debug.LogError("Server response parsing error or no Internet access.");

                    if (onInitialize != null)
                        onInitialize(true, "Server response parsing error or no Internet access.");

                    return;
                }
            }


            Load();

            isInitialized = true;

            CheckRewardStatus();

            if (onInitialize != null)
                onInitialize(false, string.Empty);
        }

        public TimeSpan GetTimeDifference()
        {
            TimeSpan difference = (lastRewardTime - DateTime.Now);
            return difference.Add(new TimeSpan(0, 24, 0, 0));
        }

        // Check if the player have unclaimed prizes
        private void CheckRewardStatus()
        {
            // It is not the first time the user claimed.
            // We need to know if he can claim another reward or not
            if (PlayerPrefs.HasKey(LAST_REWARD_TIME))
            {
                TimeSpan diff = DateTime.Now - lastRewardTime;
                Debug.Log(" Last claim was " + (long)diff.TotalHours + " hours ago.");

                int days = (int)(Math.Abs(diff.TotalHours) / 24);
                if (days <= 0)
                {
                    // No claim for you. Try tomorrow
                    CanUserGetReward = false;
                }
                else
                // The player can only claim if he logs between the following next day.
                if (days == 1)
                {
                    CanUserGetReward = true;
                    daysCount++;
                }
                else
                if (days >= 2)
                {
                    // The player loses the following day reward and resets the prize
                    CanUserGetReward = false;
                    daysCount = 0;
                    Debug.Log(" Prize reset ");

                    PlayerPrefs.SetInt("REWARD_DAYS_COUNT", daysCount);
                }
            }
            else
            {
                daysCount = 0;
                // Is this the first time? Shows only the first reward
                CanUserGetReward = true;
            }

            UpdateRewardsItems();
        }

        private void UpdateRewardsItems()
        {
            Debug.Log("daysCount::" + daysCount);

            for (int i = 0; i < _rewardsList.Count; i++)
            {
                Reward prizeReward = GetReward(i);
                if (i < daysCount)
                {
                    prizeReward.state = DailyRewardState.CLAIMED;
                }
                else if (i == daysCount)
                {
                    prizeReward.state = (CanUserGetReward ? DailyRewardState.UNCLAIMED_AVAILABLE : DailyRewardState.CLAIMED);
                }
                else
                {
                    prizeReward.state = DailyRewardState.UNCLAIMED_UNAVAILABLE;
                }
            }

            if (CanUserGetReward && onReadyClaimPrize != null)
                onReadyClaimPrize();
        }

        private void Update()
        {
            if (isInitialized)
            {
                // Updates the time due
                CheckTimeDifference();
            }
        }

        private void CheckTimeDifference()
        {
            if (!CanUserGetReward)
            {
                TimeSpan difference = GetTimeDifference();

                // If the counter below 0 it means there is a new reward to claim
                if (difference.TotalSeconds <= 0)
                {
                    CanUserGetReward = true;

                    if (onReadyClaimPrize != null)
                        onReadyClaimPrize();
                }
            }
        }

        // Checks if the player claim the prize and claims it by calling the delegate. Avoids duplicate call
        public void ClaimPrize()
        {
            if (isInitialized && CanUserGetReward)
            {
                CanUserGetReward = false;

                Reward prizeReward = GetReward(daysCount);
                prizeReward.state = DailyRewardState.CLAIMED;

                // Delegate
                if (onClaimPrize != null)
                    onClaimPrize(prizeReward);

                Debug.Log(" Reward [" + prizeReward.name + "] Claimed!");

                if (daysCount == _rewardsList.Count)
                {
                    daysCount = 0;
                }

                lastRewardTime = DateTime.Now;

                Save();
            }
            else
            {
                Debug.LogError("Error! The player is trying to claim the same reward twice.");
            }

            CheckRewardStatus();
        }

        // Returns the daily Reward of the day
        public Reward GetReward(int day)
        {
            return _rewardsList[day];
        }

        private DateTime GetLastRewardTime()
        {
            if (PlayerPrefs.HasKey(LAST_REWARD_TIME))
            {
                return DateTime.ParseExact(PlayerPrefs.GetString(LAST_REWARD_TIME), FMT, CultureInfo.InvariantCulture);
            }
            else
                return DateTime.Now;
        }

        private void Save()
        {
            PlayerPrefs.SetInt("REWARD_DAYS_COUNT", daysCount);
            PlayerPrefs.SetString(LAST_REWARD_TIME, lastRewardTime.ToString(FMT));
        }

        private void Load()
        {
            daysCount = PlayerPrefs.GetInt("REWARD_DAYS_COUNT", 0);
            lastRewardTime = GetLastRewardTime();
        }

        private DateTime GetNetTime()
        {
            var myHttpWebRequest = (HttpWebRequest)WebRequest.Create("http://www.microsoft.com");
            var response = myHttpWebRequest.GetResponse();
            string todaysDates = response.Headers["date"];
            return DateTime.ParseExact(todaysDates,
                                       "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                                       CultureInfo.InvariantCulture.DateTimeFormat,
                                       DateTimeStyles.AssumeUniversal);
        }
    }

}