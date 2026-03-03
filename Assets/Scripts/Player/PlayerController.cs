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
    private Coroutine stunRoutine = null;
    public bool Stunned => stunRoutine != null;

    private int score = 0;
    private float continuousMultiplier = 1f;
    private int discreteMultiplierIndex = 0;
    [SerializeField] private List<float> discreteMultipliers = new() { 1f, 2f, 4f, 6f, 8f };

    public int Score => score;
    public UnityEvent OnDiscreteMultiplierChange;

    protected PlayerInput playerInput;
    protected LaneBound laneBound;
    protected Rigidbody rb;
    protected Collider playerCollider;
    protected PlayerStats playerStats;

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
    }

    protected virtual void Start()
    {
        playerInput.actions.Enable();
        playerInput.actions["MoveLeft"].performed += OnMoveLeft;
        playerInput.actions["MoveRight"].performed += OnMoveRight;
    }

    private void OnMoveLeft(InputAction.CallbackContext ctx)
    {
        if (Stunned)
            return;

        if (laneBound.LaneIndex > 0)
            laneBound.MoveToLane(laneBound.LaneIndex - 1);
    }

    private void OnMoveRight(InputAction.CallbackContext ctx)
    {
        if (Stunned)
            return;

        if (laneBound.LaneIndex < LaneConfigSO.Instance.GetNumberOfLanes() - 1)
            laneBound.MoveToLane(laneBound.LaneIndex + 1);
    }

    public float GetLaneIndex()
    {
        return laneBound.LaneIndex;
    }

    public virtual float GetCooldownPercent()
    {
        throw new NotImplementedException();
    }

    public void Stun(float stunTime)
    {
        if (Stunned)
            return;

        SetContinuousMultiplier(1f);

        MeshRenderer debugMesh = GetComponentInChildren<MeshRenderer>(); // TODO remove once sprites are used for both players -> execute stun animation instead.
        if (debugMesh != null)
            debugMesh.material.color = Color.yellow;

        IEnumerator Routine()
        {
            yield return new WaitForSeconds(stunTime);
            if (debugMesh != null)
                debugMesh.material.color = Color.white;

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
}
