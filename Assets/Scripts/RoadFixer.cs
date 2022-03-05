// Implementation based on Sunny Vale Studio


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoadFixer : MonoBehaviour
{
    public GameObject deadEnd, roadStraight, corner, threeWay, fourWay;

    public void FixRoadAtPosition( PlacementManager placementManager, Vector3Int temporaryPosition)
    {
        // Get the neighbors of the temporary position and how to adjust road
        // Use the Gird class that contains logic for checking neighbors
        // Always return neighbors in this order[right, up, left, down]
        var result = placementManager.GetNeighbourTypesFor(temporaryPosition);
        int roadCount = 0;
        roadCount = result.Where(x => x == CellType.Road).Count();
        
        // CreateDeadEnd on 0 when no road neighbors
        // CreateDeadEnd on 1 when there are atleast 1 road neighbor
        if(roadCount == 0 || roadCount == 1)
        {
            CreateDeadEnd(placementManager, result, temporaryPosition);

        }else if (roadCount == 2) 
        {
            // Create straight road, or corner because neighbors could be [right and up] or [left and down] or [right and down] or [left and up]
            if(CreateStraightRoad(placementManager, result, temporaryPosition)){
                return;
            }

            CreateCorner(placementManager, result, temporaryPosition);

        }else if(roadCount == 3)
        {
            CreateThreeWay(placementManager, result, temporaryPosition);

        }else
        {
            CreateFourWay(placementManager, result, temporaryPosition);
        }
    }

    // !!!Important Method rotations based on prefabs!!!

    private void CreateFourWay(PlacementManager placementManager, CellType[] result, Vector3Int temporaryPosition)
    {
        placementManager.ModifyStructureModel(temporaryPosition,fourWay,Quaternion.identity);
    }

    // [Left, up, right, down]
    private void CreateThreeWay(PlacementManager placementManager, CellType[] result, Vector3Int temporaryPosition)
    {
        // [up, right, down]
        if (result[1] == CellType.Road && result[2] == CellType.Road && result[3] == CellType.Road)
        {
            placementManager.ModifyStructureModel(temporaryPosition, threeWay, Quaternion.identity);
        }
        // [ right, down, left]
        else if (result[2] == CellType.Road && result[3] == CellType.Road && result[0] == CellType.Road)
        {
            placementManager.ModifyStructureModel(temporaryPosition, threeWay, Quaternion.Euler(0,90,0));
        }
        // [ down, left, up]
        else if (result[3] == CellType.Road && result[0] == CellType.Road && result[1] == CellType.Road)
        {
            placementManager.ModifyStructureModel(temporaryPosition, threeWay, Quaternion.Euler(0,180,0));
        }
        // [ left, up, right]
        else if (result[0] == CellType.Road && result[1] == CellType.Road && result[2] == CellType.Road)
        {
            placementManager.ModifyStructureModel(temporaryPosition, threeWay, Quaternion.Euler(0,270,0));
        }
    }

    // [Left, up, right, down]
    private void CreateCorner(PlacementManager placementManager, CellType[] result, Vector3Int temporaryPosition)
    {
        // [up, right]
        if (result[1] == CellType.Road && result[2] == CellType.Road)
        {
            placementManager.ModifyStructureModel(temporaryPosition, corner, Quaternion.Euler(0,90,0));
        }
        // [ right, down]
        else if (result[2] == CellType.Road && result[3] == CellType.Road)
        {
            placementManager.ModifyStructureModel(temporaryPosition, corner, Quaternion.Euler(0,180,0));
        }
        // [ down, left]
        else if (result[3] == CellType.Road && result[0] == CellType.Road)
        {
            placementManager.ModifyStructureModel(temporaryPosition, corner, Quaternion.Euler(0,270,0));
        }
        // [ left, up]
        else if (result[0] == CellType.Road && result[1] == CellType.Road)
        {
            placementManager.ModifyStructureModel(temporaryPosition, corner, Quaternion.identity);
        }
    }

    // [Left, up, right, down]
    private bool CreateStraightRoad(PlacementManager placementManager, CellType[] result, Vector3Int temporaryPosition)
    {
        // [left, up]
        if (result[0] == CellType.Road && result[2] == CellType.Road)
        {
            placementManager.ModifyStructureModel(temporaryPosition, roadStraight, Quaternion.identity);
            return true;
        }
        // [left, up]
        else if (result[1] == CellType.Road && result[3] == CellType.Road)
        {
            placementManager.ModifyStructureModel(temporaryPosition, roadStraight, Quaternion.Euler(0,90,0));
            return true;
        }
        return false;
    }

    // [Left, up, right, down]
    private void CreateDeadEnd(PlacementManager placementManager, CellType[] result, Vector3Int temporaryPosition)
    {
        Debug.Log("Creating Dead End");
        // [up]
        if (result[1] == CellType.Road)
        {
            placementManager.ModifyStructureModel(temporaryPosition, deadEnd, Quaternion.Euler(0,270,0));
        }
        // [ right, down]
        else if (result[2] == CellType.Road)
        {
            placementManager.ModifyStructureModel(temporaryPosition, deadEnd, Quaternion.identity);
        }
        // [ down, left]
        else if (result[3] == CellType.Road)
        {
            placementManager.ModifyStructureModel(temporaryPosition, deadEnd, Quaternion.Euler(0,90,0));
        }
        // [ left, up]
        else if (result[0] == CellType.Road)
        {
            placementManager.ModifyStructureModel(temporaryPosition, deadEnd, Quaternion.Euler(0,180,0));
        }
    }
}
