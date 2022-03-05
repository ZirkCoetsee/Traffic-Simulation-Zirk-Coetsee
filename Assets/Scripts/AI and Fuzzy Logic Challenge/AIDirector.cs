// Implementation based on Sunny Vale Studio


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDirector : MonoBehaviour
{
    public PlacementManager placementManager;
    public GameObject[] pedestrianPrefabs;

    public GameObject carPrefab;

    AdjacencyGraph graph = new AdjacencyGraph();

    public void SpawnACar()
    {
        Debug.Log("You clicked spawn a car");
        // Assign method to a button to spawn cars at all houses
        foreach (var house in placementManager.GetAllHouses())
        {
            Debug.Log(house.gameObject.name);
            TrySpawningACar(house, placementManager.GetRandomSpecialStructure());
        }
    }

    private void TrySpawningACar(StructureModel startStructure, StructureModel endStructure)
    {
        // Check if they exist
        if (startStructure != null && endStructure != null)
        {
            // Structures require roads
            var startRoadPosition = ((INeedingRoad)startStructure).RoadPosition;
            var endRoadPosition = ((INeedingRoad)endStructure).RoadPosition;

            var path = placementManager.GetPathBetween(startRoadPosition,endRoadPosition);
            var car = Instantiate(carPrefab,startRoadPosition, Quaternion.identity);
            // Cast position to vector 3
            car.GetComponent<CarAI>().SetPath( path.ConvertAll(x => (Vector3)x));

        }
    }
}
