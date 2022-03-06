// Implementation based on Sunny Vale Studio


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Assume that structureModel has road helper
public class StructureModel : MonoBehaviour, IRequireRoad
{
    // Parent of game object so we need the height
    float yHeight = 0;

    public Vector3Int RoadPosition {get; set;}

    public void CreateModel(GameObject model)
    {
        var structure = Instantiate(model,transform);
        yHeight = structure.transform.position.y;
    }

    public void SwapModel(GameObject model, Quaternion rotation)
    {
        foreach (Transform child in transform)
        {
            // Destroy the previously created model mesh
            Destroy(child.gameObject);
        }
        // We then replace the mesh
        var structure = Instantiate(model,transform);
        structure.transform.localPosition = new Vector3(0, yHeight,0);
        structure.transform.localRotation = rotation;
    }

    // Assumes that the child object is a child prefab of the structure model
    public Vector3 GetNearestMarkerTo(Vector3 position)
    {
        return transform.GetChild(0).GetComponent<RoadHelper>().GetClosestPedestrianPosition(position);
    }

    public Marker GetPedestrianSpawnMarker(Vector3 position)
    {
        return transform.GetChild(0).GetComponent<RoadHelper>().GetPositionForPedestrianToSpawn(position);
    }

    public List<Marker> GetPedestrianMarkers(){
        return transform.GetChild(0).GetComponent<RoadHelper>().GetAllMarkers();
    }
}
