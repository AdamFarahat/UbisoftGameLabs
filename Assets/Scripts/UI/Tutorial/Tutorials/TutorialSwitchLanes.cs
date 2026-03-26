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
            gunStartingIndex = Mathf.FloorToInt(GunPlayerController.LaneIndex);
            GunPlayerController.Instance.moveEnabled = true;
            gunPlayerMoved = false;
        }

        if (SwordPlayerController.Instance != null)
        {
            swordStartingIndex = Mathf.FloorToInt(SwordPlayerController.LaneIndex);
            SwordPlayerController.Instance.moveEnabled = true;
            swordPlayerMoved = false;
        }

        foreach (SpriteRenderer lane in manager.DisabledLanes)
        {
            StartCoroutine(FadeAnimation.FadeInRoutine(lane, laneFadeInDuration));
            lane.gameObject.SetActive(true);
        }

        IEnumerator Routine()
        {
            while (true)
            {
                if (GunPlayerController.Instance != null)
                    gunPlayerMoved |= gunStartingIndex != Mathf.FloorToInt(GunPlayerController.LaneIndex);

                if (SwordPlayerController.Instance != null)
                    swordPlayerMoved |= swordStartingIndex != Mathf.FloorToInt(SwordPlayerController.LaneIndex);

                if (gunPlayerMoved && swordPlayerMoved)
                    break;

                yield return null;
            }

            EndTutorial();
        }

        StartCoroutine(Routine());
    }
}
