using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using UnityEngine;

namespace OneDevApp
{
    public class HourlyRewardsManager : MonoInstance<HourlyRewardsManager>
    {
        // Delegates
        public delegate void OnInitialize(DailyRewardState state, bool error = false, string errorMessage = ""); // When the timer initializes. Sends an error message in case it happens. Should wait for this delegate if using World Clock API
        public OnInitialize onInitialize;

        public delegate void OnClaimPrize(DailyRewardState state);        // When the player claims the prize
        public OnClaimPrize onClaimPrize;

        public delegate void OnReadyClaimPrize();   // When the prize is ready to claim
        public OnReadyClaimPrize onReadyClaimPrize;

        public bool UseOnlineTime;
        public int RewardsTimeLimit = 1;

        private DateTime lastRewardTime;     // The last time the user clicked in a reward

        private bool CanUserGetReward = false;
        private bool isInitialized = false;                 // Update flag

        // Needed Constants
        private const string LAST_REWARD_TIME = "LastRewardTimeHR";
        private const string FMT = "O";
        
        // Initializes the current DateTime. If the player is using the World Clock initializes it
        public void Initialize()
        {
            isInitialized = false;
            CanUserGetReward = false;

            DateTime now = DateTime.Now;

            if (UseOnlineTime)
            {
                now = GetNetTime();

                if (now == DateTime.MinValue)
                {
                    Debug.LogError("Server response parsing error or no Internet access.");

                    if (onInitialize != null)
                        onInitialize(DailyRewardState.UNCLAIMED_UNAVAILABLE, true, "Server response parsing error or no Internet access.");

                    return;
                }
            }

            Load();

            isInitialized = true;

            DailyRewardState state = CheckRewardStatus();

            if (onInitialize != null)
                onInitialize(state, false, string.Empty);

        }

        // Check if the player have unclaimed prizes
        private DailyRewardState CheckRewardStatus()
        {
            DailyRewardState state = DailyRewardState.UNCLAIMED_UNAVAILABLE;

            // It is not the first time the user claimed.
            // We need to know if he can claim another reward or not
            if (PlayerPrefs.HasKey(LAST_REWARD_TIME))
            {
                TimeSpan diff = DateTime.Now - lastRewardTime;
                Debug.Log(" Last claim was " + (long)diff.TotalHours + " hours ago.");

                int hours = (int)(Math.Abs(diff.TotalHours));
                if (hours <= 0)
                {
                    // No claim for you. Try after an Hour
                    CanUserGetReward = false;
                    state = DailyRewardState.UNCLAIMED_UNAVAILABLE;
                }
                else
                // The player can only claim if he logs between the following next hours.
                if (hours >= RewardsTimeLimit)
                {
                    CanUserGetReward = true;
                    state = DailyRewardState.UNCLAIMED_AVAILABLE;
                }
            }
            else
            {
                // Is this the first time? Shows only the first reward
                CanUserGetReward = true;
                state = DailyRewardState.UNCLAIMED_AVAILABLE;
            }

            if (CanUserGetReward && onReadyClaimPrize != null)
                onReadyClaimPrize();

            return state;
        }

        // Checks if the player claim the prize and claims it by calling the delegate. Avoids duplicate call
        public void ClaimPrize()
        {
            if (isInitialized && CanUserGetReward)
            {
                CanUserGetReward = false;

                // Delegate
                if (onClaimPrize != null)
                    onClaimPrize(DailyRewardState.CLAIMED);

                Debug.Log(" Reward [" + lastRewardTime + "] Claimed!");

                lastRewardTime = DateTime.Now;

                Save();
            }
            else
            {
                Debug.LogError("Error! The player is trying to claim the same reward twice.");
            }

            CheckRewardStatus();
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

        private DateTime GetLastRewardTime()
        {
            if (PlayerPrefs.HasKey(LAST_REWARD_TIME))
            {
                return DateTime.ParseExact(PlayerPrefs.GetString(LAST_REWARD_TIME), FMT, CultureInfo.InvariantCulture);
            }
            else
                return DateTime.Now;
        }

        private void Load()
        {
            lastRewardTime = GetLastRewardTime();
        }

        private void Save()
        {
            PlayerPrefs.SetString(LAST_REWARD_TIME, lastRewardTime.ToString(FMT));
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

        public TimeSpan GetTimeDifference()
        {
            TimeSpan difference = (lastRewardTime - DateTime.Now);
            return difference.Add(new TimeSpan(0, RewardsTimeLimit, 0, 0));
        }

        public string GetFormattedTime(TimeSpan span)
        {
            return string.Format("{0:D2}:{1:D2}:{2:D2}", span.Hours, span.Minutes, span.Seconds);
        }
    }

}