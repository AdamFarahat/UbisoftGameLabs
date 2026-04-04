using System.Collections;
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
    [SerializeField] private float stunTime;
    [SerializeField] private float shakeInterval = 0.1f;
    [SerializeField] private float shakeOffset = 0.025f;
    private enum SamuraiState { Walking, Slashing, Parrying, Stunned};
    private SamuraiState state;
    private float time = 0f;
    private BulletDetector bulletDetector;
    private Bullet parriedBullet;
    private int numberOfSlashesDone;
    private PlayerStats playerStats;
    private bool canSlash = true;
    protected LaneBound lane;
    private Coroutine stunRoutine;
    private Billboard spriteBillboard;

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
                if (parriedBullet)
                {

                    parriedBullet.Parry(null, ParryMultipliyer, Bullet.ProjectileState.ParriedByEnemy);
                    parriedBullet = null;
                }
                //TODO: SlashingAnimation

                HealthCollider.enabled = true;
                state = SamuraiState.Walking;
                break;
            case SamuraiState.Slashing:
                if (canSlash)
                {
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

                }
                if (time > SlashInterval)
                {
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
        return Physics.Raycast(b.transform.position, b.transform.forward, out RaycastHit hit) && hit.collider == HealthCollider;
    }

    private void WalkForward()
    {
        lane.LaneDistance -= WalkingSpeed * Time.deltaTime;
    }

    private bool IsInSlashingRangeRange()
    {
        return PlayerController.AnyPlayerInLane(lane.LaneIndex) && lane.LaneDistance <= SlashDistance;
    }

    public void OnSwordHitBoxTriggerEnter(Collider collider)
    {
        if (collider.TryGetComponent(out GunPlayerController gunPlayer)) {
            //TODO: Stun or take damage
            gunPlayer.Stun(StunTime);
        }

        if (collider.TryGetComponent(out SwordPlayerController swordPlayer))
        {
            if (swordPlayer.TryBlock())
            {

                // TODO stun sfx
                IEnumerator Routine()
                {
                    AudioManager.Instance.PlayOneShot(FMODEvents.Instance.PlayerStunned, transform.position);
                    
                    //TODO: animation 

                    //Vector3 initialCameraOffset = spriteBillboard.cameraOffset;
                    int shakeCounter = 0;
                    for (float t = 0f; t < stunTime; t += Time.deltaTime)
                    {
                        Debug.Log("Routine of stun:" + t);
                        /*if (t > shakeCounter * shakeInterval)
                        {
                            shakeCounter = Mathf.CeilToInt(t / shakeInterval);
                            Vector3 cameraOffset = initialCameraOffset;
                            Vector2 shake = UnityEngine.Random.insideUnitCircle * shakeOffset;
                            cameraOffset.x += shake.x;
                            cameraOffset.y += shake.y;
                            //spriteBillboard.cameraOffset = cameraOffset;
                        }*/

                        yield return null;
                    }
                    //spriteBillboard.cameraOffset = initialCameraOffset;

                    // TODO: animation for coming back

                    stunRoutine = null;
                    state = SamuraiState.Walking;
                }

                state = SamuraiState.Stunned;
                stunRoutine = StartCoroutine(Routine());
            }
            else { 
                swordPlayer.Stun(StunTime);
            } 
        }
    }
}
