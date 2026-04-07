using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class TutorialJump : TutorialBase
{
    [SerializeField] private TutorialGunGrunt gunGrunt;
    [SerializeField] private float shootingDuration = 6f;
    [SerializeField] private float endingPadding = 2f;

    private bool pressedJump = false;

    protected override void Awake()
    {
        base.Awake();
        Assert.IsNotNull(gunGrunt);
    }

    private void OnDisable()
    {
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

            gunGrunt.Spawn();
            yield return new WaitForSeconds(shootingDuration);

            gunGrunt.Despawn();
            yield return new WaitForSeconds(endingPadding);

            EndTutorial();
        }

        StartCoroutine(Routine());
    }
}
