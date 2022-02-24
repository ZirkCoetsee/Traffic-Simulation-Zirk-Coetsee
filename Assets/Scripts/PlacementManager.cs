using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementManager : MonoBehaviour
{

    // Ref to graph that will store data about map
    public int width, height;
    Grid placementGrid;

    //For replacing the structureModel, replacing the structureModel on point depending on what prefab or StructureModel is needed
    private Dictionary<Vector3Int,StructureModel> temporaryRoadObjects = new Dictionary<Vector3Int, StructureModel>();

    private void Start() 
    {
        // Will be filled with vertices of type empty
        placementGrid = new Grid(width,height);
    }

    internal CellType[] GetNeighbourTypesFor(Vector3Int position)
    {
        return placementGrid.GetAllAdjacentCellTypes(position.x,position.z);
    }

    internal bool CheckPositionInBound(Vector3Int position)
    {
        // Make sure structure is placed on graph
        if(position.x >= 0 && position.x < width && position.z >= 0 && position.z < height)
        {
            return true;
        }
        return false;
    }

    internal bool CheckPositionIsFree(Vector3Int position)
    {
        return CheckIfPositionIsOfType(position, CellType.Empty);
    }

    internal bool CheckIfPositionIsOfType(Vector3Int position, CellType type)
    {
        // Grid is an array of celltypes, override the index operator of grid class
        return placementGrid[position.x,position.z] == type;
    }

    internal void PlaceTemporaryStructure(Vector3Int position, GameObject structurePrefab, CellType type)
    {
        placementGrid[position.x,position.z] = type;
        StructureModel structure = CreateANewStructureModel(position,structurePrefab,type);
        temporaryRoadObjects.Add(position, structure);
    }

    internal List<Vector3Int> GetNeighbourTypesFor(Vector3Int position, CellType type)
    {
        var neighbourVertices = placementGrid.GetAdjacentCellsOfType(position.x,position.z,type);
        List<Vector3Int> neighbours = new List<Vector3Int>();
        foreach (var point in neighbourVertices)
        {
            neighbours.Add(new Vector3Int(point.X,0,point.Y));
        }
        return neighbours;
    }

    private StructureModel CreateANewStructureModel(Vector3Int position, GameObject structurePrefab, CellType type)
    {
        GameObject structure = new GameObject(type.ToString());
        structure.transform.SetParent(transform);
        structure.transform.localPosition = position;
        var structureModel = structure.AddComponent<StructureModel>();
        structureModel.CreateModel(structurePrefab);
        return structureModel;
    }

    public void ModifyStructureModel(Vector3Int position, GameObject newModel, Quaternion rotation)
    {
        if(temporaryRoadObjects.ContainsKey(position))
        {
            // Swap the model on the position
            temporaryRoadObjects[position].SwapModel(newModel, rotation);
        }
    }
}
