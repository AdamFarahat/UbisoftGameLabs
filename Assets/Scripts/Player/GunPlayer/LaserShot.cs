using System.Collections;
using UnityEngine;

class LaserShot : MonoBehaviour
{
    [SerializeField] private float duration = 0.4f;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void Fire()
    {
        if (gameObject.activeSelf)
            return;

        IEnumerator Routine()
        {
            yield return new WaitForSeconds(duration);
            gameObject.SetActive(false);
        }

        gameObject.SetActive(true);
        StartCoroutine(Routine());
    }
}
