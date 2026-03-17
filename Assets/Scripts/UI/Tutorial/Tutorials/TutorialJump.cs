using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class TutorialJump : TutorialBase
{
    [SerializeField] private GameObject gunGruntsRoot;
    [SerializeField] private float minDuration = 6f;
    [SerializeField] private float paddingAfterJump = 2f;

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
            foreach (TutorialGunGrunt gunGrunt in gunGrunts)
                gunGrunt.Spawn();

            while (!pressedJump)
                yield return null;

            if (Time.time - startTime < minDuration)
                yield return new WaitForSeconds(minDuration - (Time.time - startTime));

            foreach (TutorialGunGrunt gunGrunt in gunGrunts)
                gunGrunt.Despawn();

            yield return new WaitForSeconds(paddingAfterJump);
            EndTutorial();
        }

        StartCoroutine(Routine());
    }
}
