using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class TextButtonIconAnimator : MonoBehaviour
{
    [Serializable]
    public class ButtonIconAnimation
    {
        public string[] names;
        public float frameLength = 0.5f;

        private int index = 0;
        private float frameAge = 0f;

        public void Update()
        {
            frameAge += Time.deltaTime;
            if (frameAge >= frameLength)
            {
                index = (index + 1) % names.Length;
                frameAge = 0f;
            }
        }

        public string CurrentName()
        {
            return names[index];
        }
    }

    [SerializeField] private ButtonIconAnimation[] animations;

    private TextMeshProUGUI text;
    private string[] baseTextSplit;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        Assert.IsNotNull(text);
    }

    private void Start()
    {
        MatchCollection matches = Regex.Matches(text.text, @"<sprite name=""(.*?)"">");
        Assert.IsTrue(matches.Count == animations.Length);

        baseTextSplit = new string[animations.Length + 1];
        int baseOffset = 0;
        for (int i = 0; i < animations.Length; i++)
        {
            int index = matches[i].Index;
            int length = matches[i].Length;
            baseTextSplit[i] = text.text[baseOffset..index];
            baseOffset = index + length;
        }
        baseTextSplit[animations.Length] = text.text[baseOffset..];
    }

    private void Init()
    {

    }

    private void Update()
    {
        foreach (var anim in animations)
            anim.Update();

        string newText = baseTextSplit[0];
        for (int i = 0; i < animations.Length; ++i)
        {
            newText += $"<sprite name=\"{animations[i].CurrentName()}\">";
            newText += baseTextSplit[i + 1];
        }
        text.text = newText;
    }
}
