using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private EventInstance currentMusic;
    private EventReference currentMusicRef;
    private Bus masterBus;
    private Bus musicBus;
    private Bus sfxBus;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple instances of AudioManager detected! Destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        masterBus = RuntimeManager.GetBus("bus:/");
        musicBus = RuntimeManager.GetBus("bus:/Music");
        sfxBus = RuntimeManager.GetBus("bus:/SFX");
    }

    public void ChangeMasterVolume(float volume)
    {
        Debug.Log($"Changing master volume to {volume}");
        masterBus.setVolume(Mathf.Clamp01(volume));
    }

    public void ChangeMusicVolume(float volume)
    {
        musicBus.setVolume(Mathf.Clamp01(volume));
    }

    public void ChangeSFXVolume(float volume)
    {
        sfxBus.setVolume(Mathf.Clamp01(volume));
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    public void PlayMusic(EventReference music)
    {
        if (currentMusic.isValid())
        {
            if (currentMusicRef.Equals(music))
            {
                return;
            }

            currentMusic.stop(STOP_MODE.ALLOWFADEOUT);
            currentMusic.release();
        }

        currentMusic = RuntimeManager.CreateInstance(music);
        currentMusic.start();
        currentMusicRef = music;
    }

    public EventInstance PlayLooping(EventReference sound, GameObject obj)
    {
        EventInstance instance = RuntimeManager.CreateInstance(sound);
        RuntimeManager.AttachInstanceToGameObject(instance, obj);
        instance.start();
        return instance;
    }

    private void OnDestroy()
    {
        if (currentMusic.isValid())
        {
            currentMusic.stop(STOP_MODE.ALLOWFADEOUT);
            currentMusic.release();
        }

        if (Instance == this)
        {
            Instance = null;
        }
    }

    public float GetMasterVolume()
    {
        return masterBus.getVolume(out float volume) == FMOD.RESULT.OK ? volume : 1f;
    }

    public float GetMusicVolume()
    {
        return musicBus.getVolume(out float volume) == FMOD.RESULT.OK ? volume : 1f;
    }

    public float GetSFXVolume()
    {
        return sfxBus.getVolume(out float volume) == FMOD.RESULT.OK ? volume : 1f;
    }
}
