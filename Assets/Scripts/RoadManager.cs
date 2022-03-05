// Implementation based on Sunny Vale Studio


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

    //For taking initial click and create road between initial and end point 
    private Vector3Int startPosition;
    private bool placementmode = false;
    public RoadFixer roadFixer;

    private void Start()
    {
        roadFixer = GetComponent<RoadFixer>();
    }

    public void PlaceRoad(Vector3Int position)
    {
        // If position is outside of bounds of grid
        if(placementManager.CheckIfPositionInBound(position) == false)
        {
            return;
        }
        // If position on grid is not free
        if(placementManager.CheckIfPositionIsFree(position) == false)
        {
            return;
        }

        // check if placementmode is false
        if(placementmode == false)
        {
            // Clear before we add current position, for A*
            // We add each space to temporaryPlacementPositons 
            // Modify the neighbors and currently placed prefabs to be the correct models with rotations
            temporaryPlacementPositions.Clear();
            roadPositionsToRecheck.Clear();

            placementmode = true;
            startPosition = position;

            temporaryPlacementPositions.Add(position);
            placementManager.PlaceTemporaryStructure(position,roadFixer.deadEnd, CellType.Road);
        }else
        {
            // Remove all already placed road depending of what A* finds
            placementManager.RemoveAllTemporaryStructures();
            temporaryPlacementPositions.Clear();

            foreach (var positionsToFix in roadPositionsToRecheck)
            {
                roadFixer.FixRoadAtPosition(placementManager,positionsToFix);
            }

            roadPositionsToRecheck.Clear();

            // Use A*, could possibly replace with DOTS?
            temporaryPlacementPositions = placementManager.GetPathBetween(startPosition, position);

            foreach (var temporaryPosition in temporaryPlacementPositions)
            {
                // Place temporary structures if the temporary position is free
                    if(placementManager.CheckIfPositionIsFree(temporaryPosition) == false)
                    {
                        roadPositionsToRecheck.Add(temporaryPosition);
                        continue;
                    }
                placementManager.PlaceTemporaryStructure(temporaryPosition,roadFixer.deadEnd, CellType.Road);
            }
        }

         FixRoadPrefabs();
    }

    private void FixRoadPrefabs()
    {
        foreach (var temporaryPosition in temporaryPlacementPositions)
        {
            roadFixer.FixRoadAtPosition(placementManager,temporaryPosition);
            var neighbours = placementManager.GetNeighboursOfTypeFor(temporaryPosition, CellType.Road);
            foreach(var roadPosition in neighbours)
            {
                // Check if road positons to recheck does not already contain the position
                if(roadPositionsToRecheck.Contains(roadPosition) == false )
                {
                    roadPositionsToRecheck.Add(roadPosition);
                }
            }
        }
        foreach (var positionToFix in roadPositionsToRecheck)
        {
            roadFixer.FixRoadAtPosition(placementManager,positionToFix);
        }
    }

    public void FinishPlacingRoad()
    {
        placementmode = false;
        placementManager.AddTemporaryStructuresToStructureDictionary();
        if(temporaryPlacementPositions.Count > 0)
        {
            // Play audio placement sounds
        }
        temporaryPlacementPositions.Clear();
        startPosition = Vector3Int.zero;
    }
}
