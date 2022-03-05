// Implementation based on Sunny Vale Studio


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

    internal void PlaceObjectOnTheMap(Vector3Int position, GameObject structurePrefab, CellType type, int width = 1, int height = 1)
    {
        StructureModel structure = CreateANewStructureModel(position, structurePrefab, type);

        var structureNeedingRoad = structure.GetComponent<IRequireRoad>();
        if (structureNeedingRoad != null)
        {
            // For the nearest position to the road where pedestrians will 'enter'
            structureNeedingRoad.RoadPosition = GetNearestRoad(position, width, height).Value;
            Debug.Log("My nearest road position is: " + structureNeedingRoad.RoadPosition);
        }

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                var newPosition = position + new Vector3Int(x, 0, z);
                placementGrid[newPosition.x, newPosition.z] = type;
                structureDictionary.Add(newPosition, structure);
                DestroyNature(newPosition);
            }
        }

    }

    private Vector3Int? GetNearestRoad(Vector3Int position, int width, int height)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var newPosition = position + new Vector3Int(x, 0, y);
                var roads = GetNeighboursOfTypeFor(newPosition, CellType.Road);
                if (roads.Count > 0)
                {
                    return roads[0];
                }
            }
        }
        return null;
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

    internal List<Vector3Int> GetPathBetween(Vector3Int startPosition, Vector3Int endPosition, bool isAgent = false)
    {
        // Could possibly replace with DOTS?
        // Return a list of point that the A* finds
        var resultPath = GridSearch.AStarSearch(placementGrid, new Point(startPosition.x, startPosition.z), new Point(endPosition.x,endPosition.z), isAgent);
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

    public StructureModel GetRandomRoad()
    {
        var point = placementGrid.GetRandomRoadPoint();
        return GetStructureAt(point);
    }

    public StructureModel GetRandomSpecialStructure()
    {
        var point = placementGrid.GetRandomSpecialStructurePoint();
        return GetStructureAt(point);
    }

    public StructureModel GetRandomHouseStructure()
    {
        var point = placementGrid.GetRandomHouseStructurePoint();
        return GetStructureAt(point);
    }

    public List<StructureModel> GetAllHouses()
    {
        List<StructureModel> returnList = new List<StructureModel>();
        var housePositions = placementGrid.GetAllHouses();
        foreach (var point in housePositions)
        {
            returnList.Add(structureDictionary[new Vector3Int(point.X, 0, point.Y)]);
        }
        return returnList;
    }

    internal List<StructureModel> GetAllSpecialStructures()
    {
        List<StructureModel> returnList = new List<StructureModel>();
        var housePositions = placementGrid.GetAllSpecialStructure();
        foreach (var point in housePositions)
        {
            returnList.Add(structureDictionary[new Vector3Int(point.X, 0, point.Y)]);
        }
        return returnList;
    }
    private StructureModel GetStructureAt(Point point)
    {
        if (point != null)
        {
            return structureDictionary[new Vector3Int(point.X, 0, point.Y)];
        }
        return null;
    }

    public StructureModel GetStructureAt(Vector3Int position)
    {
        if (structureDictionary.ContainsKey(position))
        {
            return structureDictionary[position];
        }
        return null;
    }
}
