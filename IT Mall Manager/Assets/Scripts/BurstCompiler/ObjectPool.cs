using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;
    public GameObject npcPrefab;
    public int poolSize = 40;
    private Queue<GameObject> pool;

    private void Awake()
    {
        instance = this;
        pool = new Queue<GameObject>();
        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject npc = Instantiate(npcPrefab);
            npc.SetActive(false);
            pool.Enqueue(npc);
        }
    }

    public GameObject GetNPC()
    {
        if (pool.Count > 0)
        {
            GameObject npc = pool.Dequeue();
            npc.SetActive(true);
            return npc;
        }
        else
        {
            GameObject npc = Instantiate(npcPrefab);
            npc.SetActive(true);
            return npc;
        }
    }

    public void ReturnNPC(GameObject npc)
    {
        npc.SetActive(false);
        pool.Enqueue(npc);
    }
}