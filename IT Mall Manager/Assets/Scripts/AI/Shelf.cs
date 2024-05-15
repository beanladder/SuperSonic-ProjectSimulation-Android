using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shelf : MonoBehaviour
{

    public static Shelf instance;
    // Enum to represent the different types of shelves
    public enum ShelfType { CPU, RAM, Motherboard }
    public int productCount;

    // Variable to hold the type of this shelf
    public ShelfType shelfType;

    private void Awake() {
        instance = this;
    }
}
