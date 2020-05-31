using OneDevApp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHelperCloudData : MonoBehaviour
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
        CurrentScore = 0;

        //setting default value, if the game is played for the first time
        if (!PlayerPrefs.HasKey("HighScore"))
            PlayerPrefs.SetString("HighScore", "0");

        //tells us if it's the first time that this game has been launched after install - 0 = no, 1 = yes 
        if (!PlayerPrefs.HasKey("IsFirstTime"))
            PlayerPrefs.SetInt("IsFirstTime", 1);

        LoadLocal(); //we want to load local data first because loading from cloud can take quite a while

        PlayCloudDataManager.Instance.Login(OnLoginCallback);
    }

    public void LoginBtn()
    {
        PlayCloudDataManager.Instance.Login(OnLoginCallback);
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

    private void SaveData()
    {
        //if we're still running on local data (cloud data has not been loaded yet), we also want to save only locally
        if (!isCloudDataLoaded)
        {
            SaveLocal();
        }
        else
        {
            PlayCloudDataManager.Instance.SaveToCloud(Highscore.ToString());
        }
    }

    private void LoadLocal()
    {
        Highscore = int.Parse(PlayerPrefs.GetString("HighScore"));
    }

    private void SaveLocal()
    {
        PlayerPrefs.SetString("HighScore", Highscore.ToString());
    }

    private void OnLoginCallback(bool isLoginSuccess)
    {
        isCloudDataLoaded = isLoginSuccess;

        if (isLoginSuccess)
        {
            PlayCloudDataManager.Instance.LoadFromCloud((cloudData) => {

                //if it's the first time that game has been launched after installing it and successfuly logging into Google Play Games
                if (PlayerPrefs.GetInt("IsFirstTime") == 1)
                {
                    //set playerpref to be 0 (false)
                    PlayerPrefs.SetInt("IsFirstTime", 0);
                    if (int.Parse(cloudData) > int.Parse(PlayerPrefs.GetString("HighScore"))) //cloud save is more up to date
                    {
                        //set local save to be equal to the cloud save
                        PlayerPrefs.SetString("HighScore", cloudData);
                        LoadLocal();
                    }
                }
                //if it's not the first time, start comparing
                else
                {
                    //comparing integers, if one int has higher score in it than the other, we update it
                    if (int.Parse(PlayerPrefs.GetString("HighScore")) > int.Parse(cloudData))
                    {
                        //update the cloud save, first set Highscore to be equal to localSave
                        LoadLocal();
                        SaveData();
                        return;
                    }
                }
            });
        }
    }

}
