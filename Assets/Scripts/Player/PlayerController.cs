using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public abstract class PlayerController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private float switchLaneBufferDuration = 0.1f;
    private Coroutine switchLaneBufferRoutine = null;

    private Coroutine stunRoutine = null;
    public bool Stunned => stunRoutine != null;

    private int score = 0;
    private float continuousMultiplier = 1f;
    [Header("Score")]
    [SerializeField] private int discreteMultiplierIndex = 0;
    [SerializeField] private List<float> discreteMultipliers = new() { 1f, 2f, 4f, 6f, 8f };

    [Header("Stun")]
    [SerializeField] private ParticleSystem stunParticleSystem;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Billboard spriteBillboard;
    [SerializeField] private float shakeOffset = 0.1f;
    [SerializeField] private float shakeInterval = 0.02f;

    public int Score => score;
    public UnityEvent OnDiscreteMultiplierChange;

    protected PlayerInput playerInput;
    protected LaneBound laneBound;
    protected Rigidbody rb;
    protected Collider playerCollider;
    protected PlayerStats playerStats;

    public UnityAction StartButtonPressed;
    public UnityAction SelectButtonPressed;

    private float timePressedA = -100f;
    public float TimePressedA => timePressedA;
    private float timePressedB = -100f;
    public float TimePressedB => timePressedB;
    private bool inputBlockedBySuper = false;
    protected bool InputBlockedBySuper => inputBlockedBySuper;

    // Begin tutorial settings
    public bool moveEnabled = true;
    // End tutorial settings

    protected virtual void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        Assert.IsNotNull(playerInput);
        laneBound = GetComponent<LaneBound>();
        Assert.IsNotNull(laneBound);
        rb = GetComponentInChildren<Rigidbody>();
        Assert.IsNotNull(rb);
        playerCollider = GetComponentInChildren<Collider>();
        Assert.IsNotNull(playerCollider);
        playerStats = FindFirstObjectByType<PlayerStats>();
        Assert.IsNotNull(playerStats);

        Assert.IsTrue(discreteMultipliers.Count > 1);
        discreteMultipliers.Sort();

        Assert.IsNotNull(stunParticleSystem);
        Assert.IsNotNull(spriteRenderer);
        Assert.IsNotNull(spriteBillboard);
    }

    protected virtual void Start()
    {
        playerInput.actions.Enable();
        stunParticleSystem.Pause();
        stunParticleSystem.gameObject.SetActive(false);
    }

    protected virtual void Update()
    {
        if (inputBlockedBySuper)
            inputBlockedBySuper = false;
    }

    protected virtual void OnEnable()
    {
        playerInput.actions.Enable();
        playerInput.actions["MoveLeft"].performed += OnMoveLeft;
        playerInput.actions["MoveRight"].performed += OnMoveRight;
        playerInput.actions["Start"].performed += OnStartButtonPressed;
        playerInput.actions["Select"].performed += OnSelectButtonPressed;
    }

    protected virtual void OnDisable()
    {
        playerInput.actions.Disable();
        playerInput.actions["MoveLeft"].performed -= OnMoveLeft;
        playerInput.actions["MoveRight"].performed -= OnMoveRight;
        playerInput.actions["Start"].performed -= OnStartButtonPressed;
        playerInput.actions["Select"].performed -= OnSelectButtonPressed;
    }

    private void OnStartButtonPressed(InputAction.CallbackContext ctx)
    {
        StartButtonPressed?.Invoke();
    }

    private void OnSelectButtonPressed(InputAction.CallbackContext ctx)
    {
        SelectButtonPressed?.Invoke();
    }

    private void OnMoveLeft(InputAction.CallbackContext ctx)
    {
        if (moveEnabled)
            MoveToLane((int index) => { return index - 1; });
    }

    private void OnMoveRight(InputAction.CallbackContext ctx)
    {
        if (moveEnabled)
            MoveToLane((int index) => { return index + 1; });
    }

    private void MoveToLane(Func<int, int> laneFn)
    {
        if (Stunned)
            return;

        void DoMove()
        {
            int lane = laneFn(laneBound.LaneIndex);
            if (lane >= 0 && lane < LaneSet.LaneCount)
                laneBound.MoveToLane(lane);
        }

        float buffer = laneBound.SwitchLaneDurationLeft();
        if (buffer == 0f)
            DoMove();
        else if (buffer < switchLaneBufferDuration && switchLaneBufferRoutine == null)
        {
            IEnumerator Routine()
            {
                yield return new WaitForSeconds(buffer);
                DoMove();
                switchLaneBufferRoutine = null;
            }
            switchLaneBufferRoutine = StartCoroutine(Routine());
        }
    }

    public int GetLaneIndex()
    {
        return laneBound.LaneIndex;
    }

    public float GetLaneDistance()
    {
        return laneBound.LaneDistance;
    }

    public static bool AnyPlayerInLane(int laneIndex)
    {
        if (GunPlayerController.Instance != null && GunPlayerController.LaneIndex == laneIndex)
            return true;
        if (SwordPlayerController.Instance != null && SwordPlayerController.LaneIndex == laneIndex)
            return true;
        return false;
    }

    public virtual float GetCooldownPercent()
    {
        throw new NotImplementedException();
    }

    public void Stun(float stunTime)
    {
        if (Stunned || PlayerStats.Instance.IsSuperActive())
            return;

        SetContinuousMultiplier(1f);

        // TODO stun sfx
        IEnumerator Routine()
        {
            spriteRenderer.color = Color.black;
            stunParticleSystem.gameObject.SetActive(true);
            stunParticleSystem.Play();

            Vector3 initialCameraOffset = spriteBillboard.cameraOffset;
            int shakeCounter = 0;
            for (float t = 0f; t < stunTime; t += Time.deltaTime)
            {
                if (t > shakeCounter * shakeInterval)
                {
                    shakeCounter = Mathf.CeilToInt(t / shakeInterval);
                    Vector3 cameraOffset = initialCameraOffset;
                    Vector2 shake = UnityEngine.Random.insideUnitCircle * shakeOffset;
                    cameraOffset.x += shake.x;
                    cameraOffset.y += shake.y;
                    spriteBillboard.cameraOffset = cameraOffset;
                }

                yield return null;
            }
            spriteBillboard.cameraOffset = initialCameraOffset;

            stunParticleSystem.Pause();
            stunParticleSystem.gameObject.SetActive(false);
            spriteRenderer.color = Color.white;

            stunRoutine = null;
            OnStunEnd();
        }

        OnStunStart();
        stunRoutine = StartCoroutine(Routine());
    }

    protected virtual void OnStunStart()
    {
    }

    protected virtual void OnStunEnd()
    {
    }

    public void ResetScore()
    {
        score = 0;
    }

    public void AddScore(int score)
    {
        this.score += Mathf.CeilToInt(score * GetDiscreteMultiplier());
    }

    public float GetBaseDiscreteMultiplier()
    {
        return discreteMultipliers[discreteMultiplierIndex];
    }

    // If super is active, discrete multiplier is the sum of the player's base discrete multiplier and the other player's base discrete multiplier
    public float GetDiscreteMultiplier()
    {
        if (PlayerStats.Instance != null && PlayerStats.Instance.IsSuperActive())
        {
            float gunMult = GunPlayerController.Instance != null ? GunPlayerController.Instance.GetBaseDiscreteMultiplier() : 1f;
            float swordMult = SwordPlayerController.Instance != null ? SwordPlayerController.Instance.GetBaseDiscreteMultiplier() : 1f;
            
            return gunMult + swordMult; 
        }

        return GetBaseDiscreteMultiplier();
    }

    public float GetNormalizedMultiplier()
    {
        return discreteMultiplierIndex / ((float)discreteMultipliers.Count - 1);
    }

    public void SetContinuousMultiplier(float multiplier)
    {
        continuousMultiplier = Mathf.Clamp(multiplier, 1f, discreteMultipliers.Last());

        // Find largest index for which the continuous multiplier is greater or equal to the discrete multiplier
        int index = discreteMultipliers.BinarySearch(continuousMultiplier);
        if (index < 0)
            index = ~index - 1;
        if (index < 0)
            index = 0;
        if (index != discreteMultiplierIndex)
        {
            discreteMultiplierIndex = index;
            OnDiscreteMultiplierChange?.Invoke();
        }
    }

    public void AddContinuousMultiplier(float deltaMultiplier)
    {
        SetContinuousMultiplier(continuousMultiplier + deltaMultiplier);
    }

    protected void SuperInitiatedA(InputAction.CallbackContext _)
    {
        if (Stunned)
            return;

        timePressedA = Time.time;
        if (PlayerStats.Instance.TryActivatingSuper())
            inputBlockedBySuper = true;
    }

    protected void SuperInitiatedB(InputAction.CallbackContext _)
    {
        if (Stunned)
            return;

        timePressedB = Time.time;
        if (PlayerStats.Instance.TryActivatingSuper())
            inputBlockedBySuper = true;
    }

    [ContextMenu("Test Multiplier")]
    public void TestMultiplier()
    {
        OnDiscreteMultiplierChange?.Invoke();
    }

    // For testing purposes only, allows manually setting the discrete multiplier index in the inspector and firing the change event
    private void OnValidate()
    {
        if (Application.isPlaying && discreteMultipliers != null && discreteMultipliers.Count > 0)
        {
            discreteMultiplierIndex = Mathf.Clamp(discreteMultiplierIndex, 0, discreteMultipliers.Count - 1);
            
            OnDiscreteMultiplierChange?.Invoke();
        }
    }
}
