// Implementation based on Sunny Vale Studio


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SmartRoad : MonoBehaviour
{
    // Use Queue to handle first in first out
    Queue<CarAI> trafficQueue = new Queue<CarAI>();
    public CarAI currentCar;

    //Set pedestrian waiting true if there are pedestrians in intersection
    //If there are no cars set pedestrian walking to true and no cars are allowed to drive
    [SerializeField] private bool pedestrianWaiting = false, pedestrianWalking = false;
    [field: SerializeField] public UnityEvent OnPedestrianCanwalk { get; set; }
    private void OnTriggerEnter(Collider other) 
    {
        if (other.CompareTag("Car"))
        {
            var car = other.GetComponent<CarAI>();
            if (car != null && car != currentCar && car.IsLastPathIndex() == false)
            {
                trafficQueue.Enqueue(car);
                car.Stop = true;
            }
        }    
    }

    private void Update()
    {
        if(currentCar == null)
        {
            if(trafficQueue.Count > 0 && pedestrianWaiting == false && pedestrianWalking == false)
            {
                currentCar = trafficQueue.Dequeue();
                currentCar.Stop = false;

            }else if(pedestrianWalking || pedestrianWaiting)
            {
                OnPedestrianCanwalk?.Invoke();
                pedestrianWalking = true;
            }
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        if(other.CompareTag("Car"))
        {
            var car = other.GetComponent<CarAI>();
            if (car != null)
            {
                RemoveCar(car);
            }
        } 
    }

    private void RemoveCar(CarAI car)
    {
        if(car == currentCar)
        {
            currentCar = null;
        }
    }

    public void SetPedestrianFlag(bool val)
    {
        if (val)
        {
            pedestrianWaiting = true;
        }
        else
        {
            pedestrianWaiting = false;
            pedestrianWalking = false;
        }
    }
}
