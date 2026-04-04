using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Assertions;

public class SamuraiEnemy : MonoBehaviour
{
    [SerializeField] private Collider HealthCollider;
    [SerializeField] private float ProjectileTresholdSpeed = 400f;
    [SerializeField] private float WalkingSpeed = 10f;
    [SerializeField] private float CheckIfCanSlashInterval = 1f;
    [SerializeField] private float SlashInterval = 1.5f;
    [SerializeField] private float StunTime = 1f;
    [SerializeField] private int NumberOfSlashes = 2;
    [SerializeField] private float SlashDistance = 10f;
    [SerializeField] private EnemySwordHitbox swordHitBox;
    [SerializeField] private float ParryMultipliyer = 1.1f;

    private enum SamuraiState { Walking, Slashing, Parrying};
    private SamuraiState state;
    private float time = 0f;
    private BulletDetector bulletDetector;
    private Bullet parriedBullet;
    private int numberOfSlashesDone;
    private PlayerStats playerStats;
    private bool canSlash = true;
    protected LaneBound lane;
    
    protected void Awake()
    {
        lane = GetComponent<LaneBound>();
        playerStats = FindFirstObjectByType<PlayerStats>();
        bulletDetector = GetComponentInChildren<BulletDetector>();
        Assert.IsNotNull(bulletDetector);
        Assert.IsNotNull(playerStats);
        Assert.IsNotNull(lane);
        Assert.IsNotNull(swordHitBox);
        GetComponent<Enemy>().OnTakeFromPool += ResetState;

    }

    private void ResetState()
    {
        state = SamuraiState.Walking;
        canSlash = true;
        numberOfSlashesDone = 0;
        parriedBullet = null;
        lane.LaneDistance = LaneSet.SpawnLine;
        time = 0f;
    }

    private void Start()
    {
        state = SamuraiState.Walking;
    }

    private void Update()
    {
        // TODO cowboy can only dodge when walking. If shooting, change line to dodge then change back.
        time += Time.deltaTime;
        switch (state)
        {
            case SamuraiState.Walking:
                if (BulletComingInRange())
                {
                    HealthCollider.enabled = false;
                    state = SamuraiState.Parrying;
                    time = 0f;
                }
                else if (time >= CheckIfCanSlashInterval && IsInSlashingRangeRange() && !(numberOfSlashesDone > NumberOfSlashes))
                {
                    state = SamuraiState.Slashing;
                    time = 0f;
                }
                WalkForward();
                break;
            case SamuraiState.Parrying:
                if (parriedBullet) {
                    
                    parriedBullet.Parry(null, ParryMultipliyer, Bullet.ProjectileState.ParriedByEnemy);                    
                    parriedBullet = null;
                }
                //TODO: SlashingAnimation
               

                HealthCollider.enabled = true;
                state = SamuraiState.Walking;
                break;
            case SamuraiState.Slashing:
                if (canSlash) {
                    canSlash = false;
                    swordHitBox.gameObject.SetActive(true);
                    //TODO: Play Slashing Animation
                    if (++numberOfSlashesDone > NumberOfSlashes)
                    {

                        time = 0f;
                        state = SamuraiState.Walking;
                        swordHitBox.gameObject.SetActive(false);
                        canSlash = true;
                    }
                    
                } if (time > SlashInterval) {
                    time = 0;
                    canSlash = true;
                    swordHitBox.gameObject.SetActive(false);  
                }
                break;
        }
    }

    private bool BulletComingInRange()
    {
        foreach (Bullet b in bulletDetector.NearbyBullets)
        {
            if (!b.enabled || b.State != Bullet.ProjectileState.ShotByPlayer)
            {
                continue;
            }

            if (IsPredictedToHit(b) && b.Speed <= ProjectileTresholdSpeed)
            {
                parriedBullet = b;
                return true;
            }
        }

        return false;
    }

    private bool IsPredictedToHit(Bullet b)
    {
        RaycastHit hit;
        if (Physics.Raycast(b.transform.position, b.transform.forward, out hit))
        {
            if (hit.collider == HealthCollider)
            {
                return true;
            }
        }

        return false;
    }

    

    private void WalkForward()
    {
        lane.LaneDistance -= WalkingSpeed * Time.deltaTime;
    }

    private bool IsInSlashingRangeRange()
    {
        return PlayerController.AnyPlayerInLane(lane.LaneIndex) && lane.LaneDistance <= SlashDistance
            && lane.LaneDistance <= LaneSet.VisibleEndLine;
    }

    public void OnSwordHitBoxTriggerEnter(Collider collider)
    {
        PlayerController player = collider.GetComponent<PlayerController>();
        
        if (player) {
            //TODO: Stun or take damage
            player.Stun(StunTime);
        } 
    }
}
