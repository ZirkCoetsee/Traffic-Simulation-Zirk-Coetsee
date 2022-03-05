// Implementation based on Sunny Vale Studio


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StructureManager : MonoBehaviour
{
    public StructurePrefabWeighted [] housePrefabs, specialPrefabs, bigStructurePrefabs;
    public PlacementManager placementManager;

    private float[] houseWeights, specialWeights, bigStructureWeights;

    private void Start() 
    {
        houseWeights = housePrefabs.Select(prefabStats => prefabStats.weight).ToArray();
        specialWeights = specialPrefabs.Select(prefabStats => prefabStats.weight).ToArray();
        bigStructureWeights = bigStructurePrefabs.Select(prefabStats => prefabStats.weight).ToArray();

    }

    public void PlaceHouse(Vector3Int position)
    {
        if(CheckPositionBeforePlacement(position))
        {
            int randomIndex = GetRandomWeightedIndex(houseWeights);
            placementManager.PlaceObjectOnTheMap(position,housePrefabs[randomIndex].prefab,CellType.Structure);
            // Play placement audio
        }
    }

    public void PlaceSpecial(Vector3Int position)
    {
        if(CheckPositionBeforePlacement(position))
        {
            int randomIndex = GetRandomWeightedIndex(specialWeights);
            placementManager.PlaceObjectOnTheMap(position,specialPrefabs[randomIndex].prefab,CellType.Structure);
            // Play placement audio
        }
    }

    internal void PLaceBigStructure(Vector3Int position)
    {
        // Dimensions of big structure
        // Can make them dynamic
        int width = 2;
        int height = 2;

        if(CheckBigStructure(position, width, height)){
            int randomIndex = GetRandomWeightedIndex(bigStructureWeights);
            placementManager.PlaceObjectOnTheMap(position,bigStructurePrefabs[randomIndex].prefab,CellType.Structure,width,height);
            // Play placement audio
        }

    }

    private bool CheckBigStructure(Vector3Int position, int width, int height)
    {
        bool nearRoad = false;
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                var newPosition = position + new Vector3Int(x, 0, z);
                // Check the positions

                if(DefaultCheck(newPosition) == false)
                {
                    return false;
                }
                if(nearRoad == false)
                {
                    nearRoad = RoadCheck(newPosition);
                }
            }
        }
        return nearRoad;
    }

    private int GetRandomWeightedIndex(float[] weights)
    {
        float sum = 0f;
        for (int i = 0; i < weights.Length; i++)
        {
            sum = weights[i];
        }

        float randomValue = UnityEngine.Random.Range(0, sum);
        float tempSum = 0;
        for (int i = 0; i < weights.Length; i++)
        {
            // Prefab selection between ranges
            // 0 -> wight[0], weight[0] -> weight[1]
            if(randomValue >=  tempSum && randomValue < tempSum + weights[i])
            {
                return i;
            }
            tempSum += weights[i];
        }
        return 0;
    }

    private bool CheckPositionBeforePlacement(Vector3Int position)
    {
        // Default checks when placing structures
        if(DefaultCheck(position) == false)
        {
            return false;
        }
        // Only Place structure if they are near a road
        if(RoadCheck(position) == false)
        {
            return false;
        }
 
        return true;

    }

    private bool RoadCheck(Vector3Int position)
    { 
        if(placementManager.GetNeighboursOfTypeFor(position,CellType.Road).Count <= 0)
        {
            Debug.Log("Please place object near a road");
            return false;
        }
        return true;
    }

    private bool DefaultCheck(Vector3Int position)
    {
        if(placementManager.CheckIfPositionInBound(position) == false)
        {
            Debug.Log("This position is out of bounds of the grid");
            return false;
        }
        if(placementManager.CheckIfPositionIsFree(position) == false)
        {
            Debug.Log("This position is already taken");
            return false;
        }
        return true;
    }
}

// Random place a structure depending on weight
[Serializable]

public struct StructurePrefabWeighted
{
    public GameObject prefab;

    [Tooltip("For determining which prefabs will be selected more frequently by random selection")]
    [Range(0,1)]
    public float weight;

}
