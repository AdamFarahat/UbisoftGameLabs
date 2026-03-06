using UnityEngine;
using UnityEngine.Assertions;

public class Bobber : MonoBehaviour
{
    [SerializeField] private float bobAmplitude = 0.2f;
    [SerializeField] private float bobSpeed = 3f;
    [SerializeField] private Transform bobber;

    private float age = 0f;

    private void Awake()
    {
        Assert.IsNotNull(bobber);
        if (TryGetComponent(out Enemy enemy))
            enemy.OnTakeFromPool += ResetState;
    }

    private void ResetState()
    {
        age = 0f;
    }

    private void Update()
    {
        age += Time.deltaTime;
        Vector3 position = bobber.localPosition;
        position.y = bobAmplitude * Mathf.Sin(bobSpeed * age);
        bobber.localPosition = position;
    }
}
