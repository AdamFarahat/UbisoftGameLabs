using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private Transform tipsParent;
    [SerializeField] private float tipDuration = 3f;

    private GameObject[] tips;
    private int tipIndex = -1;
    private float tipAge = 0f;

    private void Awake()
    {
        Assert.IsNotNull(tipsParent);

        List<GameObject> tipsList = new();
        foreach (Transform child in tipsParent)
        {
            child.gameObject.SetActive(false);
            tipsList.Add(child.gameObject);
        }
        tips = tipsList.ToArray();
        Assert.IsTrue(tips.Length > 0);
        tipIndex = tips.Length - 1;
    }

    private void Start()
    {
        if (GunPlayerController.Instance != null)
            GunPlayerController.Instance.StartButtonPressed += OnStartButtonPressed;

        if (SwordPlayerController.Instance != null)
            SwordPlayerController.Instance.StartButtonPressed += OnStartButtonPressed;

        ShowNextTip();
    }

    private void Update()
    {
        tipAge += Time.deltaTime;
        while (tipAge >= tipDuration)
        {
            tipAge -= tipDuration;
            ShowNextTip();
        }
    }

    private void OnStartButtonPressed()
    {
        SceneManager.LoadScene("Menu");
    }

    private void ShowNextTip()
    {
        // TODO animate
        tips[tipIndex].SetActive(false);
        tipIndex = (tipIndex + 1) % tips.Length;
        tips[tipIndex].SetActive(true);
    }
}
