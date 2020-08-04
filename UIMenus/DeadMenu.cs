using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeadMenu : MonoBehaviour {
    public Canvas deadCanvas;

    [Header("Score")]
    public TextMeshProUGUI playerScoreText;
    public TextMeshProUGUI playerBestScoreText;

    [Header("Level Progress")]
    public Image statusResultsPanel;
    public Image progressBarFront;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI[] statusTexts = new TextMeshProUGUI[3];

    [Header("Money Progress")]
    public Image moneyResultsPanel;
    public TextMeshProUGUI[] moneyTexts = new TextMeshProUGUI[3];
    public TextMeshProUGUI moneyText;

    [Header("Level Up Animation")]
    public float millisecondsPerProgressBarFill = 500f;
    public float millisecondsPerTickLevelingUp = 30f;

    public float startStatusTextLeft = 1000f;
    public float millisecondsPerTextAppear = 500f;

    private const string SYMBOL_MONEY = "<sprite index=0>";
    private const string SYMBOL_PLUS = "<sprite index=1>";

    private string MONEY_LABEL_PREFIX = SYMBOL_MONEY + " ";

    private IEnumerator animationLevelingUp;
    private List<int> listNewStatuses = new List<int>();
    private List<int> listNewMoney = new List<int>();
    private PlayerStatus playerStatus;
    private int oldMoney;

    void Start()
    {
        playerStatus = GetComponent<PlayerStatus>();
        SetVisibility(false);

        HideTexts(statusTexts);
        HideTexts(moneyTexts);
    }

    void Update()
    {
        if (deadCanvas.enabled)
            if (Input.GetKeyDown(KeyCode.Escape))
                RestartLevel();
    }

    /// <summary>
    /// Открываем меню после смерти и проигрывает анимацию получения уровня
    /// </summary>
    /// <param name="oldPoints">С каких поинтов начинается анимация</param>
    public void OpenDeadMenu(long oldPoints)
    {
        SetVisibility(true);
        GetComponent<PlayableMenu>().SetVisibility(false);

        SetCurrentScore();
        SetBestScore();
        SetStatusValues();
        SetMoneyValues();
        SetProgressBarValue(oldPoints);

        //Заполняем лист с кодами тех статусов, которые игрок получил за прохождение
        FillStatusList();
        //Заполняем лист с деньгами, которые игрок получил за прохождение
        FillMoneyList();
        //Подкорректируем задний фон
        CalibratePanel(statusResultsPanel, listNewStatuses.Count, statusTexts.Length);
        CalibratePanel(moneyResultsPanel, listNewMoney.Count, moneyTexts.Length);

        //Запускаем коротин
        animationLevelingUp = StartAnimationLevelingUp(oldPoints);
        StartCoroutine(animationLevelingUp);
    }

    #region Animation (General)
    public void StopAnimationLevelingUp()
    {
        if (animationLevelingUp == null)
            return;
        StopCoroutine(animationLevelingUp);
        OnAnimationEnd();
    }

    /// <summary>
    /// Что происходит, когда заканчиваются абсолютно все анимации получения уровня
    /// </summary>
    private void OnAnimationEnd()
    {
        AudioManager.instance.Stop("ExpGain1");
        SetProgressBarValue();
        ShowTexts(statusTexts);
        ShowTexts(moneyTexts);
        moneyText.text = GetMoneyString(GlobalVars.instance.GetMoney());
    }

    private IEnumerator StartAnimationLevelingUp(long oldPoints)
    {
        for (int i = 1; i <= 3; i++) {
            if (listNewStatuses.Contains(statusTexts.Length - i + 1)) {
                yield return StartCoroutine(AnimationTextXPos(statusTexts[i - 1]));

                AudioManager.instance.Play("ExpGain1");
                long newPoints = oldPoints + PointsUtility.GetStatusPointsSum(statusTexts.Length - i + 1, playerStatus.GetResultVariable(statusTexts.Length - i + 1));
                yield return StartCoroutine(AnimationProgressBar(oldPoints, newPoints));
                oldPoints = newPoints;
                AudioManager.instance.Stop("ExpGain1");
            }
        }

        for (int i = 0; i < 3; i++) {
            if (listNewMoney.Contains(i)) {
                int newMoney = oldMoney + playerStatus.GetMoneyArray()[i];
                yield return StartCoroutine(AnimationTextXPos(moneyTexts[i]));
                yield return StartCoroutine(AnimationTextIncreaseValue(moneyText, MONEY_LABEL_PREFIX, oldMoney, newMoney));
                oldMoney = newMoney;
            }
        }

        OnAnimationEnd();
    }

    #endregion Animation (General)

    #region Animation (Texts)
    /// <summary>
    /// Анимация появления текста слева (используется xPos)
    /// </summary>
    private IEnumerator AnimationTextXPos(TextMeshProUGUI textPro)
    {
        float positionToAdd = startStatusTextLeft / (millisecondsPerTextAppear / millisecondsPerTickLevelingUp);
        RectTransform textTransform = textPro.GetComponent<RectTransform>();
        while (textTransform.anchoredPosition.x < 0) {
            textTransform.anchoredPosition = new Vector2(textTransform.anchoredPosition.x + positionToAdd, textTransform.anchoredPosition.y);
            yield return new WaitForSecondsRealtime(millisecondsPerTickLevelingUp / 1000f);
        }
    }

    /// <summary>
    /// Анимация изменения текста с числом (прибавляется 1, пока не получится число)
    /// </summary>
    /// <returns></returns>
    private IEnumerator AnimationTextIncreaseValue(TextMeshProUGUI textPro, string prefix, int fromValue, int toValue)
    {
        while (fromValue <= toValue) {
            textPro.text = GetMoneyString(fromValue);
            fromValue++;
            yield return new WaitForSecondsRealtime(millisecondsPerTickLevelingUp / 1000f);
        }
    }

    /// <summary>
    /// Выставляем тексты статусов сверху вниз и заполняем список, полученных статусов
    /// </summary>
    private void FillStatusList()
    {
        List<TextMeshProUGUI> textsInOrder = new List<TextMeshProUGUI>(statusTexts);

        for (int i = 0; i < 3; i++) {
            if (playerStatus.GetResultVariable(statusTexts.Length - i) <= 0) {
                for (int j = textsInOrder.IndexOf(statusTexts[i]); j < 2; j++) {
                    RectTransformUtility.SwapRectTransProperties(textsInOrder[j].GetComponent<RectTransform>(), textsInOrder[j + 1].GetComponent<RectTransform>());

                    var temp = textsInOrder[j];
                    textsInOrder[j] = textsInOrder[j + 1];
                    textsInOrder[j + 1] = temp;
                }
                statusTexts[i].enabled = false;
            } else {
                listNewStatuses.Add(statusTexts.Length - i);
            }
        }
    }

    /// <summary>
    /// Выставляем тексты money сверху вниз и заполняем список, полученных money
    /// </summary>
    private void FillMoneyList()
    {
        List<TextMeshProUGUI> textsInOrder = new List<TextMeshProUGUI>(moneyTexts);

        for (int i = 0; i < 3; i++) {
            if (playerStatus.GetMoneyArray()[i] <= 0) {
                for (int j = textsInOrder.IndexOf(moneyTexts[i]); j < 2; j++) {
                    RectTransformUtility.SwapRectTransProperties(textsInOrder[j].GetComponent<RectTransform>(), textsInOrder[j + 1].GetComponent<RectTransform>());

                    var temp = textsInOrder[j];
                    textsInOrder[j] = textsInOrder[j + 1];
                    textsInOrder[j + 1] = temp;
                }
                moneyTexts[i].enabled = false;
            } else {
                listNewMoney.Add(i);
            }
        }
    }

    #endregion

    #region Animation (Progress Bar)
    /// <summary>
    /// Анимация заполнения прогресс бара
    /// </summary>
    private IEnumerator AnimationProgressBar(long oldPoints, long newPoints)
    {
        float fOldPoints = oldPoints;
        float fPoints = newPoints;

        float fPointsDiffer = fPoints - fOldPoints;
        float fPointsToAdd = fPointsDiffer / (millisecondsPerProgressBarFill / millisecondsPerTickLevelingUp);

        while (fOldPoints < fPoints) {
            SetProgressBarValue(fOldPoints);
            fOldPoints += fPointsToAdd;
            if (PointsUtility.GetLevel(fOldPoints) != PointsUtility.GetLevel(fPoints))
                AudioManager.instance.Play("LevelUp1");

            yield return new WaitForSecondsRealtime(millisecondsPerTickLevelingUp / 1000f);
        }
    }

    #endregion Animation (Progress Bar)

    #region Progress Bar
    /// <summary>
    /// Устанавливает progressBar с соответсвием прогресса уровня. Показывает уровень игрока. Данные из параметров
    /// </summary>
    private void SetProgressBarValue(float percent, int level)
    {
        progressBarFront.fillAmount = percent;
        levelText.text = level.ToString();
    }

    /// <summary>
    /// Устанавливает progressBar с соответсвием прогресса уровня. Показывает уровень игрока. Локальные данные
    /// </summary>
    private void SetProgressBarValue()
    {
        progressBarFront.fillAmount = PointsUtility.GetLevelPercent();
        levelText.text = PointsUtility.GetLevel().ToString();
    }

    /// <summary>
    /// Устанавливает progressBar с соответсвием прогресса уровня. Показывает уровень игрока. Данные из очков (float)
    /// </summary>
    private void SetProgressBarValue(float fPoints)
    {
        SetProgressBarValue(PointsUtility.GetLevelPercent(fPoints), PointsUtility.GetLevel(fPoints));
    }

    /// <summary>
    /// Устанавливает progressBar с соответсвием прогресса уровня. Показывает уровень игрока. Данные из очков (long)
    /// </summary>
    private void SetProgressBarValue(long points)
    {
        SetProgressBarValue(PointsUtility.GetLevelPercent(points), PointsUtility.GetLevel(points));
    }
    #endregion Progress Bar

    #region Status and Money Texts
    private void HideTexts(TextMeshProUGUI[] texts)
    {
        foreach (TextMeshProUGUI text in texts)
            text.GetComponent<RectTransform>().SetXPos(-startStatusTextLeft);
    }

    private void ShowTexts(TextMeshProUGUI[] texts)
    {
        foreach (TextMeshProUGUI text in texts)
            text.GetComponent<RectTransform>().SetXPos(0);
    }

    private void SetStatusValue(int code)
    {
        //Почему я здесь не использовал format? :/
        statusTexts[statusTexts.Length - code].text = SYMBOL_PLUS + " " + playerStatus.GetResultVariable(code).ToString() +
            " \"" + LocalizationManager.instance.GetLocalizedValue("perfectionStatus" + code) + "\"!";
    }

    private void SetStatusValues()
    {
        SetStatusValue(1);
        SetStatusValue(2);
        SetStatusValue(3);
    }

    private void SetMoneyValue(int code, int moneyGot)
    {
        moneyTexts[code - 1].text = LocalizationManager.instance.GetLocalizedValue("moneyLabel" + code) + ": " + SYMBOL_PLUS + moneyGot + SYMBOL_MONEY;
    }

    private void SetMoneyValues()
    {
        int oldMoney = GlobalVars.instance.GetMoney();
        for (int i = 0; i < moneyTexts.Length; i++) {
            if (playerStatus.GetMoneyArray()[i] > 0) {
                SetMoneyValue(i + 1, playerStatus.GetMoneyArray()[i]);
                oldMoney -= playerStatus.GetMoneyArray()[i];
            }
        }

        moneyText.text = GetMoneyString(oldMoney);

        this.oldMoney = oldMoney;
    }

    #endregion

    #region Score
    private void SetCurrentScore()
    {
        playerScoreText.text = playerStatus.GetCurrentScore().ToString();
    }

    private void SetBestScore()
    {
        playerBestScoreText.text = GlobalVars.instance.GetHighScore() + "";
    }

    #endregion

    private string GetMoneyString(int count)
    {
        return MONEY_LABEL_PREFIX + count;
    }

    /// <summary>
    /// Изменяем высоту панели в зависимости от строк в ней
    /// </summary>
    private void CalibratePanel(Image panel, int parametersGot, int parametersMax)
    {
        RectTransform rt = panel.GetComponent<RectTransform>();
        rt.SetHeight(rt.GetHeight() * ((float) parametersGot / parametersMax));
    }

    public void RestartLevel()
    {
        AudioManager.instance.Play("Click1");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OpenStartMenu()
    {
        GlobalVars.instance.SetHasGameStarted(false);

        RestartLevel();
    }

    private void SetVisibility(bool isVisible)
    {
        deadCanvas.GetComponent<Canvas>().enabled = isVisible;
    }

    public bool GetVisibility() {
        return deadCanvas.GetComponent<Canvas>().enabled;
    }
}
