using UnityEngine;
using UnityEngine.Assertions;

public class MeleeMovementAI : MonoBehaviour
{
    [SerializeField] private float initialLaneDistance = 300f;

    [SerializeField] private float speed = 1f;

    private LaneBound laneBound;

    private void Awake()
    {
        laneBound = GetComponent<LaneBound>();
        Assert.IsNotNull(laneBound);

        GetComponent<Enemy>().OnTakeFromPool += ResetState;
    }

    private void ResetState()
    {
        laneBound.LaneDistance = initialLaneDistance;
    }

    private void Update()
    {
        laneBound.LaneDistance -= speed * Time.deltaTime;
    }
}
