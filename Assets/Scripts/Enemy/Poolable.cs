using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public abstract class Poolable : MonoBehaviour
{
    protected IObjectPool<GameObject> enemyPool;
    private Coroutine deathRoutine = null;

    public void SetPool(IObjectPool<GameObject> pool) 
    {
        enemyPool = pool;
    }

    public void Death(IEnumerator animation = null)
    {
        IEnumerator Routine()
        {
            DisableComponents();

            if (animation != null)
                yield return animation;

            if (enemyPool != null)
                enemyPool.Release(gameObject);
            else
                Destroy(gameObject);

            deathRoutine = null;
        }

        if (deathRoutine == null)
            deathRoutine = StartCoroutine(Routine());
    }
    
    public virtual void TakeFromPool()
    {
        EnableComponents();
    }

    private void EnableComponents()
    {
        foreach (var comp in GetComponentsInChildren<MonoBehaviour>())
        {
            if (comp != this)
                comp.enabled = true;
        }
    }

    private void DisableComponents()
    {
        foreach (var comp in GetComponentsInChildren<MonoBehaviour>())
        {
            if (comp != this)
                comp.enabled = false;
        }
    }
}
