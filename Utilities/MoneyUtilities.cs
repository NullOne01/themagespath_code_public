using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyUtilities
{
    public const int MONEY_PER_LEVEL = 5;

    public const int SOME_SCORE = 5;
    public const int MONEY_PER_SOME_SCORE = 5;

    public const int SOME_STREAK = 3;
    public const int MONEY_FOR_SCORE_ON_STREAK = 1;

    /// <summary>
    /// Прибавляет деньги в зависимости от того, сколько игрок получил уровней
    /// </summary>
    /// <param name="levels"></param>
    /// <returns></returns>
    public static int AddMoneyForLevels(int levels)
    {
        int money = levels * MONEY_PER_LEVEL;
        GlobalVars.instance.AddMoney(money);
        return money;
    }

    /// <summary>
    /// Прибавляет деньги в зависимости от того, сколько игрок прошёл
    /// </summary>
    /// <param name="score"></param>
    /// <returns></returns>
    public static int AddMoneyForScore(int score)
    {
        int money = (score / SOME_SCORE) * MONEY_PER_SOME_SCORE;
        GlobalVars.instance.AddMoney(money);
        return money;
    }

    /// <summary>
    /// Прибавляет деньги в зависимости от превосходного стрика игрока
    /// </summary>
    /// <param name="perfectStreak"></param>
    /// <returns></returns>
    public static int AddMoneyForPerfectStreak(int perfectStreak)
    {
        int actualStreak = perfectStreak - SOME_STREAK;
        if (actualStreak <= 0)
            return 0;

        int money = actualStreak * MONEY_FOR_SCORE_ON_STREAK;
        GlobalVars.instance.AddMoney(money);
        return money;
    }
}
