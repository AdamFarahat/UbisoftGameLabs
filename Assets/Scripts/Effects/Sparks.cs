using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class Sparks : MonoBehaviour
{
    [SerializeField] private float maxAngleOffset = 10f;
    [SerializeField] private float maxLinearOffset = 0.3f;

    private SpriteAnimator animator;
    private Billboard billboard;

    private void Awake()
    {
        animator = GetComponentInChildren<SpriteAnimator>();
        Assert.IsNotNull(animator);

        billboard = GetComponentInChildren<Billboard>();
        Assert.IsNotNull(billboard);
    }

    private void Start()
    {
        billboard.rotation += Random.Range(-maxAngleOffset, maxAngleOffset);
        billboard.cameraOffset = Random.insideUnitCircle * maxLinearOffset;

        float duration = animator.GetAnimationDuration("Idle");

        IEnumerator EndRoutine()
        {
            yield return new WaitForSeconds(duration);
            Destroy(gameObject);
        }

        StartCoroutine(EndRoutine());
    }
}
