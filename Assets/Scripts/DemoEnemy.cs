using UnityEngine;
using UnityEngine.Assertions;

public class DemoEnemy : MonoBehaviour
{
    [SerializeField] private float speed = 1f;

    private LaneBound laneBound;

    private void Awake()
    {
        laneBound = GetComponent<LaneBound>();
        Assert.IsNotNull(laneBound);
    }

    private void Update()
    {
        laneBound.LaneDistance -= speed * Time.deltaTime;
    }

    public void Death()
    {
        //Play Death Animation
        Destroy(gameObject);
    }
}
