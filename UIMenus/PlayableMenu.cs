using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayableMenu : MonoBehaviour
{
    public Canvas playableCanvas;
    public TextMeshProUGUI scoreLabel;

    void Start()
    {
        if (GlobalVars.instance.HasGameStarted())
            SetVisibility(true);
        else
            SetVisibility(false);
    }

    /// <summary>
    /// Изменяет UI-текст счёта сверху экрана
    /// </summary>
    /// <param name="score">Счёт</param>
    public void ShowScore(int score)
    {
        scoreLabel.text = score.ToString();
    }

    public void SetVisibility(bool isEnabled)
    {
        playableCanvas.enabled = isEnabled;
    }
}
