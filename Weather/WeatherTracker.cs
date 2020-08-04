using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherTracker : MonoBehaviour
{
    public void CheckWeather(BackgroundObject background, float backProgress)
    {
        if (background.hasRain) {
            if (IsProgressBetween(backProgress, background.rainFrom, background.rainTo)) {
                GetComponent<BaseWeather>().TurnOnRain();
                return;
            }
        }

        if (background.hasSnow) {
            if (IsProgressBetween(backProgress, background.snowFrom, background.snowTo)) {
                GetComponent<BaseWeather>().TurnOnSnow();
                return;
            }
        }

        GetComponent<BaseWeather>().TurnOffWeather();
    }

    /// <summary>
    /// Находится ли игрок на определённом промежутке отрезка?
    /// </summary>
    /// <param name="backProgress">Положение игрока</param>
    /// <param name="from">Начало отрезка</param>
    /// <param name="to">Конец отрезка</param>
    /// <returns></returns>
    private bool IsProgressBetween(float backProgress, float from, float to)
    {
        if (from == to)
            return false;

        if (from < to)
            return (backProgress >= from) && (backProgress <= to);
        else
            return ! ( (backProgress > to ) && (backProgress < from) );
    }
}
