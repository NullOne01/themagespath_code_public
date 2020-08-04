using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsUtility
{
    private const int ADD_POINTS_NICE = 1;
    private const int ADD_POINTS_GREAT = 2;
    private const int ADD_POINTS_PERFECT = 3;
    
    private const int FIRST_LEVEL_NEED_POINTS = 10;
    private const int EACH_LEVEL_INCREASE_VALUE = 5;

    /// <summary>
    /// В зависимости от результатов игрока, мы получаем новый уровень
    /// </summary>
    /// <param name="niceCounter"></param>
    /// <param name="greatCounter"></param>
    /// <param name="perfectCounter"></param>
    public static void AddLevelProgress(int niceCounter, int greatCounter, int perfectCounter) {
        GlobalVars.instance.AddPoints( niceCounter * ADD_POINTS_NICE + 
                                       greatCounter * ADD_POINTS_GREAT +
                                       perfectCounter * ADD_POINTS_PERFECT );
    }

    public static int GetStatusPointsSum(int code, int num)
    {
        switch (code) {
            case PlayerStatus.STATUS_CODE_NICE: return num * ADD_POINTS_NICE;
            case PlayerStatus.STATUS_CODE_GREAT: return num * ADD_POINTS_GREAT;
            case PlayerStatus.STATUS_CODE_PERFECT: return num * ADD_POINTS_PERFECT;
            default: return 0;
        }
    }

    public static int GetLevel()
    {
        long points = GlobalVars.instance.GetPoints();
        int level = 0;

        while (points > 0) {
            level++;
            points -= FIRST_LEVEL_NEED_POINTS + (level - 1) * EACH_LEVEL_INCREASE_VALUE;
        }

        return level;
    }

    public static int GetLevel(long points)
    {
        int level = 0;

        do {
            level++;
            points -= FIRST_LEVEL_NEED_POINTS + (level - 1) * EACH_LEVEL_INCREASE_VALUE;
        } while (points > 0);

        return level;
    }

    public static int GetLevel(double fPoints)
    {
        int level = 0;

        do {
            level++;
            fPoints -= FIRST_LEVEL_NEED_POINTS + (level - 1) * EACH_LEVEL_INCREASE_VALUE;
        } while (fPoints > 0);

        return level;
    }

    /// <summary>
    /// Получаем процент того, сколько игрок выполнил, чтобы получить новый уровень
    /// </summary>
    /// <returns></returns>
    public static float GetLevelPercent()
    {
        long points = GlobalVars.instance.GetPoints();
        long pointsLevel = GetAN(GetLevel() - 1);
        long pointsNextLevel = GetAN(GetLevel());

        return Mathf.InverseLerp(pointsLevel, pointsNextLevel, points);
    }

    public static float GetLevelPercent(long points)
    {
        long pointsLevel = GetAN(GetLevel(points) - 1);
        long pointsNextLevel = GetAN(GetLevel(points));

        return Mathf.InverseLerp(pointsLevel, pointsNextLevel, points);
    }

    public static float GetLevelPercent(double fPoints)
    {
        long pointsLevel = GetAN(GetLevel(fPoints) - 1);
        long pointsNextLevel = GetAN(GetLevel(fPoints));

        return Mathf.InverseLerp(pointsLevel, pointsNextLevel, (float)fPoints);
    }

    /// <summary>
    /// Разница уровней между двумя разными счётами
    /// </summary>
    /// <param name="startPoints"></param>
    /// <param name="endPoints"></param>
    /// <returns></returns>
    public static int GetLevelDifference(long startPoints, long endPoints)
    {
        return GetLevel(endPoints) - GetLevel(startPoints);
    }

    /// <summary>
    /// Получаем A(n) из прогресси согласно формуле:
    /// n*A(1) + sum(d*x)(n-1 >= x >= 1)
    /// </summary>
    /// <param name="N"></param>
    /// <returns></returns>
    private static long GetAN(int N)
    {
        return N * FIRST_LEVEL_NEED_POINTS + GetArithmeticProgSum(N);
    }

    /// <summary>
    /// sum(d*x)(n-1 >= x >= 1)
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    private static int GetArithmeticProgSum(int n) {
        int sum = 0;
        for (int i = 1; i < n; i++)
            sum += EACH_LEVEL_INCREASE_VALUE * i;
        return sum;
    }
}
