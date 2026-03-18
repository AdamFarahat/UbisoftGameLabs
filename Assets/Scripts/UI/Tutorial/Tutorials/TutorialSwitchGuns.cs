using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class TutorialSwitchGuns : TutorialBase
{
    [Header("General")]
    [SerializeField] private float minDuration = 8f;
    [SerializeField] private GameObject meleeGruntsRoot;

    [Header("Descriptions")]
    [SerializeField] private TextMeshProUGUI firstDescription;
    [SerializeField] private TextMeshProUGUI secondDescription;

    private MeleeGruntMovementAI[] meleeGrunts;

    private readonly HashSet<int> gunsNotSeen = new();
    private bool transitionsSwitched = false;

    protected override void Awake()
    {
        base.Awake();
        
        Assert.IsNotNull(meleeGruntsRoot);
        meleeGrunts = meleeGruntsRoot.GetComponentsInChildren<MeleeGruntMovementAI>();

        Assert.IsNotNull(firstDescription);
        Assert.IsNotNull(secondDescription);

        secondDescription.GetComponent<RectTransform>().localScale = new(1f, 0f, 1f);

        foreach (MeleeGruntMovementAI meleeGrunt in meleeGrunts)
            if (meleeGrunt != null)
                meleeGrunt.gameObject.SetActive(false);
    }

    protected override void StartTutorial()
    {
        GunPlayerController gunPlayer = GunPlayerController.Instance;

        if (gunPlayer == null)
        {
            EndTutorial();
            return;
        }

        for (int i = 0; i < gunPlayer.Holster.NumberOfGuns; i++)
        {
            if (i != gunPlayer.Holster.ActiveGunIndex)
                gunsNotSeen.Add(i);
        }

        gunPlayer.toggleGunEnabled = true;
        gunPlayer.PressedToggle += ShowSecondDescription;

        float age = Time.time;
        IEnumerator Routine()
        {
            foreach (MeleeGruntMovementAI meleeGrunt in meleeGrunts)
                meleeGrunt.gameObject.SetActive(true);

            while (gunsNotSeen.Count > 0)
            {
                gunsNotSeen.Remove(gunPlayer.Holster.ActiveGunIndex);
                yield return null;
            }

            float duration = Time.time - age;
            if (duration < minDuration)
                yield return new WaitForSeconds(minDuration - duration);

            EndTutorial();
        }

        StartCoroutine(Routine());
    }

    private void ShowSecondDescription()
    {
        if (transitionsSwitched)
            return;
        transitionsSwitched = true;

        IEnumerator Transition()
        {
            yield return FadeOutRoutine(firstDescription.gameObject);
            yield return FadeInRoutine(secondDescription.gameObject);
        }

        StartCoroutine(Transition());
    }
}
