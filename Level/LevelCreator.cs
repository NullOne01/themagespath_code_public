using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCreator : MonoBehaviour
{
    public GameObject columnPrefab;
    public GameObject excellentPointPrefab;

    [SerializeField]
    private float startMaxScale = 1.5f;
    [SerializeField]
    private float startMinScale = 1;
    [SerializeField]
    private float minScale = 0.5f;
    [SerializeField]
    private float decreaseEachPoint = 0.25f;
    [SerializeField]
    private float positionYGrowning = -10f;

    private GameObject currentColumn;
    private GameObject prevColumn;
    private Camera mainCamera;


    private float minLeftX;
    private float maxRightX;
    [SerializeField]
    private float positionY = -1;

    void Start()
    {
        mainCamera = Camera.main;
        //Первая колонна
        prevColumn = gameObject.transform.GetChild(0).gameObject;

        StartCreatingLevel();

        if (!GlobalVars.instance.HasGameStarted())
            currentColumn.transform.position = new Vector3(currentColumn.transform.position.x, positionYGrowning, currentColumn.transform.position.z);
    }

    public void StartCreatingLevel()
    {
        CreateNextColumn(0);
    }

    public void CreateNextColumn(float addPos)
    {
        //Правая граница, где могут появлятся колонны
        maxRightX = mainCamera.ViewportToWorldPoint(new Vector3(1, 0.5f, mainCamera.nearClipPlane)).x;
        //Левая граница, где могут появлятся колонны (1/3f)
        minLeftX = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, mainCamera.nearClipPlane)).x;

        //Получаем рандомный размер по оси X (с учётом счёта игрока)
        float scaleX = GetRandomScaleX();
        //Получаем рандомную позицию по оси X (с учётом границ и размеров колонны)
        float positionX = GetRandomPositionX(scaleX) + addPos;
        //Создаём колонну, теперь она - наша цель, прошлую также сохраняем
        if (currentColumn != null)
            prevColumn = currentColumn;
        currentColumn = CreateColumn(positionX, scaleX);
    }

    private float GetRandomPositionX(float scaleX)
    {
        //Координаты с учётом размеров объекта
        float minX = minLeftX + SpriteUtilities.GetSpriteWidth(columnPrefab, scaleX) / 2;
        float maxX = maxRightX - SpriteUtilities.GetSpriteWidth(columnPrefab, scaleX) / 2;
        return Random.Range(minX, maxX);
    }

    private float GetRandomScaleX()
    {
        PlayerStatus playerStatus = Camera.main.GetComponent<PlayerStatus>();
        float score = playerStatus.GetCurrentScore();

        //Число не может быть меньше минимального
        float randomScale = Random.Range(startMinScale - score * decreaseEachPoint, startMaxScale - score * decreaseEachPoint);
        if (randomScale < minScale)
            randomScale = minScale;

        return randomScale;
    }

    private GameObject CreateColumn(float positionX, float scaleX)
    {
        GameObject go = Instantiate(columnPrefab, new Vector3(positionX, positionY, 0), Quaternion.identity, transform);
        go.transform.localScale = new Vector3(scaleX, go.transform.localScale.y, go.transform.localScale.z);
        return go;
    }

    /// <summary>
    /// Меняет Y-координату колонны от positionYGrowning до positionY в зависимости от процента percent
    /// </summary>
    /// <param name="percent">
    /// percent = [0,1]
    /// </param>
    public void SetCurrentColumnGrowningPercent(float percent)
    {
        float newY = Mathf.Lerp(positionYGrowning, positionY, percent);
        currentColumn.transform.position = new Vector3(currentColumn.transform.position.x, newY, currentColumn.transform.position.z);
    }

    public GameObject GetCurrentColumn()
    {
        return currentColumn;
    }

    public float GetCurrentColumnWidth()
    {
        return SpriteUtilities.GetSpriteWidth(currentColumn);
    }

    public float GetPrevColumnWidth()
    {
        return SpriteUtilities.GetSpriteWidth(prevColumn);
    }

    public GameObject GetPrevColumn()
    {
        return prevColumn;
    }
}
