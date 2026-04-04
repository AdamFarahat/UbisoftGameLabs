using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

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
    [SerializeField] private Image image;
    [SerializeField] private Animation[] animations;
    public string defaultName = "Idle";
    private readonly Dictionary<string, Animation> animationMap = new();

    private Coroutine animationRoutine = null;
    private float lastAnimationStartTime = 0f;
    public float LastAnimationStartTime => lastAnimationStartTime;

    private string currentAnimationName;
    private bool isLooping;
    private float pausedTime;

    private int localFrame;
    public int LocalFrame => localFrame;

    private void Awake()
    {
        if (spriteRenderer == null)
            Assert.IsNotNull(image);

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

    private void OnDisable()
    {
        if (animationRoutine == null)
            return;

        StopCoroutine(animationRoutine);
        animationRoutine = null;

        if (!string.IsNullOrEmpty(currentAnimationName))
            pausedTime = Mathf.Repeat(Time.time - lastAnimationStartTime, GetAnimationDuration(currentAnimationName));
    }

    private void OnEnable()
    {
        if (!string.IsNullOrEmpty(currentAnimationName))
        {
            if (isLooping)
                PlayCycle(currentAnimationName, pausedTime);
            else
                PlayOneShot(currentAnimationName, pausedTime);
        }
        else
            PlayDefaultCycle();
    }

    private void SetSprite(Sprite sprite)
    {
        if (spriteRenderer != null)
            spriteRenderer.sprite = sprite;
        if (image != null)
            image.sprite = sprite;
    }

    public float GetAnimationDuration(string name)
    {
        return animationMap[name].duration;
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

    public int GetAnimationFrameCount(string name)
    {
        return animationMap[name].frames.Length;
    }

    public void PlayOneShot(string name, float skipTime = 0f)
    {
        if (!animationMap.ContainsKey(name))
        {
            Debug.LogError($"Animation {name} does not exist in animation map");
            return;
        }

        currentAnimationName = name;
        isLooping = false;

        if (animationRoutine != null)
            StopCoroutine(animationRoutine);

        IEnumerator Routine()
        {
            yield return PlayAllFrames(animationMap[name], skipTime);
            animationRoutine = null;
            PlayDefaultCycle();
        }

        animationRoutine = StartCoroutine(Routine());
    }

    public void PlayDefaultCycle(float skipTime = 0f)
    {
        PlayCycle(defaultName, skipTime);
    }

    public void PlayCycle(string name, float skipTime = 0f)
    {
        if (!animationMap.ContainsKey(name))
        {
            Debug.LogError($"Animation {name} does not exist in animation map");
            return;
        }

        currentAnimationName = name;
        isLooping = true;

        if (animationRoutine != null)
            StopCoroutine(animationRoutine);

        IEnumerator Routine()
        {
            yield return PlayAllFrames(animationMap[name], skipTime);
            while (true)
                yield return PlayAllFrames(animationMap[name], 0f);
        }

        animationRoutine = StartCoroutine(Routine());
    }

    private IEnumerator PlayAllFrames(Animation animation, float skipTime)
    {
        lastAnimationStartTime = Time.time;
        float frameLength = animation.duration / animation.frames.Length;
        WaitForSeconds wait = new(frameLength);
        for (int i = 0; i < animation.frames.Length; i++)
        {
            if (skipTime > 0f)
            {
                if (skipTime >= frameLength)
                {
                    skipTime -= frameLength;
                    SetSprite(animation.frames[i]);
                    localFrame = i;
                    continue;
                }
                else
                {
                    SetSprite(animation.frames[i]);
                    localFrame = i;
                    yield return new WaitForSeconds(frameLength - skipTime);
                }
            }
            else
            {
                SetSprite(animation.frames[i]);
                localFrame = i;
                yield return wait;
            }
        }
    }
}
