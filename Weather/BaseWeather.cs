using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWeather : MonoBehaviour
{
    public ParticleSystem snowParticle;
    public ParticleSystem rainParticle;

    void Awake()
    {
        GetComponent<WeatherTracker>().CheckWeather(GameObject.FindGameObjectWithTag("Background").GetComponent<BackgroundMovement>().GetCurrentBackground(), 0);
    }

    public void TurnOnRain()
    {
        if ((rainParticle.isPlaying) && (snowParticle.isStopped))
            return;

        rainParticle.Play();
        snowParticle.Stop();
    }

    public void TurnOnSnow()
    {
        if ((snowParticle.isPlaying) && (rainParticle.isStopped))
            return;

        snowParticle.Play();
        rainParticle.Stop();
    }

    public void TurnOffWeather()
    {
        if ((snowParticle.isStopped) && (rainParticle.isStopped))
            return;

        snowParticle.Stop();
        rainParticle.Stop();
    }
}
