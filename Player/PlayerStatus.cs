using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStatus : MonoBehaviour {
    public GameObject textPrefab;
    public Transform panelWhereTextSpawns;

    [Header("Result Text Properties")]
    [SerializeField]
    private float minWidth = 300f;
    [SerializeField]
    private float maxWidth = 400f;
    [SerializeField]
    private float maxAngleZ = 15f;

    private DeadMenu deadMenu;
    private PlayableMenu playableMenu;

    private int currentScore = 0;
    private int niceCounter = 0;
    private int greatCounter = 0;
    private int perfectCounter = 0;

    private enum StatusResults { Nice = 1, Great, Perfect };

    public const int STATUS_CODE_NICE = (int)StatusResults.Nice;
    public const int STATUS_CODE_GREAT = (int)StatusResults.Great;
    public const int STATUS_CODE_PERFECT = (int)StatusResults.Perfect;

    private int moneyGotFromLevel = 0;
    private int moneyGotFromScore = 0;
    private int moneyGotFromStreak = 0;

    private int perfectStreak = 0;

    void Start()
    {
        deadMenu = GetComponent<DeadMenu>();
        playableMenu = GetComponent<PlayableMenu>();

        playableMenu.ShowScore(currentScore);
    }

    public void KillPlayer()
    {
        GlobalVars.instance.TrySetNewHighScore(currentScore);
        long oldPoints = GlobalVars.instance.GetPoints();

        FixPlayerResults(oldPoints);
        deadMenu.OpenDeadMenu(oldPoints);
    }

    /// <summary>
    /// Записываем всё, что игрок получил за прохождение
    /// </summary>
    private void FixPlayerResults(long oldPoints)
    {
        PointsUtility.AddLevelProgress(niceCounter, greatCounter, perfectCounter);
        long newPoints = GlobalVars.instance.GetPoints();
        GServiceManager.instance.AddLevelToLeaderboard(PointsUtility.GetLevel());

        moneyGotFromLevel = MoneyUtilities.AddMoneyForLevels(PointsUtility.GetLevelDifference(oldPoints, newPoints));
        moneyGotFromScore = MoneyUtilities.AddMoneyForScore(currentScore);
        moneyGotFromStreak = MoneyUtilities.AddMoneyForPerfectStreak(perfectStreak);
    }

    public void OnResultGet(int result)
    {
        //Result: 1 - Nice, 2 - Great, 3 - Perfect
        ShowResultMessage(result);
        FillResultVariables(result);

        Debug.Log("Player got result: " + result);
    }

    /// <summary>
    /// Создаёт в случайном месте экрана надпись: насколько точно попал игрок в колонну
    /// </summary>
    /// <param name="result">Код результата</param>
    public void ShowResultMessage(int result)
    {
        GameObject textObject = Instantiate(textPrefab, panelWhereTextSpawns);
        float panelHeight = panelWhereTextSpawns.GetComponent<RectTransform>().rect.height;
        float panelWidth = panelWhereTextSpawns.GetComponent<RectTransform>().rect.width;
        float ratio = textObject.GetComponent<RectTransform>().sizeDelta.y / textObject.GetComponent<RectTransform>().sizeDelta.x; //Height by Width
        float textWidth = Random.Range(minWidth, maxWidth);
        float textHeight = textWidth * ratio;

        //Координаты надписи
        float randomX = Random.Range(textWidth / 2f, panelWidth - (textWidth / 2f));
        float randomY = Random.Range(textHeight / 2f, panelHeight - (textHeight / 2f));
        float angleZ = Random.Range(0, maxAngleZ);

        //Надпись должна быть повёрнута к середине
        if (randomX > panelWidth/2f)
            angleZ *= -1;

        //Ставим надпись под углом
        textObject.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, angleZ);
        textObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(randomX, randomY);
        textObject.GetComponent<RectTransform>().sizeDelta = new Vector2(textWidth, textHeight);

        //Result: 1 - Nice, 2 - Great, 3 - Perfection
        textObject.GetComponent<TextMeshProUGUI>().text = LocalizationManager.instance.GetLocalizedValue("perfectionStatus" + result) + "!";
    }

    /// <summary>
    /// Увеличивает на 1 одну из переменных для финального подсчёта очков
    /// </summary>
    /// <param name="result">Код результата</param>
    private void FillResultVariables(int result) {
        switch (result) {
            case STATUS_CODE_NICE: niceCounter++; break;
            case STATUS_CODE_GREAT: greatCounter++; break;
            case STATUS_CODE_PERFECT: perfectCounter++; break;
        }

        if (result == STATUS_CODE_PERFECT)
            perfectStreak++;
        else
            perfectStreak = 0;
    }

    public int[] GetMoneyArray()
    {
        return new int[] { moneyGotFromLevel, moneyGotFromScore, moneyGotFromStreak };
    }

    /// <summary>
    /// Возвращает одну из переменных: niceCounter(1), greatCounter(2), perfectCounter(3)
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public int GetResultVariable(int result)
    {
        switch (result) {
            case STATUS_CODE_NICE: return niceCounter;
            case STATUS_CODE_GREAT: return greatCounter;
            case STATUS_CODE_PERFECT: return perfectCounter;
        }

        return 0;
    }

    public int GetCurrentScore()
    {
        return currentScore;
    }

    public void AddPoint()
    {
        currentScore++;
        playableMenu.ShowScore(currentScore);
    }
}
