using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private HashSet<Shelf.ShelfType> availableShelfTypes = new HashSet<Shelf.ShelfType>();
    public GameObject objectToEnable; // The GameObject to enable when shelf count exceeds threshold
    public int minShelfCount = 1; // Minimum number of shelves required to enable the GameObject

    private void Start()
    {
        InitializeShelves();
    }

    private void FixedUpdate()
    {
        UpdateAvailableShelves();
    }

    private void InitializeShelves()
    {
        Transform[] allChildren = GetComponentsInChildren<Transform>(true);
        foreach (Transform child in allChildren)
        {
            if (child.CompareTag("CheckShelf"))
            {
                GameObject shelfObj = child.gameObject;
                if (!shelfObj.activeInHierarchy)
                {
                    shelfObj.SetActive(true);
                    UpdateAvailableShelves();
                    shelfObj.SetActive(false);
                }
            }
        }
    }

    public void UpdateAvailableShelves()
    {
        HashSet<Shelf.ShelfType> newAvailableShelfTypes = new HashSet<Shelf.ShelfType>();

        Transform[] allChildren = GetComponentsInChildren<Transform>();
        int activeShelfCount = 0;

        foreach (Transform child in allChildren)
        {
            if (child.CompareTag("CheckShelf"))
            {
                GameObject shelfObj = child.gameObject;
                if (shelfObj.activeInHierarchy)
                {
                    Shelf shelfComponent = shelfObj.GetComponent<Shelf>();
                    if (shelfComponent != null)
                    {
                        newAvailableShelfTypes.Add(shelfComponent.shelfType);
                        activeShelfCount++;
                    }
                }
            }
        }

        if (!newAvailableShelfTypes.SetEquals(availableShelfTypes))
        {
            availableShelfTypes = newAvailableShelfTypes;
            Debug.Log("Available shelves updated.");
            foreach (var shelfType in availableShelfTypes)
            {
                Debug.Log($"Available shelf type: {shelfType}");
            }

            UpdateProductInfo(availableShelfTypes);
        }

        // Enable the GameObject if the active shelf count meets the minimum requirement
        if (objectToEnable != null)
        {
            objectToEnable.SetActive(activeShelfCount >= minShelfCount);
        }
    }

    private void UpdateProductInfo(HashSet<Shelf.ShelfType> availableShelfTypes)
    {
        ProductInfo[] productInfos = FindObjectsOfType<ProductInfo>();
        foreach (var productInfo in productInfos)
        {
            productInfo.FilterProductsByShelfTypes(availableShelfTypes);
        }
    }
}