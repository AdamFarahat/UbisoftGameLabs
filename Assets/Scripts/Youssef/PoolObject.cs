using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Rendering;

public class PoolObject : MonoBehaviour
{
    public static PoolObject SharedInstance;
    public List<GameObject> pooledObjects;
    public GameObject objectToPool;
    public int amountToPool = 20;

    void Awake()
    {
        SharedInstance = this;
    }

    void Start()
    {
        pooledObjects = new List<GameObject>();
        GameObject tmp;
        for (int i = 0; i < amountToPool; i++)
        {
            tmp = Instantiate(objectToPool);
            tmp.SetActive(false);
            pooledObjects.Add(tmp);
        }

    }
    public GameObject Spawn(Vector3 position, Quaternion rot)
    {
        GameObject tmp = null;
        for (int i = 0; i < amountToPool; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                tmp = pooledObjects[i];
            }
        }
        if (tmp == null) {
            tmp = Instantiate(objectToPool);
            pooledObjects.Add(tmp);
            amountToPool++;
        }
        tmp.transform.position = position;
        tmp.transform.rotation = rot;
        tmp.SetActive(true);
        return tmp;
    }

}
