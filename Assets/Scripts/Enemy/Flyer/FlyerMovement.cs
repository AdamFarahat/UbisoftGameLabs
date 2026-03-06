using UnityEngine;

public class FlyerMovement : MonoBehaviour
{
    [SerializeField] private float bobAmplitude = 1f;
    [SerializeField] private float bobSpeed = 3f;

    private float age = 0f;
    private float baselineY = 0f;

    private void Start()
    {
        baselineY = transform.position.y;
    }

    private void Update()
    {
        age += Time.deltaTime;
        Vector3 position = transform.position;
        position.y = baselineY + bobAmplitude * Mathf.Sin(bobSpeed * age);
        transform.position = position;
    }
}
