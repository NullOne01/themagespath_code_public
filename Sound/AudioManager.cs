using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public Sound[] sounds;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        foreach (Sound sound in sounds) {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.loop = sound.loop;
        }
    }

    private void Update() {
        if (!Camera.main.GetComponent<DeadMenu>().GetVisibility())
            Stop("ExpGain1");
    }

    public void Play(string soundName)
    {
        Sound playSound = System.Array.Find(sounds, sound => sound.name == soundName);
        if (playSound == null) {
            Debug.LogWarning("Sound with name: " + soundName + " wasn't found!");
            return;
        }
        playSound.source.Play();
        playSound.source.loop = playSound.loop;
    }

    public void Stop(string soundName) {
        Sound playSound = System.Array.Find(sounds, sound => sound.name == soundName);
        if (playSound == null) {
            Debug.LogWarning("Sound with name: " + soundName + " wasn't found!");
            return;
        }
        playSound.source.Stop();
    }

    public void PlayRandom(string soundPrefix)
    {
        int maxSoundIndex = 0;
        Sound playSound;
        do {
            maxSoundIndex++;
            playSound = System.Array.Find(sounds, sound => sound.name == soundPrefix + maxSoundIndex);
        } while (playSound != null);

        int randomInt = Random.Range(1, maxSoundIndex);
        playSound = System.Array.Find(sounds, sound => sound.name == soundPrefix + randomInt);
        playSound.source.Play();
        playSound.source.loop = playSound.loop;
    }
}
