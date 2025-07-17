using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;

    private List<GameObject> objects = new List<GameObject>();
    private int amount = 150;

    [SerializeField] private GameObject bulletPrefab;
    private void Awake()
    {
        if (instance == null) {
            instance = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < amount; i++) {
            GameObject obj = Instantiate(bulletPrefab);
            obj.SetActive(false);
            objects.Add(obj);
        }
    }

    public GameObject GetPooledObjects()
    {
        for (int i = 0; i < objects.Count; i++) {
            if (!objects[i].activeInHierarchy)
            {
                return objects[i];
            }
        }

        return null;
    }
}
