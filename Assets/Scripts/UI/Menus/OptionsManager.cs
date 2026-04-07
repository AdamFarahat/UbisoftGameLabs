using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Slider enemyEmissionSlider;
    [SerializeField] private Slider uiEmissionSlider;
    [SerializeField] private Slider qualitySlider;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;


    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private EventSystem eventSystem;

    [Header("Params")]
    [SerializeField] private float enemyEmissionMin = 0f;
    [SerializeField] private float enemyEmissionMid = 1f;
    [SerializeField] private float enemyEmissionMax = 10f;
    [SerializeField] private float uiEmissionMin = 0f;
    [SerializeField] private float uiEmissionMid = 1f;
    [SerializeField] private float uiEmissionMax = 10f;


    [SerializeField] private float enemyEmissionSliderDefaultValue = 0.5f;
    [SerializeField] private float uiEmissionSliderDefaultValue = 0.5f;
    [SerializeField] private float qualitySliderDefaultValue = 3;
    [SerializeField] private float masterVolumeSliderDefaultValue = 0.5f;
    [SerializeField] private float musicVolumeSliderDefaultValue = 0.5f;
    [SerializeField] private float sfxVolumeSliderDefaultValue = 0.5f;
    void Awake()
    {
        Assert.IsNotNull(enemyEmissionSlider);
        Assert.IsNotNull(uiEmissionSlider);
        Assert.IsNotNull(qualitySlider);
        Assert.IsNotNull(masterVolumeSlider);
        Assert.IsNotNull(musicVolumeSlider);
        Assert.IsNotNull(sfxVolumeSlider);
        if (pauseMenu == null) { 
            Debug.LogWarning("Pause menu reference is not set in OptionsManager. " +
                "This is only a problem if the options menu is used in-game and not from the main menu.");
        }
        Assert.IsNotNull(eventSystem);
    }
    
    private void OnEnable()
    {
        gameObject.SetActive(true);

        // TODO load from persistent data
        enemyEmissionSlider.value = Settings.Instance.EnemyEmissionIntensity;
        uiEmissionSlider.value = Settings.Instance.UIEmissionIntensity;
        qualitySlider.value = QualitySettings.GetQualityLevel();
        masterVolumeSlider.value = AudioManager.Instance.GetMasterVolume();
        musicVolumeSlider.value = AudioManager.Instance.GetMusicVolume();
        sfxVolumeSlider.value = AudioManager.Instance.GetSFXVolume();

        
        enemyEmissionSlider.onValueChanged.AddListener(OnEnemyEmissionSliderChange);
        uiEmissionSlider.onValueChanged.AddListener(OnUIEmissionSliderChange);
        qualitySlider.onValueChanged.AddListener(OnQualitySliderChange);
        masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeSliderChange);
        musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeSliderSliderChange);
        sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeSliderChange);


        IEnumerator setSelectedButtonNextFrame()
        {
            // Wait for the end of the frame to ensure the UI is fully active
            yield return new WaitForEndOfFrame();
            eventSystem.SetSelectedGameObject(enemyEmissionSlider.gameObject);
        }
        StartCoroutine(setSelectedButtonNextFrame());
    }

    void OnDisable()
    {

        enemyEmissionSlider.onValueChanged.RemoveListener(OnEnemyEmissionSliderChange);
        uiEmissionSlider.onValueChanged.RemoveListener(OnUIEmissionSliderChange);
        qualitySlider.onValueChanged.RemoveListener(OnQualitySliderChange);
    }

    // This is called by the input system when the cancel button is pressed.
    // It will trigger the same behavior as clicking the back button.
    // Hooked up to the 'Cancel' action in the Player Input component
    public void OnCancel()
    {
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UICancel, Vector3.zero);
        if (SceneManager.GetActiveScene().name == "Options" || pauseMenu == null)
        {
            SceneManager.LoadScene("Menu");
        }
        else
        {
            gameObject.SetActive(false);
            if (pauseMenu != null)
                pauseMenu.SetActive(true);
            else {
                Debug.LogError("PAUSE MENU IS NON-NULL, THIS SHOULD ONLY BE POSSIBLE IN THE OPTIONS LEVEL");
            }

        } 
    }

    public void OnReset() {
        //TODO: implement reset data functionality
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UIPress, Vector3.zero);
        ResetSettingsValuesToDefaut();
    }

    private void ResetSettingsValuesToDefaut()
    {
        enemyEmissionSlider.value = enemyEmissionSliderDefaultValue;
        uiEmissionSlider.value = uiEmissionSliderDefaultValue;
        qualitySlider.value = qualitySliderDefaultValue;
        masterVolumeSlider.value = masterVolumeSliderDefaultValue;
        musicVolumeSlider.value = musicVolumeSliderDefaultValue;
        sfxVolumeSlider.value = sfxVolumeSliderDefaultValue;
        //TODO: Reset High Score here.
    }

    

    private void OnEnemyEmissionSliderChange(float _)
    {
        Settings.OnUpdateEnemyEmissionPercentage(RedistributedSliderValue(enemyEmissionSlider, enemyEmissionMin, enemyEmissionMid, enemyEmissionMax));

    }
    
    private void OnUIEmissionSliderChange(float _)
    {
        Settings.OnUpdateUIEmissionPercentage(RedistributedSliderValue(uiEmissionSlider, uiEmissionMin, uiEmissionMid, uiEmissionMax));
    }

    private float RedistributedSliderValue(Slider slider, float min, float mid, float max)
    {
        float a = Mathf.InverseLerp(slider.minValue, slider.maxValue, slider.value);

        if (a < 0.5f)
            return Mathf.Lerp(min, mid, a * 2f);
        else
            return Mathf.Lerp(mid, max, a * 2f - 1f);
    }
    
    private void OnSFXVolumeSliderChange(float value)
    {
        AudioManager.Instance.ChangeSFXVolume(value);
    }
    
    private void OnMasterVolumeSliderChange(float value)
    {
        AudioManager.Instance.ChangeMasterVolume(value);
    }
    
    private void OnMusicVolumeSliderSliderChange(float value)
    {
        AudioManager.Instance.ChangeMusicVolume(value);
    }
    
    private void OnQualitySliderChange(float value)
    {
        Debug.Log("Quality slider changed to " + value);
        QualitySettings.SetQualityLevel((int)value);
    }
}
