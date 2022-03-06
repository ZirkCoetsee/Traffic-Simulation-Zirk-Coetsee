// Implementation based on Sunny Vale Studio


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CarAI : MonoBehaviour
{
    [SerializeField] private List<Vector3> path = null;

    // arriveDistance during travel can be further
    [SerializeField] private float arriveDistance = .2f, lastpointArriveDistance = .1f;

    // Threshhold of to check the rotation of the car in relation to next point
    // Continue rotating car until angle is within 5 degrees
    [SerializeField] private float turningAngleOffset = 5f;

    //Serialized this field to debug view the target 
    [SerializeField] private Vector3 currentTargetPosition;

    private int index = 0;

    // Stop car via private property
    private bool stop;
    public bool Stop
    {
        get { return stop; }
        set { stop = value; }
    }


    //CarController move method attached in inspector 
    [field: SerializeField]
    public UnityEvent<Vector2> OnDrive { get; set; }

    private void Start() 
    {
        if (path == null || path.Count == 0)
        {
            Stop = true;
        }else
        {
            currentTargetPosition = path[index];
        }
    }

    public void SetPath(List<Vector3> path)
    {
        // If there is no path there is no point in having the car
        if(path.Count == 0)
        {
            Destroy(gameObject);
            return;
        }
        this.path = path;
        index = 0;
        // Reset the start position
        currentTargetPosition = this.path[index];

        //Calculate angle and rotate car to point car in the direction of the next point
        //From world position to local position
        Vector3 relativePoint = transform.InverseTransformPoint(this.path[index + 1]);

        float angle = Mathf.Atan2(relativePoint.x,relativePoint.z) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0,angle,0);
        Stop = false;
    }

    private void Update() 
    {
        CheckIfArrived();
        Drive();
    }

    private void Drive()
    {
        if (Stop)
        {
            // Check if there is a method listening to event and only if there is something listening call invoke
            OnDrive?.Invoke(Vector2.zero);
        }
        else
        {
            Vector3 relativePoint = transform.InverseTransformPoint(currentTargetPosition);
            float angle = Mathf.Atan2(relativePoint.x,relativePoint.z) * Mathf.Rad2Deg;
            var rotateCar = 0;
            if (angle > turningAngleOffset)
            {
                // Rotate right
                rotateCar = 1;
            }else if (angle < -turningAngleOffset)
            {
                // Rotate left
                rotateCar = -1;
            }
            // Always moving forward if the car is not stopped
            OnDrive?.Invoke(new Vector2(rotateCar,1));
        }
    }

    private void CheckIfArrived()
    {
        if (Stop == false)
        {
            var distanceToCheck = arriveDistance;
            // If the point is the last point in the list
            if (index == path.Count - 1)
            {
                distanceToCheck = lastpointArriveDistance;
            }
            // 
            if (Vector3.Distance(currentTargetPosition, transform.position) < distanceToCheck)
            {
                SetNextTargetIndex();
            }
        }
    }

    private void SetNextTargetIndex()
    {
        index++;
        // Check if index is in bounds of path
        if (index >= path.Count)
        {
            Stop = true;
            Destroy(gameObject);
        }
        else
        {
            currentTargetPosition = path[index];
        }
    }
}
