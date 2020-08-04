using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMovement : MonoBehaviour {
    public WeatherTracker weather;

    [SerializeField]
    private GameObject sampleBackgroundQuad;
    private BackgroundObject showingBackgroundObject;

    public List<BackgroundObject> backgrounds;

    [SerializeField]
    private float distanceMultiplier = 0.1f;
    //paralaxMultiplier и distanceMultiplier используются для финального подсчёта Paralax. resultMultiplier = distance * paralax^(i)
    [SerializeField]
    private float paralaxMultiplier = 0.5f;

    [SerializeField]
    private float distancePassive = 1f;

    /// <summary>
    /// Background'ы идут в списке от самого ближнего до самого дальнего
    /// Самый первый background - самый близкий
    /// </summary>

    private void Start()
    {
        SetBackground(GetCurrentBackground());
        showingBackgroundObject = GetCurrentBackground();
    }

    private void FixedUpdate() {
        MoveOnDistancePassive();
    }

    public void MoveOnDistancePassive() {
        float resultMultiplier = distanceMultiplier;
        for (int i = 0; i < showingBackgroundObject.bgMaterialLayers.Count; i++) {
            GameObject child = transform.GetChild(i).gameObject;

            if (i < showingBackgroundObject.bgSpeedLayers.Count)
                child.GetComponent<Renderer>().material.mainTextureOffset += new Vector2(resultMultiplier * showingBackgroundObject.bgSpeedLayers[i], 0);
            resultMultiplier *= paralaxMultiplier;

            if (i == 0) {
                //x % 1 -> находим прогресс фона. Пример: 3.56f % 1 = 0.56f
                weather.CheckWeather(showingBackgroundObject, child.GetComponent<Renderer>().material.mainTextureOffset.x % 1);
            }
        }
    }

    public void MoveOnDistance(float distance)
    {
        float resultMultiplier = distanceMultiplier;
        for (int i = 0; i < GetCurrentBackground().bgMaterialLayers.Count; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;

            child.GetComponent<Renderer>().material.mainTextureOffset += new Vector2(distance * resultMultiplier, 0);
            resultMultiplier *= paralaxMultiplier;

            if (i == 0)
            {
                //x % 1 -> находим прогресс фона. Пример: 3.56f % 1 = 0.56f
                weather.CheckWeather(GetCurrentBackground(), child.GetComponent<Renderer>().material.mainTextureOffset.x % 1);
            }
        }
    }

    public void SetBackground(BackgroundObject background)
    {
        showingBackgroundObject = background;
        ClearBackgrounds();

        for (int i = 0; i < background.bgMaterialLayers.Count; i++)
        {
            GameObject newBackgroundQuad = Instantiate(sampleBackgroundQuad, transform);
            newBackgroundQuad.transform.position += new Vector3(0, 0, 1f);

            newBackgroundQuad.GetComponent<Renderer>().material = background.bgMaterialLayers[i];
            newBackgroundQuad.GetComponent<Renderer>().material.mainTextureOffset += new Vector2(Random.Range(0, 1f), 0);
            if (i == 0)
                weather.CheckWeather(GetCurrentBackground(), newBackgroundQuad.GetComponent<Renderer>().material.mainTextureOffset.x % 1);
        }

        GameObject.Find("CloudPlane").GetComponent<JustCloud>().SetColor(background.cloudColor);
    }

    private void ClearBackgrounds()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void SetBackground(int bgID)
    {
        SetBackground(backgrounds[bgID]);
    }

    public string GetName(int bgID)
    {
        return LocalizationManager.instance.GetLocalizedValue(backgrounds[bgID].bgName);
    }

    public int GetCost(int bgID)
    {
        return backgrounds[bgID].cost;
    }

    public BackgroundObject GetCurrentBackground()
    {
        return backgrounds[GlobalVars.instance.GetCurrentBackground()];
    }
}
