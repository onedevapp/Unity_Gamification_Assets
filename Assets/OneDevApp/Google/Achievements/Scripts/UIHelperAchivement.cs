using OneDevApp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHelperAchivement : MonoBehaviour
{

    public void LoginBtn()
    {
        LeaderboardManager.Instance.Login((val)=> { });
    }

    public void ShowAchievements()
    {
        AchievementManager.Instance.ShowAchievementsUI();
    }

    public void Increment()
    {
        AchievementManager.Instance.IncrementAchievement(GPGSIds.achievement_incremental_achievement, 5);
    }

    public void Unlock()
    {
        AchievementManager.Instance.UnlockAchievement(GPGSIds.achievement_standard_achievement);
    }
}
