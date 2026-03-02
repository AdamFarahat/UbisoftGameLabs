using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public abstract class PlayerController : MonoBehaviour
{
    public float score = 0;
    public float multiplier = 1f;
    protected PlayerInput playerInput;
    protected LaneBound laneBound;
    protected Rigidbody rb;
    protected Collider playerCollider;
    
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
        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        playerInput.actions.Enable();
        playerInput.actions["MoveLeft"].performed += OnMoveLeft;
        playerInput.actions["MoveRight"].performed += OnMoveRight;
    }

    private void OnMoveLeft(InputAction.CallbackContext ctx)
    {
        if (laneBound.LaneIndex > 0)
            laneBound.MoveToLane(laneBound.LaneIndex - 1);
    }

    private void OnMoveRight(InputAction.CallbackContext ctx)
    {
        if (laneBound.LaneIndex < LaneConfigSO.Instance.GetNumberOfLanes() - 1)
            laneBound.MoveToLane(laneBound.LaneIndex + 1);
    }

    public virtual float GetCooldownPercent()
    {
        throw new NotImplementedException();
    }

    public float GetLaneIndex() {
        return laneBound.LaneIndex;
    }
    public virtual void UpdateScore(float multiplierGain, int scoreOfEnemy) { 
        throw new NotImplementedException();
    }
}
