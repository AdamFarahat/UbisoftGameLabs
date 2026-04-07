using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class HologramText : MonoBehaviour
{
    [SerializeField] private GameObject boxRoot;
    [SerializeField] private float transitionDuration = 0.2f;
    [SerializeField] private float flickerMinDuration = 0.03f;
    [SerializeField] private float flickerMaxDuration = 0.1f;
    [SerializeField] private float paddingDuration = 0.1f;

    private void Awake()
    {
        Assert.IsNotNull(boxRoot);
    }

    public IEnumerator SpawnRoutine()
    {
        // TODO play new tutorial tip ping SFX
        
        gameObject.SetActive(true);
        boxRoot.SetActive(false);
        yield return new WaitForSeconds(paddingDuration);
        yield return FlickerRoutine();
        boxRoot.SetActive(true);
    }

    public IEnumerator DespawnRoutine()
    {
        // TODO play tutorial tip end ping SFX

        boxRoot.SetActive(true);
        yield return FlickerRoutine();
        boxRoot.SetActive(false);
        yield return new WaitForSeconds(paddingDuration);
        gameObject.SetActive(false);
    }

    private IEnumerator FlickerRoutine()
    {
        float nextFlicker = Random.Range(flickerMinDuration, flickerMaxDuration);
        for (float t = 0f; t < transitionDuration; t += Time.deltaTime)
        {
            if (t > nextFlicker)
            {
                boxRoot.SetActive(!boxRoot.activeSelf);
                nextFlicker += Random.Range(flickerMinDuration, flickerMaxDuration);
            }
            yield return null;
        }
    }
}
