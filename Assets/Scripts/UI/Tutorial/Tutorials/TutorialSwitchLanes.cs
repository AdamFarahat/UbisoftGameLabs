using System.Collections;
using UnityEngine;

public class TutorialSwitchLanes : TutorialBase
{
    [Header("Animation")]
    [SerializeField] private float laneFadeInDuration = 0.5f;

    private int gunStartingIndex = -1;
    private int swordStartingIndex = -1;

    private bool gunPlayerMoved = true;
    private bool swordPlayerMoved = true;

    protected override void StartTutorial()
    {
        if (GunPlayerController.Instance != null)
        {
            gunStartingIndex = GunPlayerController.LaneIndex;
            GunPlayerController.Instance.moveEnabled = true;
            gunPlayerMoved = false;
        }

        if (SwordPlayerController.Instance != null)
        {
            swordStartingIndex = SwordPlayerController.LaneIndex;
            SwordPlayerController.Instance.moveEnabled = true;
            swordPlayerMoved = false;
        }

        IEnumerator Routine()
        {
            while (true)
            {
                if (GunPlayerController.Instance != null)
                    gunPlayerMoved |= gunStartingIndex != GunPlayerController.LaneIndex;

                if (SwordPlayerController.Instance != null)
                    swordPlayerMoved |= swordStartingIndex != SwordPlayerController.LaneIndex;

                if (gunPlayerMoved && swordPlayerMoved)
                    break;

                yield return null;
            }

            EndTutorial();
        }

        StartCoroutine(Routine());
    }

    protected override void PreTutorial()
    {
        foreach (GameObject lane in manager.DisabledLanes)
        {
            lane.SetActive(true);
            foreach (SpriteRenderer spriteRenderer in lane.GetComponentsInChildren<SpriteRenderer>())
                StartCoroutine(FadeAnimation.FadeInRoutine(spriteRenderer, laneFadeInDuration));
        }

        foreach (LaneBar lane in FindObjectsByType<LaneBar>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            lane.gameObject.SetActive(true);
            foreach (SpriteRenderer spriteRenderer in lane.GetComponentsInChildren<SpriteRenderer>())
                StartCoroutine(FadeAnimation.FadeInRoutine(spriteRenderer, laneFadeInDuration));
        }
    }
}
