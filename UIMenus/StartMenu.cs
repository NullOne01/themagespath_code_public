using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour {
    public Canvas startCanvas;
    public Button rewardButton;
    public Button leaderboardButton;

    [SerializeField]
    private float stepCameraX = 0.1f;
    public float startMenuPosX = -2f;

    private float gamePosX = 0f;

    void Start()
    {
        //Запоминаем, куда вернуть камеру
        gamePosX = transform.position.x;

        if (GlobalVars.instance.HasGameStarted()) {
            SetVisibility(false);
        } else {
            transform.position = new Vector3(startMenuPosX, transform.position.y, transform.position.z);
            SetVisibility(true);
            GetComponent<PlayableMenu>().SetVisibility(false);
        }

        //InvokeRepeating("CheckRewardButton", 0, 2.0f);
        InvokeRepeating("CheckLeaderboardButton", 0, 2.0f);
    }

    void Update()
    {
        if (startCanvas.enabled)
            if (Input.GetKeyDown(KeyCode.Escape))
                Application.Quit();
    }

    private IEnumerator SetCameraPosition(Action callBack)
    {
        LevelCreator levelCreator = GameObject.FindGameObjectWithTag("LevelCreator").GetComponent<LevelCreator>();

        while (Math.Abs(transform.position.x - gamePosX) > 0.05f) {
            transform.position = new Vector3(transform.position.x + (stepCameraX * Math.Sign(gamePosX - transform.position.x)), transform.position.y, transform.position.z);
            levelCreator.SetCurrentColumnGrowningPercent(1f - Math.Abs(transform.position.x - gamePosX));
            yield return null;
        }

        callBack?.Invoke();
    }

    public void SetVisibility(bool isEnabled)
    {
        startCanvas.enabled = isEnabled;
    }

    private void OnCameraArrived()
    {
        GlobalVars.instance.SetHasGameStarted(true);
    }

    public void StartGame()
    {
        AudioManager.instance.Play("Click1");
        //Начинаем перемещать камеру. Метод OnCameraArrived сработает, когда камера станёт на своё место
        StartCoroutine(SetCameraPosition(OnCameraArrived));

        //Прячем меню
        SetVisibility(false);
        //Включаем игровое UI
        GetComponent<PlayableMenu>().SetVisibility(true);
    }

    public void OpenShop()
    {
        AudioManager.instance.Play("Click1");
        //Прячем меню
        SetVisibility(false);
        //Включаем UI магазина
        GetComponent<ShopMenu>().SetVisibility(true);
    }

    public void OpenLeaderboard()
    {
        AudioManager.instance.Play("Click1");
        GServiceManager.instance.ShowAllLeaderboard();
    }

    public void OpenReward()
    {
        AudioManager.instance.Play("Click1");
        //Прячем меню
        SetVisibility(false);
        //Включаем UI магазина
        GetComponent<RewardMenu>().SetVisibility(true);
    }
    
    /// <summary>
    /// Скрывает кнопку, если нет наград, которые можно забрать
    /// </summary>
    private void CheckRewardButton()
    {
        if (startCanvas.enabled)
            rewardButton.gameObject.SetActive(GlobalVars.instance.IsAnyRewardReady());
    }

    private void CheckLeaderboardButton() {
        if (startCanvas.enabled)
            leaderboardButton.GetComponent<Image>().enabled = (GServiceManager.instance.Authenticated);
    }
}
