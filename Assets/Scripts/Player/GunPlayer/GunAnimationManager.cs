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

    private SpriteAnimator animator;

    private void Awake()
    {
        animator = GetComponent<SpriteAnimator>();
        Assert.IsNotNull(animator);

        Assert.IsTrue(gunNames.Length == 3);

        LaneBound laneBound = GetComponent<LaneBound>();
        Assert.IsNotNull(laneBound);
        laneBound.DashStart += OnDashStart;
        laneBound.DashEnd += OnDashEnd;
    }

    private void OnDashStart(float deltaLane)
    {
        if (deltaLane > 0f)
            animator.PlayCycle($"{gunNames[gunIndex]} Dash Right");
        else if (deltaLane < 0f)
            animator.PlayCycle("Dash Left");
    }

    private void OnDashEnd()
    {
        animator.PlayDefaultCycle();
    }

    public void PlayIdle()
    {
        animator.defaultName = $"{gunNames[gunIndex]} Idle";
        animator.PlayDefaultCycle();
    }

    public void PlayShoot()
    {
        animator.PlayOneShot($"{gunNames[gunIndex]} Shoot");
    }

    public void StartGrenadeAim()
    {
        animator.PlayCycle("Grenade Aim");
    }

    public void StopGrenadeAim()
    {
        animator.PlayDefaultCycle();
    }

    public void PlayGrenadeThrow()
    {
        animator.PlayOneShot("Grenade Throw");
    }
}
