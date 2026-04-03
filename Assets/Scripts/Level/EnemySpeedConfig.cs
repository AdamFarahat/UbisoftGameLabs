using UnityEngine;

public class EnemySpeedConfig : MonoBehaviour
{
    [SerializeField] private float minSpeed = 2f;
    [SerializeField] private float maxSpeed = 8f;

    public float EvaluateSpeed(float difficulty)
    {
        return Mathf.Lerp(minSpeed, maxSpeed, difficulty);
    }
}