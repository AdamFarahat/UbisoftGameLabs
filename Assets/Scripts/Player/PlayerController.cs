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

    protected virtual void OnEnable()
    {
        playerInput.actions.Enable();
        playerInput.actions["MoveLeft"].performed += OnMoveLeft;
        playerInput.actions["MoveRight"].performed += OnMoveRight;
        playerInput.actions["Start"].performed += OnStartButtonPressed;
    }

    protected virtual void OnDisable()
    {
        playerInput.actions.Disable();
        playerInput.actions["MoveLeft"].performed -= OnMoveLeft;
        playerInput.actions["MoveRight"].performed -= OnMoveRight;
        playerInput.actions["Start"].performed -= OnStartButtonPressed;
    }

    private void OnStartButtonPressed(InputAction.CallbackContext ctx)
    {
        StartButtonPressed?.Invoke();
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

    public float GetLaneIndex()
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

    public float GetDiscreteMultiplier()
    {
        return discreteMultipliers[discreteMultiplierIndex];
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

    [ContextMenu("Test Multiplier")]
    public void TestMultiplier()
    {
        OnDiscreteMultiplierChange?.Invoke();
    }
}
