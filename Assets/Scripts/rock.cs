using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    public float size = 1.0f; // Kaya boyutu
    void Start()
    {
        InitializeRock();
    }



    private void InitializeRock()
    {
        // Kaya boyutunu ayarla
        transform.localScale = Vector3.one * size;

        
    }
}
