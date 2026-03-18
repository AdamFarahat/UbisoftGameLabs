using UnityEngine;
using UnityEngine.Assertions;

public class StunVFX : MonoBehaviour
{
    ParticleSystem ps;
    ParticleSystemRenderer psr;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        Assert.IsNotNull(ps);

        psr = ps.GetComponent<ParticleSystemRenderer>();
        Assert.IsNotNull(psr);
    }

    private void Update()
    {
        psr.material.color = Color.white;
    }
}
