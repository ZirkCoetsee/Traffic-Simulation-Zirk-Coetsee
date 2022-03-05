// Implementation based on Sunny Vale Studio


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIDirector : MonoBehaviour
{
    public PlacementManager placementManager;
    public GameObject[] pedestrianPrefabs;

    public GameObject carPrefab;

    AdjacencyGraph graph = new AdjacencyGraph();

    public void SpawnAllAgents()
    {
        Debug.Log("You clicked spawn a agent");

        // Spawn agents from all houses, and special structures and give them different location types
        foreach (var house in placementManager.GetAllHouses())
        {
            Debug.Log("Foreach House");
            TrySpawningAnAgent(house,placementManager.GetRandomSpecialStructure());
        }
        foreach (var specialStructures in placementManager.GetAllSpecialStructures())
        {
            Debug.Log("Foreach Special");
            TrySpawningAnAgent(specialStructures,placementManager.GetRandomHouseStructure());
        }
    }

    private void TrySpawningAnAgent(StructureModel startStructure, StructureModel endStructure)
    {
        // Check if the start structure is not null and end structure is not null
        if(startStructure != null && endStructure != null)
        {
            Debug.Log("Try spawing, objects are not null");
            var startPosition = ((IRequireRoad)startStructure).RoadPosition;
            var endPosition =   ((IRequireRoad)endStructure).RoadPosition;
            var agent = Instantiate(GetRandomPedestrian(),startPosition, Quaternion.identity);
            var path = placementManager.GetPathBetween(startPosition,endPosition,true);
            if(path.Count > 0)
            {
                path.Reverse();
                var aiAgent = agent.GetComponent<AIAgent>();
                aiAgent.Initialize(new List<Vector3>(path.Select(x => (Vector3)x).ToList()));
            }
        }
    }

    private GameObject GetRandomPedestrian()
    {
        return pedestrianPrefabs[UnityEngine.Random.Range(0, pedestrianPrefabs.Length)];
    }

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
            var startRoadPosition = ((IRequireRoad)startStructure).RoadPosition;
            var endRoadPosition = ((IRequireRoad)endStructure).RoadPosition;

            var path = placementManager.GetPathBetween(startRoadPosition,endRoadPosition);
            var car = Instantiate(carPrefab,startRoadPosition, Quaternion.identity);
            // Cast position to vector 3
            car.GetComponent<CarAI>().SetPath( path.ConvertAll(x => (Vector3)x));

        }
    }
}
