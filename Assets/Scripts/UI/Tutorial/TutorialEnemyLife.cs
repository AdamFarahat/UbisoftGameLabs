using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

public class TutorialEnemyLife : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    public UnityAction Die;
    private LaneBound lane;
    private Coroutine fadeRoutine;
    private bool dead = false;

    private void Awake()
    {
        Assert.IsNotNull(spriteRenderer);
        
        lane = GetComponent<LaneBound>();
        Assert.IsNotNull(lane);

        this.GetComponentInHierarchy<Enemy>().Die += () => { dead = true; Die?.Invoke(); };
    }

    private void Start()
    {
        Spawn();
    }

    private void OnEnable()
    {
        Spawn();
    }

    private void Spawn()
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        spriteRenderer.color = Color.white;
        fadeRoutine = StartCoroutine(FadeAnimation.FadeInRoutine(spriteRenderer));
        // TODO sfx
    }

    private void Update()
    {
        if (dead)
            return;

        if (lane.LaneDistance < LaneSet.TutorialEnemyDespawnLine)
        {
            dead = true;
            Die?.Invoke();

            IEnumerator Routine()
            {
                yield return FadeAnimation.FadeOutRoutine(spriteRenderer);
                Destroy(gameObject);
            }

            StartCoroutine(Routine());
        }
    }
}
