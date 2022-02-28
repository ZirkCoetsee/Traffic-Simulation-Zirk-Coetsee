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

    //For the actual model that is shown on the map
    private Dictionary<Vector3Int,StructureModel> structureDictionary = new Dictionary<Vector3Int, StructureModel>();



    private void Start() 
    {
        // Will be filled with vertices of type empty
        placementGrid = new Grid(width,height);
    }

    internal CellType[] GetNeighbourTypesFor(Vector3Int position)
    {
        return placementGrid.GetAllAdjacentCellTypes(position.x,position.z);
    }

    internal bool CheckIfPositionInBound(Vector3Int position)
    {
        // Make sure structure is placed on graph
        if(position.x >= 0 && position.x < width && position.z >= 0 && position.z < height)
        {
            return true;
        }
        return false;
    }


    internal void PlaceObjectOnTheMap(Vector3Int position, GameObject structurePrefab, CellType type)
    {
        placementGrid[position.x,position.z] = type;
        StructureModel structure = CreateANewStructureModel(position,structurePrefab,type);
        structureDictionary.Add(position, structure);
        DestroyNature(position);
    }

    private void DestroyNature(Vector3Int position)
    {
        //  Use Raycast to destroy nature object on the map in the area of the road,structures
        // Rough logic here, but make sure to put nature object on Nature layer
        RaycastHit[] hits = Physics.BoxCastAll(position+new Vector3(0,0.5f,0),new Vector3(0.5f,0.5f,0.5f),
        transform.up, Quaternion.identity,1f, 1<< LayerMask.NameToLayer("Nature"));
        foreach (var item in hits)
        {
            Destroy(item.collider.gameObject);
        }
    }



    internal bool CheckIfPositionIsFree(Vector3Int position)
    {
        return CheckIfPositionIsOfType(position, CellType.Empty);
    }

    private bool CheckIfPositionIsOfType(Vector3Int position, CellType type)
    {
        return placementGrid[position.x, position.z] == type;
    }

    internal void PlaceTemporaryStructure(Vector3Int position, GameObject structurePrefab, CellType type)
    {
        placementGrid[position.x,position.z] = type;
        StructureModel structure = CreateANewStructureModel(position,structurePrefab,type);
        temporaryRoadObjects.Add(position, structure);
    }

   internal List<Vector3Int> GetNeighboursOfTypeFor(Vector3Int position, CellType type)
    {
        var neighbourVertices = placementGrid.GetAdjacentCellsOfType(position.x, position.z, type);
        List<Vector3Int> neighbours = new List<Vector3Int>();
        foreach (var point in neighbourVertices)
        {
            neighbours.Add(new Vector3Int(point.X, 0, point.Y));
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

    internal List<Vector3Int> GetPathBetween(Vector3Int startPosition, Vector3Int endPosition)
    {
        // Could possibly replace with DOTS?
        // Return a list of point that the A* finds
        var resultPath = GridSearch.AStarSearch(placementGrid, new Point(startPosition.x, startPosition.z), new Point(endPosition.x,endPosition.z));
        List<Vector3Int> path = new List<Vector3Int>();
        foreach (Point point in resultPath)
        {
            // add the points to the path
            path.Add(new Vector3Int(point.X,0,point.Y));
        }

        return path;
    }

    internal void RemoveAllTemporaryStructures()
    {
        foreach (var structure in temporaryRoadObjects.Values)
        {
            var position = Vector3Int.RoundToInt(structure.transform.position);
            placementGrid[position.x,position.z]  = CellType.Empty;
            Destroy(structure.gameObject);
        }
        temporaryRoadObjects.Clear();
    }

    public void ModifyStructureModel(Vector3Int position, GameObject newModel, Quaternion rotation)
    {
        if(temporaryRoadObjects.ContainsKey(position))
        {
            // Swap the model on the position
            temporaryRoadObjects[position].SwapModel(newModel, rotation);
        }else if(structureDictionary.ContainsKey(position))
        {
            structureDictionary[position].SwapModel(newModel, rotation);
        }
    }

    internal void AddTemporaryStructuresToStructureDictionary()
    {
        foreach (var structure in temporaryRoadObjects)
        {
            structureDictionary.Add(structure.Key, structure.Value);
            DestroyNature(structure.Key);

        }
        temporaryRoadObjects.Clear();
    }
}
