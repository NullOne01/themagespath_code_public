using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVars : MonoBehaviour {
    public static GlobalVars instance;
    private bool hasAlreadyStarted = false;

    public const int MAX_PLAYERS = 6;
    public const int MAX_BACKGROUNDS = 4;
    public const int MAX_TIME_SAVES = 2;

    private const string PREFS_FIRST_TIME = "isFirstTime";
    private const string PREFS_HIGH_SCORE = "highScore";
    private const string PREFS_MONEY = "money";
    private const string PREFS_POINTS = "points";
    private const string PREFS_PREFIX_PLAYERS = "playerModel";
    private const string PREFS_CURRENT_PLAYER = "currentPlayerModel";
    private const string PREFS_PREFIX_BACKGROUNDS = "background";
    private const string PREFS_CURRENT_BACKGROUND = "currentBackground";
    private const string PREFS_PREFIX_TIME = "time";

    void Awake()
    {
        Application.targetFrameRate = 60;

        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this);
    }

    #region Cloud
    public void SetUpServiceData(GServiceData serviceData)
    {
        SetPoints(serviceData.points);
        SetHighScore(serviceData.highScore);
        SetMoney(serviceData.money);
        CopyListPlayers(serviceData.openedPlayerModels);
        CopyListBackgrounds(serviceData.openedBackgrounds);
    }
    #endregion

    #region Has game started?
    public void SetHasGameStarted(bool hasAlreadyStarted)
    {
        this.hasAlreadyStarted = hasAlreadyStarted;
    }

    /// <summary>
    /// Игра начинается сразу без подготовки (т.е. без лишних меню)?
    /// </summary>
    public bool HasGameStarted()
    {
        return hasAlreadyStarted;
    }
    #endregion

    #region First time
    public void RemoveIsFirstTime()
    {
        PlayerPrefs.SetInt(PREFS_FIRST_TIME, 0);
    }

    public bool GetIsFirstTime()
    {
        return PlayerPrefs.GetInt(PREFS_FIRST_TIME, 1) == 1 ? true : false;
    }
    #endregion

    #region High score
    public bool TrySetNewHighScore(int newHighScore)
    {
        int currHighScore = GetHighScore();
        //Если новый счёт - это рекорд, то следовательно сохраняем рекорд
        if (newHighScore > currHighScore) {
            SaveHighScore(newHighScore);
            return true;
        } else {
            return false;
        }
    }

    private void SaveHighScore(int newHighScore)
    {
        GServiceManager.instance.AddScoreToLeaderboard(newHighScore);
        GServiceManager.instance.SaveLocalData();
        PlayerPrefs.SetInt(PREFS_HIGH_SCORE, newHighScore);
    }

    public void SetHighScore(int value)
    {
        PlayerPrefs.SetInt(PREFS_HIGH_SCORE, value);
    }

    public int GetHighScore()
    {
        return PlayerPrefs.GetInt(PREFS_HIGH_SCORE, 0);
    }

    #endregion

    #region Points and Levels
    public void AddPoints(long pointsToAdd)
    {
        PlayerPrefs.SetString(PREFS_POINTS, (GetPoints() + pointsToAdd).ToString());
        GServiceManager.instance.SaveLocalData();
    }

    public long GetPoints()
    {
        return long.Parse(PlayerPrefs.GetString(PREFS_POINTS, "0"));
    }

    public void SetPoints(long points)
    {
        PlayerPrefs.SetString(PREFS_POINTS, points.ToString());
    }
    #endregion

    #region Players and Backgrounds

    /// <summary>
    /// Создаёт List(bool) со значениями, взятаых из PlayerPrefs
    /// Здесь сразу обнуляем нулевое значение, ибо дефолтные скины у нас куплены сразу
    /// </summary>
    /// <param name="prefixStr">Префикс данных в PlayerPrefs<param>
    /// <param name="num">Размер листа</param>
    /// <returns>List(bool) со значениями, взятаых из PlayerPrefs</returns>
    private List<bool> CreateListFromPrefs(string prefixStr, int num)
    {
        List<bool> resultArray = new List<bool>();
        resultArray.Add(true); //Default skin
        for (int i = 1; i < num; i++) {
            resultArray.Add(PlayerPrefs.GetInt(prefixStr + i, 0) == 0 ? false : true); //Если 0 - false, 1 - true
        }

        return resultArray;
    }

    /// <summary>
    /// Копирует значения из listToCopy и ставляет в PlayerPrefs
    /// </summary>
    /// <param name="prefixStr">Префикс данных в PlayerPrefs</param>
    /// <param name="listToCopy">Откуда копируем значения</param>
    private void CopyPrefsListFromList(string prefixStr, List<bool> listToCopy)
    {
        PlayerPrefs.SetInt(prefixStr + '0', 1); //Default skin
        for (int i = 1; i < listToCopy.Count; i++)
            PlayerPrefs.SetInt(prefixStr + i, listToCopy[i] == true ? 1 : 0);
    }

    public List<bool> GetListPlayers()
    {
        return CreateListFromPrefs(PREFS_PREFIX_PLAYERS, MAX_PLAYERS);
    }

    public List<bool> GetListBackgrounds()
    {
        return CreateListFromPrefs(PREFS_PREFIX_BACKGROUNDS, MAX_BACKGROUNDS);
    }

    public void CopyListPlayers(List<bool> newPlayers)
    {
        CopyPrefsListFromList(PREFS_PREFIX_PLAYERS, newPlayers);
    }

    public void CopyListBackgrounds(List<bool> newBackgrounds)
    {
        CopyPrefsListFromList(PREFS_PREFIX_BACKGROUNDS, newBackgrounds);
    }

    public void SetPlayerEnabled(int playerID)
    {
        PlayerPrefs.SetInt(PREFS_PREFIX_PLAYERS + playerID, 1);
        GServiceManager.instance.SaveLocalData();
    }

    public int GetCurrentPlayer()
    {
        return PlayerPrefs.GetInt(PREFS_CURRENT_PLAYER, 0);
    }

    public void SetCurrentPlayer(int currentPlayerID)
    {
        PlayerPrefs.SetInt(PREFS_CURRENT_PLAYER, currentPlayerID);
    }

    public void SetBackgroundEnabled(int bgID)
    {
        PlayerPrefs.SetInt(PREFS_PREFIX_BACKGROUNDS + bgID, 1);
        GServiceManager.instance.SaveLocalData();
    }

    public int GetCurrentBackground()
    {
        return PlayerPrefs.GetInt(PREFS_CURRENT_BACKGROUND, 0);
    }

    public void SetCurrentBackground(int currentBackgroundID)
    {
        PlayerPrefs.SetInt(PREFS_CURRENT_BACKGROUND, currentBackgroundID);
    }

    #endregion

    #region Money
    public int GetMoney()
    {
        return PlayerPrefs.GetInt(PREFS_MONEY, 0);
    }

    public void SetMoney(int value)
    {
        PlayerPrefs.SetInt(PREFS_MONEY, value);
    }


    public void AddMoney(int value)
    {
        PlayerPrefs.SetInt(PREFS_MONEY, GetMoney() + value);
        GServiceManager.instance.SaveLocalData();
    }

    public void RemoveMoney(int value)
    {
        PlayerPrefs.SetInt(PREFS_MONEY, GetMoney() - value);
        GServiceManager.instance.SaveLocalData();
    }
    #endregion

    #region Time

    public void SaveTime(int id, DateTime dateTime)
    {
        PlayerPrefs.SetString(PREFS_PREFIX_TIME + id, dateTime.ToBinary().ToString());
    }

    public DateTime LoadTime(int id)
    {
        return DateTime.FromBinary(Convert.ToInt64(PlayerPrefs.GetString(PREFS_PREFIX_TIME + id, DateTime.Now.ToBinary().ToString())));
    }

    public void AddTime(int id, TimeSpan timeToAdd)
    {
        SaveTime(id, LoadTime(id).Add(timeToAdd));
    }

    public bool IsAnyRewardReady()
    {
        DateTime currentTime = DateTime.Now;
        for (int i = 0; i < MAX_TIME_SAVES; i++) {
            if (currentTime >= instance.LoadTime(i))
                return true;
        }
        return false;
    }

    #endregion
}
