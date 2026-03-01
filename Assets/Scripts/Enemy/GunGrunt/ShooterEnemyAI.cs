using UnityEngine;

public class ShooterEnemyAI : MonoBehaviour
{
    [SerializeField] private float minLaneDistance = 100f;
    [SerializeField] private float maxLaneDistance = 150f;
    [SerializeField] private float researchCooldown = 3f;

    public int shootingIndex;
    public Transform projSpawnPoint;
    public float ResearchCooldown => researchCooldown;

    private void Start()
    {
        GetComponent<LaneBound>().LaneDistance = Random.Range(minLaneDistance, maxLaneDistance);
    }
}
