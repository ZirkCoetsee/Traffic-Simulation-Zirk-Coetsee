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

            // Find closest road to structure
            // Find closest marker on road
            var startMarkerPosition = placementManager.GetStructureAt(startPosition).GetPedestrianSpawnMarker(startStructure.transform.position);
            var endMarkerPosition = placementManager.GetStructureAt(endPosition).GetPedestrianSpawnMarker(endStructure.transform.position);


            var agent = Instantiate(GetRandomPedestrian(),startMarkerPosition.Position, Quaternion.identity);
            var path = placementManager.GetPathBetween(startPosition,endPosition,true);
            if(path.Count > 0)
            {
                path.Reverse();
                List<Vector3> agentPath = GetPedestrianPath(path, startMarkerPosition.Position, endMarkerPosition.Position);
                var aiAgent = agent.GetComponent<AIAgent>();
                aiAgent.Initialize(agentPath);
            }
        }
    }

    private List<Vector3> GetPedestrianPath(List<Vector3Int> path, Vector3 startPosition, Vector3 endPosition)
    {
        graph.ClearGraph();
        CreateAGraph(path);
        // Overiden string
        Debug.Log(graph);

        return AdjacencyGraph.AStarSearch(graph,startPosition,endPosition);
    }

    private void CreateAGraph(List<Vector3Int> path)
    {
        //For 3 Way and 4 Way crossings
         Dictionary<Marker,Vector3> tempDictionary = new Dictionary<Marker, Vector3>();

         for (int i = 0; i < path.Count; i++)
         {
             var currentPosition = path[i];
             var roadStructure = placementManager.GetStructureAt(currentPosition);
             var markerList = roadStructure.GetPedestrianMarkers();
            
             bool limitDistance = markerList.Count == 4;

             tempDictionary.Clear();
             foreach (var marker in markerList)
             {
                //  Add markers from prefab to graph since they are traversable 
                 graph.AddVertex(marker.Position);
                 foreach (var markerNeighbourPosition in marker.GetAdjacentPositions())
                 {
                     graph.AddEdge(marker.Position, markerNeighbourPosition);
                 }

                 if(marker.OpenForConnections && i+1 < path.Count)
                 {
                    //  Access markers on next road prefab
                     var nextRoadStructure = placementManager.GetStructureAt(path[i+1]);
                    // If limit is true, don't add the markers directly to graph but to temp
                    if (limitDistance)
                     {
                         tempDictionary.Add(marker, nextRoadStructure.GetNearestMarkerTo(marker.Position));
                     }else
                     {
                         graph.AddEdge(marker.Position, nextRoadStructure.GetNearestMarkerTo(marker.Position));
                     }
                 }
             }
             if (limitDistance && tempDictionary.Count == 4)
             {
                //  Sort positions to take the shortest distances 
                 var distanceSortedMembers = tempDictionary.OrderBy(x => Vector3.Distance(x.Key.Position,x.Value)).ToList();
                //Only take the 2 positions
                for (int j = 0; j < 2; j++)
                {
                    graph.AddEdge(distanceSortedMembers[j].Key.Position, distanceSortedMembers[j].Value);
                }
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

    private void Update() {
        foreach (var vertex in graph.GetVertices())
        {
            foreach (var vertexNeighbour in graph.GetConnectedVerticesTo(vertex))
            {
                Debug.DrawLine(vertex.Position + Vector3.up,vertexNeighbour.Position + Vector3.up,Color.red);
            }
        }
    }
}
