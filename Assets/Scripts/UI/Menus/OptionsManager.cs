using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    [SerializeField] private Slider FontSizeSlider;
    [SerializeField] private Slider UIScalingSlider;
    [SerializeField] private Slider EnemyEmissionSlider;
    [SerializeField] private Slider UIEmissionSlider;
    [SerializeField] private Slider QualitySlider;
    [SerializeField] private Slider MasterVolumeSlider;
    [SerializeField] private Slider MusicVolumeSlider;
    [SerializeField] private Slider SFXVolumeSlider;
    [SerializeField] private Button BackBtn;
    [SerializeField] private Button ResetScoreBtn;

    [SerializeField] private GameObject PauseMenu;

    [SerializeField] private EventSystem eventSystem;

    void Awake()
    {
        Assert.IsNotNull(FontSizeSlider);
        Assert.IsNotNull(UIScalingSlider);
        Assert.IsNotNull(EnemyEmissionSlider);
        Assert.IsNotNull(UIEmissionSlider);
        Assert.IsNotNull(QualitySlider);
        Assert.IsNotNull(MasterVolumeSlider);
        Assert.IsNotNull(MusicVolumeSlider);
        Assert.IsNotNull(SFXVolumeSlider);

        
    }

    private void OnEnable()
    {
        gameObject.SetActive(true);

        FontSizeSlider.value = Settings.Instance.fontSizePourcentage;
        UIScalingSlider.value = Settings.Instance.uiScalingPourcentage;
        EnemyEmissionSlider.value = Settings.Instance.enemyEmissionIntensity;
        UIEmissionSlider.value = Settings.Instance.uiEmissionIntensity;
        QualitySlider.value = QualitySettings.GetQualityLevel();

        FontSizeSlider.onValueChanged.AddListener(OnFontSizeSliderChange);
        UIScalingSlider.onValueChanged.AddListener(OnUIScalingSliderChange);
        EnemyEmissionSlider.onValueChanged.AddListener(OnEnemyEmissionSliderChange);
        UIEmissionSlider.onValueChanged.AddListener(OnUIEmissionSliderChange);
        QualitySlider.onValueChanged.AddListener(OnQualitySliderChange);
        MasterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeSliderChange);
        MusicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeSliderSliderChange);
        SFXVolumeSlider.onValueChanged.AddListener(OnSFXVolumeSliderChange);
        ResetScoreBtn.onClick.AddListener(OnResetClick);
        BackBtn.onClick.AddListener(OnBackBtnClick);


        IEnumerator setSelectedButtonNextFrame()
        {
            // Wait for the end of the frame to ensure the UI is fully active
            yield return new WaitForEndOfFrame();
            eventSystem.SetSelectedGameObject(FontSizeSlider.gameObject);
        }
        StartCoroutine(setSelectedButtonNextFrame());
    }

    void OnDisable()
    {
        FontSizeSlider.onValueChanged.RemoveListener(OnFontSizeSliderChange);
        UIScalingSlider.onValueChanged.RemoveListener(OnUIScalingSliderChange);
        EnemyEmissionSlider.onValueChanged.RemoveListener(OnEnemyEmissionSliderChange);
        UIEmissionSlider.onValueChanged.RemoveListener(OnUIEmissionSliderChange);
        QualitySlider.onValueChanged.RemoveListener(OnQualitySliderChange);
        BackBtn.onClick.RemoveListener(OnBackBtnClick);
        ResetScoreBtn.onClick.RemoveListener(OnResetClick);
    }

    private void OnResetClick()
    {
        //TODO: implement reset score functionality
    }

    private void OnBackBtnClick()
    {
        if (SceneManager.GetActiveScene().name == "Options" || PauseMenu == null)
        {
            SceneManager.LoadScene("Menu");
        }
        else
        {
            gameObject.SetActive(false);
            if (PauseMenu != null)
                PauseMenu.SetActive(true);

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
    
    private void OnEnemyEmissionSliderChange(float value)
    {
        Settings.Instance.enemyEmissionIntensity = value;
        Settings.OnUpdateEnemyEmissionPercentage();

    }
    
    private void OnUIEmissionSliderChange(float value)
    {
        Settings.Instance.uiEmissionIntensity = value;
        Settings.OnUpdateUIEmissionPercentage();

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
