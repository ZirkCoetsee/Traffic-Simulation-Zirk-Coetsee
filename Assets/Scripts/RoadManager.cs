using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
    public PlacementManager placementManager;

    public List<Vector3Int> temporaryPlacementPositions = new List<Vector3Int>();
    
    // List to check if the currently placed node's neighbor should be fixed
    public List<Vector3Int> roadPositionsToRecheck = new List<Vector3Int>();

    public GameObject roadStraight;

    public RoadFixer roadFixer;

    private void Start()
    {
        roadFixer = GetComponent<RoadFixer>();
    }

    public void PlaceRoad(Vector3Int position)
    {
        // If position is outside of bounds of grid
        if(placementManager.CheckPositionInBound(position) == false)
        {
            return;
        }
        // If position on grid is not free
        if(placementManager.CheckPositionIsFree(position) == false)
        {
            return;
        }
        // Clear before we add current position, for A*
        // We add each space to temporaryPlacementPositons 
        // Modify the neighbors and currently placed prefabs to be the correct models with rotations
        temporaryPlacementPositions.Clear();
        temporaryPlacementPositions.Add(position);
        placementManager.PlaceTemporaryStructure(position,roadStraight, CellType.Road);
        FixRoadPrefabs();
    }

    private void FixRoadPrefabs()
    {
        foreach (var temporaryPosition in temporaryPlacementPositions)
        {
            roadFixer.FixRoadAtPosition(placementManager,temporaryPosition);
            var neighbours = placementManager.GetNeighbourTypesFor(temporaryPosition, CellType.Road);
            foreach(var roadPosition in neighbours)
            {
                roadPositionsToRecheck.Add(roadPosition);
            }
        }
        foreach (var positionToFix in roadPositionsToRecheck)
        {
            roadFixer.FixRoadAtPosition(placementManager,positionToFix);
        }
    }
}
