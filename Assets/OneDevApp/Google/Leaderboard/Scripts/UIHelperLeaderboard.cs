using OneDevApp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHelperLeaderboard : MonoBehaviour
{
    public Text HighScoreText;
    public Text CurrentScoreText;

    private int _currentScore;
    public int CurrentScore
    {
        get
        {
            return _currentScore;
        }
        set
        {
            _currentScore = value;
            CurrentScoreText.text = "Score : " + _currentScore.ToString();
        }
    }

    private int _highScore;
    public int Highscore
    {
        get
        {
            return _highScore;
        }
        set
        {
            _highScore = value;
            HighScoreText.text = "HighScore : " + _highScore.ToString();
        }
    }

    bool isCloudDataLoaded = false;

    // Use this for initialization
    void Start()
    {
        LeaderboardManager.Instance.Login(OnLoginCallback);
    }

    public void LoginBtn()
    {
        LeaderboardManager.Instance.Login(OnLoginCallback);
    }

    public void RestartGameBtn()
    {
        CurrentScore = UnityEngine.Random.Range(10, 200);

        if (CurrentScore > Highscore)
        {
            Highscore = CurrentScore;
            SaveData();
        }
    }

    public void ShowLeaderboardsUI()
    {
        LeaderboardManager.Instance.ShowLeaderboardsUI();
        //LeaderboardManager.Instance.ShowLeaderboardsUI(GPGSIds.leaderboard_leaderboard);
    }

    private void SaveData()
    {
        //if we're still running on local data (cloud data has not been loaded yet), we also want to save only locally
        if (isCloudDataLoaded)
        {           
            LeaderboardManager.Instance.AddScoreToLeaderboard(GPGSIds.leaderboard_leaderboard, CurrentScore);
        }
    }

    private void OnLoginCallback(bool isLoginSuccess)
    {
        isCloudDataLoaded = isLoginSuccess;
    }

}
