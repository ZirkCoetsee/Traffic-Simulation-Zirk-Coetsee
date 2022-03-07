using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoadHelper : MonoBehaviour
{
    [Tooltip("All pedestrian markers on selected prefab")]
    [SerializeField] protected List<Marker> pedestrianMarkers;
    [Tooltip("All car markers on selected prefab")]
    [SerializeField] protected List<Marker> carMarkers;
    [SerializeField] protected bool isCorner;
    [SerializeField] protected bool hasCrossWalks;

    float approximateThresholdCorner = 0.3f;

    [Tooltip("Only for dead end road")]
    [SerializeField] private Marker incoming,outgoing;

    public virtual Marker GetPositionForPedestrianToSpawn( Vector3 structurePosition)
    {
        return GetClosestMarkerTo(structurePosition, pedestrianMarkers);
    }

    public virtual Marker GetPositionForCarToSpawn( Vector3 nextPathPosition)
    {
        // return outgoing marker as spawn point
        return outgoing;
    }

    // keep virtual to override in child scripts
    public virtual Marker GetPositionForCarToEnd( Vector3 previousPathPosition)
    {
        // return incoming marker as spawn point
        return incoming;
    }

    // protected to allow child classes to access it
    protected Marker GetClosestMarkerTo(Vector3 structurePosition, List<Marker> pedestrianMarkers, bool isCorner = false)
    {
        if(isCorner)
        {
            //Corner closest markers needs to calculate directions
            foreach (var marker in pedestrianMarkers)
            {
                var direction = marker.Position - structurePosition;
                if(Mathf.Abs(direction.x) < approximateThresholdCorner || 
                Mathf.Abs(direction.z) < approximateThresholdCorner)
                {
                    return marker;
                }
            }
            return null;
        }
        else
        {
            // Find the closest marker
            Marker closestMarker = null;
            float distance = float.MaxValue;
            foreach (var marker in pedestrianMarkers)
            {
                var markerDistance = Vector3.Distance(structurePosition, marker.Position);
                if(distance > markerDistance)
                {
                    distance = markerDistance;
                    closestMarker = marker;
                }
            }
            return closestMarker;
        }
    }

    public Vector3 GetClosestPedestrianPosition(Vector3 currentPosition)
    {
        return GetClosestMarkerTo(currentPosition, pedestrianMarkers, isCorner).Position;
    }

    public Vector3 GetClosestCarMarkerPosition(Vector3 currentPosition)
    {
        return GetClosestMarkerTo(currentPosition, carMarkers, false).Position;
    }

    public List<Marker> GetAllMarkers()
    {
        return pedestrianMarkers;
    }

    public List<Marker> GetAllCarMarkers()
    {
        return carMarkers;
    }
}
