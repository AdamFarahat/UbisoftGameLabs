using UnityEngine;
using UnityEngine.Assertions;

public class GunAnimationManager : MonoBehaviour
{
    [SerializeField] private string[] gunNames;
    
    private int gunIndex = 0;
    public int GunIndex {
        get => gunIndex;
        set {
            gunIndex = value;
            PlayIdle();
        }
    }

    private string gunIdle = "";

    private SpriteAnimator[] animators;

    private void Awake()
    {
        animators = GetComponentsInChildren<SpriteAnimator>();
        Assert.IsTrue(animators.Length > 0);

        Assert.IsTrue(gunNames.Length == 3);

        LaneBound laneBound = GetComponent<LaneBound>();
        Assert.IsNotNull(laneBound);
        laneBound.DashStart += OnDashStart;
        laneBound.DashEnd += OnDashEnd;
    }

    private void Start()
    {
        PlayIdle();
    }

    private void PlayCycle(string name)
    {
        foreach (var animator in animators)
            animator.PlayCycle(name);
    }

    private void SetDefaultName(string name)
    {
        foreach (var animator in animators)
            animator.defaultName = name;
    }

    private void PlayDefaultCycle()
    {
        foreach (var animator in animators)
            animator.PlayDefaultCycle();
    }

    private void PlayOneShot(string name)
    {
        foreach (var animator in animators)
            animator.PlayOneShot(name);
    }

    private void OnDashStart(float deltaLane)
    {
        if (deltaLane > 0f)
            PlayCycle($"{gunNames[gunIndex]} Dash Right");
        else if (deltaLane < 0f)
            PlayCycle($"Dash Left");
    }

    private void OnDashEnd()
    {
        PlayDefaultCycle();
    }

    public void PlayIdle()
    {
        gunIdle = $"{gunNames[gunIndex]} Idle";
        SetDefaultName(gunIdle);
        PlayDefaultCycle();
    }

    public void PlayShoot()
    {
        PlayOneShot($"{gunNames[gunIndex]} Shoot");
    }

    public void StartGrenadeAim()
    {
        SetDefaultName("Grenade Aim");
        PlayDefaultCycle();
    }

    public void StopGrenadeAim()
    {
        SetDefaultName(gunIdle);
        PlayDefaultCycle();
    }

    public void PlayGrenadeThrow()
    {
        PlayOneShot("Grenade Throw");
    }
}
