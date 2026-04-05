using UnityEngine;
using FMODUnity;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private FMOD.Studio.EventInstance currentMusic;

    private EventReference currentMusicRef;

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
                 return; // Same music is already playing, do nothing
             }

            currentMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            currentMusic.release();
            currentMusic = RuntimeManager.CreateInstance(music);
            currentMusic.start();
            currentMusicRef = music;
        }
        else
        {
            currentMusic = RuntimeManager.CreateInstance(music);
            currentMusic.start();
            currentMusicRef = music;
        }
    }

    private void OnDestroy()
    {
        if (currentMusic.isValid())
        {
            currentMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            currentMusic.release();
        }

        if (Instance == this)
        {
            Instance = null;
        }
    }
}
