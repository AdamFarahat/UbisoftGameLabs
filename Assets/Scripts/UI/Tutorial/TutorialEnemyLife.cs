using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

public class TutorialEnemyLife : MonoBehaviour
{
    public float fadeInDuration = 0.1f;
    public UnityAction Die;
    private LaneBound lane;
    private bool dead = false;
    public bool Dead => dead;

    private SpriteRenderer[] spriteRenderers;
    private Coroutine[] fadeRoutines;

    private void Awake()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        Assert.IsTrue(spriteRenderers.Length > 0);
        fadeRoutines = new Coroutine[spriteRenderers.Length];

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
        int i = 0;
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            if (fadeRoutines[i] != null)
                StopCoroutine(fadeRoutines[i]);

            spriteRenderer.color = Color.white;
            fadeRoutines[i] = StartCoroutine(FadeAnimation.FadeInRoutine(spriteRenderer, fadeInDuration));
            i++;
        }

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
                int animsLeft = spriteRenderers.Length;

                IEnumerator Animate(SpriteRenderer spriteRenderer)
                {
                    yield return FadeAnimation.FadeOutRoutine(spriteRenderer);
                    animsLeft--;
                }

                foreach (SpriteRenderer spriteRenderer in spriteRenderers)
                    StartCoroutine(Animate(spriteRenderer));

                yield return new WaitUntil(() => animsLeft == 0);
                Destroy(gameObject);
            }

            StartCoroutine(Routine());
        }
    }
}
