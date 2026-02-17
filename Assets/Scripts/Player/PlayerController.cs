using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    protected PlayerInput playerInput;
    protected LaneBound laneBound;
    protected Rigidbody rb;
    protected Collider playerCollider;

    [SerializeField] private float switchLaneDuration = 0.1f;
    private bool switchingLanes = false;

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
        if (ctx.performed && laneBound.LaneIndex > 0)
        {
            if (!switchingLanes)
                StartCoroutine(SwitchLanes(laneBound.LaneIndex - 1));
            // TODO else dash
        }
    }

    private void OnMoveRight(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && laneBound.LaneIndex < LaneConfigSO.Instance.GetNumberOfLanes() - 1)
        {
            if (!switchingLanes)
                StartCoroutine(SwitchLanes(laneBound.LaneIndex + 1));
            // TODO else dash
        }
    }

    private IEnumerator SwitchLanes(float toIndex)
    {
        switchingLanes = true;
        float fromIndex = laneBound.LaneIndex;
        for (float t = 0f; t < switchLaneDuration; t += Time.deltaTime)
        {
            float a = Mathf.Clamp01(t / switchLaneDuration);
            laneBound.LaneIndex = Mathf.Lerp(fromIndex, toIndex, a);
            yield return null;
        }
        laneBound.LaneIndex = toIndex;
        switchingLanes = false;
    }
}
