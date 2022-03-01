using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Make sure there is always a rigidbody attached
[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    Rigidbody rb;

    [SerializeField] private float power = 5f;
    [SerializeField] private float torque = 0.5f;
    [SerializeField] private float maxSpeed = 0.5f;

    [SerializeField] private Vector2 movementVector;

    private void Awake() 
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Move(Vector2 movementInput)
    {
        this.movementVector = movementInput;
    }

    private void FixedUpdate()
    {
        if(rb.velocity.magnitude < maxSpeed)
        {
            // Moving car forward
            rb.AddForce(movementVector.y * transform.forward * power);
        }
        // Allow car to turn
        // Note that the car prefab rotation on x an z have been locked
        // Car can not turn while not moving
        rb.AddTorque(movementVector.x * Vector3.up * torque * movementVector.y);
    }
}
