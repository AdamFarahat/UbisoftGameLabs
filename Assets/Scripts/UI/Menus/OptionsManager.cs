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

    [SerializeField] private GameObject PauseMenu;
    [SerializeField] private static float offset = 0.5f;

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
    void OnDisable()
    {
        FontSizeSlider.onValueChanged.RemoveListener(OnFontSizeSliderChange);
        UIScalingSlider.onValueChanged.RemoveListener(OnUIScalingSliderChange);
        EnemyEmissionSlider.onValueChanged.RemoveListener(OnEnemyEmissionSliderChange);
        UIEmissionSlider.onValueChanged.RemoveListener(OnFUIEmissionSliderChange);
        QualitySlider.onValueChanged.RemoveListener(OnFontSizeSliderChange);
        BackBtn.onClick.RemoveListener(OnBackBtnClick);
    }

    private void OnBackBtnClick()
    {
        if (SceneManager.GetActiveScene().name == "Options" || PauseMenu == null)
        {
            SceneManager.LoadScene("Menu");
        }
        else { 
            gameObject.SetActive(false);
            if (PauseMenu != null)
                PauseMenu.SetActive(true);

        }
    }

    private void OnFontSizeSliderChange(float value)
    {
        Settings.fontSizePourcentage = 1 + (value - 0.5f);
    }
    private void OnUIScalingSliderChange(float value)
    {
        Settings.UIScalingPourcentage = 1 + (value - 0.5f);
    }
    private void OnEnemyEmissionSliderChange(float value)
    {
        Settings.EnemyEmissionPourcentage = 1 + (value - 0.5f);

    }
    private void OnFUIEmissionSliderChange(float value)
    {
        Settings.EnemyEmissionPourcentage = 1 + (value - 0.5f);

    }
    private void OnQualitySliderChange(float value)
    {
        Debug.Log($"Quality slider changed to {value}");
    }

    private void OnEnable()
    {       
        gameObject.SetActive(true);

        FontSizeSlider.value = Settings.fontSizePourcentage - offset;
        UIScalingSlider.value = Settings.UIScalingPourcentage - offset;
        EnemyEmissionSlider.value = Settings.EnemyEmissionPourcentage - offset;
        UIEmissionSlider.value = Settings.UIEmissionPourcentage - offset;

        FontSizeSlider.onValueChanged.AddListener(OnFontSizeSliderChange);
        UIScalingSlider.onValueChanged.AddListener(OnUIScalingSliderChange);
        EnemyEmissionSlider.onValueChanged.AddListener(OnEnemyEmissionSliderChange);
        UIEmissionSlider.onValueChanged.AddListener(OnFUIEmissionSliderChange);
        QualitySlider.onValueChanged.AddListener(OnFontSizeSliderChange);         BackBtn.onClick.AddListener(OnBackBtnClick);
        IEnumerator setSelectedButtonNextFrame()
        {
            // Wait for the end of the frame to ensure the UI is fully active
            yield return new WaitForEndOfFrame();
            eventSystem.SetSelectedGameObject(FontSizeSlider.gameObject);
        }
        StartCoroutine(setSelectedButtonNextFrame());
    }



}
