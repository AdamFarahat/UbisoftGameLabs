using UnityEngine;
using UnityEngine.Pool;
public interface IPoolable
{
    void SetPool(IObjectPool<GameObject> pool);
}
