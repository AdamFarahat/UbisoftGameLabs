using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class TutorialJump : TutorialBase
{
    [SerializeField] private GameObject gunGruntsRoot;
    [SerializeField] private float spawnDelay = 0.3f;
    [SerializeField] private float shootingDuration = 6f;
    [SerializeField] private float endingPadding = 2f;

    private TutorialGunGrunt[] gunGrunts;

    private bool pressedJump = false;

    protected override void Awake()
    {
        base.Awake();
        Assert.IsNotNull(gunGruntsRoot);

        gunGrunts = gunGruntsRoot.GetComponentsInChildren<TutorialGunGrunt>();
        Assert.IsTrue(gunGrunts.Length == LaneSet.LaneCount);
    }

    private void OnDisable()
    {
        foreach (TutorialGunGrunt gunGrunt in gunGrunts)
            if (gunGrunt != null)
                gunGrunt.gameObject.SetActive(false);
    }

    protected override void StartTutorial()
    {
        SwordPlayerController swordPlayer = SwordPlayerController.Instance;

        if (swordPlayer == null)
        {
            EndTutorial();
            return;
        }

        swordPlayer.jumpEnabled = true;
        swordPlayer.PressedJump += () => { pressedJump = true; };

        float startTime = Time.time;
        IEnumerator Routine()
        {
            while (!pressedJump)
                yield return null;

            foreach (TutorialGunGrunt gunGrunt in gunGrunts)
            {
                gunGrunt.Spawn();
                yield return new WaitForSeconds(spawnDelay);
            }
            yield return new WaitForSeconds(shootingDuration);

            foreach (TutorialGunGrunt gunGrunt in gunGrunts)
                gunGrunt.Despawn();
            yield return new WaitForSeconds(endingPadding);

            EndTutorial();
        }

        StartCoroutine(Routine());
    }
}
