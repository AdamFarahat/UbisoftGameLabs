using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Slider fontSizeSlider;
    [SerializeField] private Slider uiScalingSlider;
    [SerializeField] private Slider enemyEmissionSlider;
    [SerializeField] private Slider uiEmissionSlider;
    [SerializeField] private Slider qualitySlider;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Button backBtn;
    [SerializeField] private Button resetDataBtn;

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private EventSystem eventSystem;

    [Header("Params")]
    [SerializeField] private float enemyEmissionMin = 0f;
    [SerializeField] private float enemyEmissionMid = 1f;
    [SerializeField] private float enemyEmissionMax = 10f;
    [SerializeField] private float uiEmissionMin = 0f;
    [SerializeField] private float uiEmissionMid = 1f;
    [SerializeField] private float uiEmissionMax = 10f;

    void Awake()
    {
        Assert.IsNotNull(fontSizeSlider);
        Assert.IsNotNull(uiScalingSlider);
        Assert.IsNotNull(enemyEmissionSlider);
        Assert.IsNotNull(uiEmissionSlider);
        Assert.IsNotNull(qualitySlider);
        Assert.IsNotNull(masterVolumeSlider);
        Assert.IsNotNull(musicVolumeSlider);
        Assert.IsNotNull(sfxVolumeSlider);

        Assert.IsNotNull(pauseMenu);
        Assert.IsNotNull(eventSystem);
    }

    private void OnEnable()
    {
        gameObject.SetActive(true);

        // TODO load from persistent data
        fontSizeSlider.value = Settings.Instance.fontSizePourcentage;
        uiScalingSlider.value = Settings.Instance.uiScalingPourcentage;
        //enemyEmissionSlider.value = Mathf.Lerp(enemyEmissionSlider.minValue, enemyEmissionSlider.maxValue, 0.5f);
        //uiEmissionSlider.value = Mathf.Lerp(uiEmissionSlider.minValue, uiEmissionSlider.maxValue, 0.5f);
        qualitySlider.value = QualitySettings.GetQualityLevel();

        fontSizeSlider.onValueChanged.AddListener(OnFontSizeSliderChange);
        uiScalingSlider.onValueChanged.AddListener(OnUIScalingSliderChange);
        enemyEmissionSlider.onValueChanged.AddListener(OnEnemyEmissionSliderChange);
        uiEmissionSlider.onValueChanged.AddListener(OnUIEmissionSliderChange);
        qualitySlider.onValueChanged.AddListener(OnQualitySliderChange);
        masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeSliderChange);
        musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeSliderSliderChange);
        sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeSliderChange);
        resetDataBtn.onClick.AddListener(OnResetDataClick);
        backBtn.onClick.AddListener(OnBackBtnClick);


        IEnumerator setSelectedButtonNextFrame()
        {
            // Wait for the end of the frame to ensure the UI is fully active
            yield return new WaitForEndOfFrame();
            eventSystem.SetSelectedGameObject(fontSizeSlider.gameObject);
        }
        StartCoroutine(setSelectedButtonNextFrame());
    }

    void OnDisable()
    {
        fontSizeSlider.onValueChanged.RemoveListener(OnFontSizeSliderChange);
        uiScalingSlider.onValueChanged.RemoveListener(OnUIScalingSliderChange);
        enemyEmissionSlider.onValueChanged.RemoveListener(OnEnemyEmissionSliderChange);
        uiEmissionSlider.onValueChanged.RemoveListener(OnUIEmissionSliderChange);
        qualitySlider.onValueChanged.RemoveListener(OnQualitySliderChange);
        backBtn.onClick.RemoveListener(OnBackBtnClick);
        resetDataBtn.onClick.RemoveListener(OnResetDataClick);
    }

    private void OnResetDataClick()
    {
        //TODO: implement reset data functionality
    }

    private void OnBackBtnClick()
    {
        if (SceneManager.GetActiveScene().name == "Options" || pauseMenu == null)
        {
            SceneManager.LoadScene("Menu");
        }
        else
        {
            gameObject.SetActive(false);
            if (pauseMenu != null)
                pauseMenu.SetActive(true);

        }
    }

    private void OnFontSizeSliderChange(float value)
    {
        Settings.Instance.fontSizePourcentage = value;
    }

    private void OnUIScalingSliderChange(float value)
    {
        Settings.Instance.uiScalingPourcentage = value;
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
