// Implementation based on Sunny Vale Studio


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CarSpawner : MonoBehaviour
{
    public GameObject[] carPrafabs;

    private void Start() 
    {
        Instantiate(SelectACarPrefab(),transform);
    }

    private GameObject SelectACarPrefab()
    {
        var randomIndex = Random.Range(0, carPrafabs.Length);
        return carPrafabs[randomIndex];
    }
}
