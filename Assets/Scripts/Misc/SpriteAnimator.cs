using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class SpriteAnimator : MonoBehaviour
{
    [Serializable]
    public class Animation
    {
        public string name;
        public Sprite[] frames;
        public float duration;
    }

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animation[] animations;
    public string defaultName = "Idle";
    private readonly Dictionary<string, Animation> animationMap = new();

    private Coroutine animationRoutine = null;

    private void Awake()
    {
        Assert.IsNotNull(spriteRenderer);

        foreach (Animation animation in animations)
        {
            if (animation.frames.Length == 0)
            {
                Debug.LogError($"Animation {animation.name} has no frames");
                continue;
            }

            if (animation.duration <= 0f)
            {
                Debug.LogError($"Animation {animation.name} has no duration");
                continue;
            }

            animationMap[animation.name] = animation;
        }
    }

    private void Start()
    {
        PlayDefaultCycle();
    }

    public void SetAnimationDuration(string name, float duration)
    {
        if (animationMap.TryGetValue(name, out Animation animation))
        {
            if (duration > 0f)
                animation.duration = duration;
            else
                Debug.LogError($"Duration {duration} must be greater than 0");
        }
        else
            Debug.LogError($"Animation {name} does not exist in animation map");
    }

    public void PlayOneShot(string name)
    {
        if (!animationMap.ContainsKey(name))
        {
            Debug.LogError($"Animation {name} does not exist in animation map");
            return;
        }

        if (animationRoutine != null)
            StopCoroutine(animationRoutine);

        IEnumerator Routine()
        {
            yield return PlayAllFrames(animationMap[name]);
            animationRoutine = null;
            PlayCycle(defaultName);
        }

        animationRoutine = StartCoroutine(Routine());
    }

    public void PlayDefaultCycle()
    {
        PlayCycle(defaultName);
    }

    public void PlayCycle(string name)
    {
        if (!animationMap.ContainsKey(name))
        {
            Debug.LogError($"Animation {name} does not exist in animation map");
            return;
        }

        if (animationRoutine != null)
            StopCoroutine(animationRoutine);

        IEnumerator Routine()
        {
            while (true)
                yield return PlayAllFrames(animationMap[name]);
        }

        animationRoutine = StartCoroutine(Routine());
    }

    private IEnumerator PlayAllFrames(Animation animation)
    {
        WaitForSeconds wait = new(animation.duration / animation.frames.Length);
        foreach (Sprite frame in animation.frames)
        {
            spriteRenderer.sprite = frame;
            yield return wait;
        }
    }
}
