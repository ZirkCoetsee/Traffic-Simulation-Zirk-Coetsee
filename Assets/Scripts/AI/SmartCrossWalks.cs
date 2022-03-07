// Implementation based on Sunny Vale Studio

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SmartCrossWalks : MonoBehaviour
{
    List<AIAgent> pedestrianList = new List<AIAgent>();

    // using events notify the crosswalks that a pedestrian has entered the space
    // Becking field for in inspector
    [field: SerializeField] public UnityEvent OnPedestrianEnter { get; set; }
    [field: SerializeField] public UnityEvent OnPedestrianExit { get; set; }

    private void OnTriggerEnter(Collider other) 
    {
        var pedestrian = other.GetComponent<AIAgent>();
        if (pedestrian != null && pedestrianList.Contains(pedestrian) ==  false)
        {
            pedestrianList.Add(pedestrian);
            pedestrian.Stop = true;
            OnPedestrianEnter?.Invoke();
            // Inform smart road object that a pedestrian is waiting
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        var pedestrian = other.GetComponentInParent<AIAgent>();
        if (pedestrian != null)
        {
            RemovePedestrian(pedestrian);
        }
    }

    private void RemovePedestrian(AIAgent pedestrian)
    {
        pedestrianList.Remove(pedestrian);
        if (pedestrianList.Count <= 0)
        {
            OnPedestrianExit?.Invoke();
        }
    }

    public void MovePedestrians()
    {
        foreach (var pedestrian in pedestrianList)
        {
            pedestrian.Stop = false;
        }
    }
}
