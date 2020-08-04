using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardMenu : MonoBehaviour {
    public Canvas rewardCanvas;
    public TextMeshProUGUI moneyLabel;
    public TextMeshProUGUI loadingLabel;
    public Button videoButton;
    public Button intersticalButton;

    public const int REWARDED_VIDEO_MONEY_GET = 15;
    public const int INTERSTICIAL_MONEY_GET = 10;
    public static readonly TimeSpan ADS_CD_TIME = new TimeSpan(12, 0, 0);
    public static readonly TimeSpan EPSILON_TIME = new TimeSpan(0, 0, 3);

    private List<Button> adsButtons = new List<Button>();
    private List<string> defaultTexts = new List<string>();

    private void Start()
    {
        SetVisibility(false);

        adsButtons.Add(videoButton);
        adsButtons.Add(intersticalButton);

        //Запоминаем тексты на кнопках, чтобы потом их вернуть
        for (int i = 0; i < adsButtons.Count; i++)
            defaultTexts.Add(adsButtons[i].GetComponentInChildren<TextMeshProUGUI>().text);

        ForceUpdateUI();
        InvokeRepeating("UpdateUI", 0, 1.0f);
    }

    void Update()
    {
        if (rewardCanvas.enabled)
            if (Input.GetKeyDown(KeyCode.Escape))
                OnReturnButton();
    }

    private void ForceUpdateUI()
    {
        GetComponent<ShopMenu>().SetMoneyLabel(moneyLabel);

        DateTime currentTime = DateTime.Now;
        List<bool> adsReady = GoogleMobileAdsScript.instance.GetAdsReadyList();

        bool loadingLabelEnabled = false;
        for (int i = 0; i < adsButtons.Count; i++) {
            DateTime adsCdTime = GlobalVars.instance.LoadTime(i);

            if ((currentTime - adsCdTime) >= EPSILON_TIME) {
                if (adsReady[i]) { //Проверяем, загрузилась ли реклама
                    EnableButton(adsButtons[i], defaultTexts[i]);
                } else {
                    DisableButton(adsButtons[i], defaultTexts[i]);
                    loadingLabelEnabled = true;
                }
            } else if ((currentTime - adsCdTime) <= -EPSILON_TIME) {
                TimeSpan timeRemaining = adsCdTime.Subtract(currentTime);
                DisableButton(adsButtons[i], string.Format("{0:D2}:{1:D2}:{2:D2}", timeRemaining.Hours, timeRemaining.Minutes, timeRemaining.Seconds));
            }
        }

        loadingLabel.enabled = loadingLabelEnabled;

    }

    public void UpdateUI()
    {
        if (!rewardCanvas.enabled)
            return;

        ForceUpdateUI();
    }

    public void OnClickRewardedVideo()
    {
        AudioManager.instance.Play("Click1");
        ForceUpdateUI();
        if (!videoButton.interactable)
            return;

        GoogleMobileAdsScript.instance.ShowRewardedVideoAd();
    }

    public void OnClickRewardedBanner()
    {
        AudioManager.instance.Play("Click1");
        ForceUpdateUI();
        if (!intersticalButton.interactable)
            return;

        GoogleMobileAdsScript.instance.ShowInterstitialAd();
    }

    public void OnReturnButton()
    {
        SetVisibility(false);
        GetComponent<StartMenu>().SetVisibility(true);
    }

    private void DisableButton(Button button, string text)
    {
        if (button.IsInteractable())
            button.interactable = false;
        button.GetComponentInChildren<TextMeshProUGUI>().text = text;
    }

    private void EnableButton(Button button, string text)
    {
        if (!button.IsInteractable())
            button.interactable = true;
        button.GetComponentInChildren<TextMeshProUGUI>().text = text;
    }

    public void SetVisibility(bool isEnabled)
    {
        rewardCanvas.enabled = isEnabled;
    }
}
