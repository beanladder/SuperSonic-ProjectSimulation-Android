using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //public static GameManager instance;
    private HashSet<Shelf.ShelfType> availableShelfTypes = new HashSet<Shelf.ShelfType>();

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