// Implementation based on Sunny Vale Studio


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class AdjacencyGraph
{
    // Graph for storing adjacent Vertex class objects on road structures
       Dictionary<Vertex, List<Vertex>> adjacencyDictionary = new Dictionary<Vertex, List<Vertex>>();

        public Vertex AddVertex(Vector3 position)
        {
            if(GetVertexAt(position) != null)
            {
                return null;
            }

            Vertex v = new Vertex(position);
            AddVertex(v);
            return v;

        }

        private void AddVertex(Vertex v)
        {
            // Make sure the graph does not contain the passed Vertex Object class
            if (adjacencyDictionary.ContainsKey(v))
                return;
            adjacencyDictionary.Add(v, new List<Vertex>());
        }

        private Vertex GetVertexAt(Vector3 position)
        {
            return adjacencyDictionary.Keys.FirstOrDefault(x => CompareVertices(position, x.Position));
        }

        private bool CompareVertices(Vector3 position1, Vector3 position2)
        {
            return Vector3.SqrMagnitude(position1 - position2) < 0.0001f;
        }

        public void AddEdge(Vector3 startPosition, Vector3 endPosition)
        {
            // Can't create a edge between the same vertex
            if(CompareVertices(startPosition, endPosition))
            {
                return;
            }
            var v1 = GetVertexAt(startPosition);
            var v2 = GetVertexAt(endPosition);
            if(v1 == null)
            {
                v1 = new Vertex(startPosition);
                AddVertex(v1);
            }
            if(v2 == null)
            {
                v2 = new Vertex(endPosition);
                AddVertex(v2);
            }
            // Add both directions
            // Changed this to only add in once direction to create a directional graph for 3 Way
            AddEdgeBetween(v1, v2);
            // AddEdgeBetween(v2, v1);

        }

        private void AddEdgeBetween(Vertex v1, Vertex v2)
        {
            if(v1 == v2)
            {
                return;
            }
            if (adjacencyDictionary.ContainsKey(v1))
            {
                //List can take two same vertices as two spaces on list so make sure not to add the edge twice
                if(adjacencyDictionary[v1].FirstOrDefault(x => x == v2) == null)
                {
                    adjacencyDictionary[v1].Add(v2);
                }
            }
            else
            {
                AddVertex(v1);
                adjacencyDictionary[v1].Add(v2);
            }

        }

        public List<Vertex> GetConnectedVerticesTo(Vertex v1)
        {
            if (adjacencyDictionary.ContainsKey(v1))
            {
                return adjacencyDictionary[v1];
            }
            return null;
        }

        public List<Vertex> GetConnectedVerticesTo(Vector3 position)
        {
            var v1 = GetVertexAt(position);
            if (v1 == null)
                return null;
            return adjacencyDictionary[v1];
        }

        public void ClearGraph()
        {
            adjacencyDictionary.Clear();
        }

        public IEnumerable<Vertex> GetVertices()
        {
            return adjacencyDictionary.Keys;
        }

        //For debugging override the ToString Method
        // Print all adjacent vertex to current vertex
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (var vertex in adjacencyDictionary.Keys)
            {
                builder.AppendLine("Vertex " + vertex.ToString() + " neighbours: " + String.Join(", ", adjacencyDictionary[vertex]));
            }
            return builder.ToString();
        }

        public static List<Vector3> AStarSearch(AdjacencyGraph graph, Vector3 startPosition, Vector3 endPosition)
        {
            List<Vector3> path = new List<Vector3>();

            Vertex start = graph.GetVertexAt(startPosition);
            Vertex end = graph.GetVertexAt(endPosition);

            List<Vertex> positionsTocheck = new List<Vertex>();
            Dictionary<Vertex, float> costDictionary = new Dictionary<Vertex, float>();
            Dictionary<Vertex, float> priorityDictionary = new Dictionary<Vertex, float>();
            Dictionary<Vertex, Vertex> parentsDictionary = new Dictionary<Vertex, Vertex>();

            positionsTocheck.Add(start);
            priorityDictionary.Add(start, 0);
            costDictionary.Add(start, 0);
            parentsDictionary.Add(start, null);

            while (positionsTocheck.Count > 0)
            {
                Vertex current = GetClosestVertex(positionsTocheck, priorityDictionary);
                positionsTocheck.Remove(current);
                if (current.Equals(end))
                {
                    path = GeneratePath(parentsDictionary, current);
                    return path;
                }

                foreach (Vertex neighbour in graph.GetConnectedVerticesTo(current))
                {
                    float newCost = costDictionary[current] + 1;
                    if (!costDictionary.ContainsKey(neighbour) || newCost < costDictionary[neighbour])
                    {
                        costDictionary[neighbour] = newCost;

                        float priority = newCost + ManhattanDiscance(end, neighbour);
                        positionsTocheck.Add(neighbour);
                        priorityDictionary[neighbour] = priority;

                        parentsDictionary[neighbour] = current;
                    }
                }
            }
            return path;
        }

        private static Vertex GetClosestVertex(List<Vertex> list, Dictionary<Vertex, float> distanceMap)
        {
            Vertex candidate = list[0];
            foreach (Vertex vertex in list)
            {
                if (distanceMap[vertex] < distanceMap[candidate])
                {
                    candidate = vertex;
                }
            }
            return candidate;
        }

        private static float ManhattanDiscance(Vertex endPos, Vertex position)
        {
            return Math.Abs(endPos.Position.x - position.Position.x) + Math.Abs(endPos.Position.z - position.Position.z);
        }

        public static List<Vector3> GeneratePath(Dictionary<Vertex, Vertex> parentMap, Vertex endState)
        {
            List<Vector3> path = new List<Vector3>();
            Vertex parent = endState;
            while (parent != null && parentMap.ContainsKey(parent))
            {
                path.Add(parent.Position);
                parent = parentMap[parent];
            }
            path.Reverse();
            return path;
        }
}
