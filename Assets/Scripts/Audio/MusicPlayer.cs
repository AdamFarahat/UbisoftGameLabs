using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    private FlipFlop<AudioSource> audioSource;

    [SerializeField] private List<AudioClip> trackList;
    [SerializeField] private float trackFadeDuration = 1f;
}
