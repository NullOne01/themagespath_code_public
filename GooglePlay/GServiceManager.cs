using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using UnityEngine.SocialPlatforms;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using System;

public class GServiceManager : MonoBehaviour
{
    public static GServiceManager instance;
    private static GServiceSaver myGServiceSaver;

    public bool Authenticated {
        get {
            return Social.Active.localUser.authenticated;
        }
    }

    void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this);
    }

    void Start() {
        if (Application.platform == RuntimePlatform.Android) {
            myGServiceSaver = new GServiceSaver();
            ConfigureServices();
            //Входим в сервисы и скачиваем данные
            SignIn(myGServiceSaver);
        }
    }

    public void SaveLocalData() {
        if (Application.platform == RuntimePlatform.Android)
            myGServiceSaver.SaveData();
    }

    private void ConfigureServices() {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
        .EnableSavedGames()
        .Build();

        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
    }

    private void SignIn(GServiceSaver myGServiceSaver) {
        Social.localUser.Authenticate((bool success) =>
        {
            if (success) {
                Debug.Log("Successfully signed in google services!");
                //((GooglePlayGames.PlayGamesPlatform)Social.Active).SetGravityForPopups(Gravity.TOP);
                //Читаем данные
                myGServiceSaver.LoadData();
            } else {
                Debug.LogError("Failed on signing in");
            }
        });
    }

    public void AddScoreToLeaderboard(int score) {
        if (Application.platform == RuntimePlatform.Android) {
            Social.localUser.Authenticate((bool success1) =>
            {
                if (success1) {
                    Social.ReportScore(score, GPGSIds.leaderboard_best_scores, (bool success) =>
                    {
                        if (success)
                            Debug.Log("High score has been submitted to the leaderboard!");
                        else
                             Debug.LogError("Failed submitting score to the leaderboard");
                    });
                }
            });
        }
    }

    public void AddLevelToLeaderboard(int level) {
        if (Application.platform == RuntimePlatform.Android) {
            Social.localUser.Authenticate((bool success1) =>
            {
                if (success1) {
                    Social.ReportScore(level, GPGSIds.leaderboard_levels, (bool success) =>
                    {
                        if (success)
                            Debug.Log("Level has been submitted to the leaderboard!");
                        else
                            Debug.LogError("Failed submitting level to the leaderboard");
                    });
                }
            });
        }
    }

    public void ShowScoreLeaderboard() {
        PlayGamesPlatform.Instance.ShowLeaderboardUI(GPGSIds.leaderboard_best_scores);
    }

    public void ShowAllLeaderboard() {
        Debug.Log("Tried to show all leadeboard");
        PlayGamesPlatform.Instance.ShowLeaderboardUI();
    }
}
