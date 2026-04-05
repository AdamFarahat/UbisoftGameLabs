using UnityEngine;

public class LaneBar : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private float minZ = -100f;
    [SerializeField] private float maxZ = 500f;

    private void Update()
    {
        float z = transform.position.z - speed * Time.deltaTime;
        while (z < minZ)
            z += maxZ - minZ;
        transform.position = new Vector3(transform.position.x, transform.position.y, z);
    }
}
