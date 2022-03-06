// Implementation based on Sunny Vale Studio


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vertex : IEquatable<Vertex>
{
    // Dictionary of Vertex classes and a list of vertex that are adjacent to these vertex

    public Vector3 Position { get; private set; }
    public Vertex(Vector3 position)
    {
        this.Position = position;
    }
    public bool Equals(Vertex other)
    {
       return Vector3.SqrMagnitude(Position - other.Position) < 0.0001f;
    }

    public override string ToString()
    {
        return Position.ToString();
    }

}
