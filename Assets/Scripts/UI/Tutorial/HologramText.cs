using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class HologramText : MonoBehaviour
{
    [SerializeField] private GameObject boxRoot;
    [SerializeField] private int numberOfFlickers = 2;
    [SerializeField] private float flickerOnDuration = 0.05f;
    [SerializeField] private float flickerOffDuration = 0.05f;
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

        for (int i = 0; i < numberOfFlickers; i++)
        {
            boxRoot.SetActive(true);
            yield return new WaitForSeconds(flickerOnDuration);
            boxRoot.SetActive(false);
            yield return new WaitForSeconds(flickerOffDuration);
        }

        boxRoot.SetActive(true);
    }

    public IEnumerator DespawnRoutine()
    {
        // TODO play tutorial tip end ping SFX

        boxRoot.SetActive(true);

        for (int i = 0; i < numberOfFlickers; i++)
        {
            boxRoot.SetActive(false);
            yield return new WaitForSeconds(flickerOffDuration);
            boxRoot.SetActive(true);
            yield return new WaitForSeconds(flickerOnDuration);
        }

        boxRoot.SetActive(false);
        yield return new WaitForSeconds(paddingDuration);

        gameObject.SetActive(false);
    }
}
