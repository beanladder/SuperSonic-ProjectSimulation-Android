using UnityEngine;

public class FloorUIManager : MonoBehaviour
{
    public GameObject[] floorUIs;
    public GameObject[] spawnedObjects;
    private string spawnKeyPrefix = "SpawnedObject_";

    void Start()
    {
        for (int i = 0; i < floorUIs.Length; i++)
        {
            string key = spawnKeyPrefix + i;
            if (PlayerPrefs.GetInt(key, 0) == 1)
            {
                SpawnObject(i);
            }
            else
            {
                ShowFloorUI(i);
            }
        }
    }

    public void OnReceiveMoney(int floorIndex)
    {
        // Logic to receive money
        // ...

        // Spawn the object and save the state
        SpawnObject(floorIndex);
        string key = spawnKeyPrefix + floorIndex;
        PlayerPrefs.SetInt(key, 1);
        PlayerPrefs.Save();
    }

    void SpawnObject(int index)
    {
        floorUIs[index].SetActive(false);
        spawnedObjects[index].SetActive(true);
    }

    void ShowFloorUI(int index)
    {
        floorUIs[index].SetActive(true);
        spawnedObjects[index].SetActive(false);
    }
}
