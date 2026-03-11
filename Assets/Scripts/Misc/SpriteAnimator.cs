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
        public float frameLengthOverride = 0f;
    }

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animation[] animations;
    public string idleName;
    public float frameLength = 0.1f;
    private readonly Dictionary<string, Animation> animationMap = new();

    private Coroutine animationRoutine = null;

    private void Awake()
    {
        Assert.IsNotNull(spriteRenderer);

        foreach (Animation animation in animations)
        {
            if (animation.frames.Length > 0)
                animationMap[animation.name] = animation;
        }
    }

    private void Start()
    {
        PlayCycle(idleName);
    }

    public void PlayOneShot(string name)
    {
        if (!animationMap.ContainsKey(name))
        {
            Debug.LogError($"{name} does not exist in animation map");
            return;
        }

        if (animationRoutine != null)
            StopCoroutine(animationRoutine);

        IEnumerator Routine()
        {
            yield return PlayAllFrames(animationMap[name]);
            animationRoutine = null;
            PlayCycle(idleName);
        }

        animationRoutine = StartCoroutine(Routine());
    }

    public void PlayCycle(string name)
    {
        if (!animationMap.ContainsKey(name))
        {
            Debug.LogError($"{name} does not exist in animation map");
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
        WaitForSeconds wait = new(animation.frameLengthOverride > 0f ? animation.frameLengthOverride : frameLength);
        foreach (Sprite frame in animation.frames)
        {
            spriteRenderer.sprite = frame;
            yield return wait;
        }
    }
}
