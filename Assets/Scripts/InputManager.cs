// Implementation based on Sunny Vale Studio

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    [SerializeField] Camera mainCamera;

    //Deligates for Mouse click events
    public Action<Vector3Int> OnMouseClick, OnMouseHold;
    public Action OnMouseUp, OnEscape;
    public LayerMask groundMask;

    //Property for camera movement 
    private Vector2 cameraMovementVector;
    public Vector2 CameraMovementVector
    {
        get { return cameraMovementVector; }
    }

    private void Update() {
        // Get Inputs from player
        CheckClickDownEvent();
        CheckClickUpEvent();
        CheckClickHoldEvent();
        CheckArrowInput();

    }

    private Vector3Int? RaycastGround()
    {
        // Check if the raycast has hit the ground
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit, Mathf.Infinity, groundMask))
        {
            Vector3Int positionInt = Vector3Int.RoundToInt(hit.point);
            return positionInt;
        }

        return null;

    }

    private void CheckArrowInput()
    {
        // Get arrows or WASD keys
        cameraMovementVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    private void CheckClickHoldEvent()
    {

        // Refer to the left mouse button
        // Check if mouse pointer is over game object
        // Important to have a EventSystem object in current scene!
            if( Input.GetMouseButton(0) && EventSystem.current.IsPointerOverGameObject() == false)
            {
                // Debug.Log("Mouse Button Hold");

                var position = RaycastGround();
                if(position != null){
                    // Notify any listeners that the mouse has been moved
                    // ? Allows us to avoid exception where nothing is listening  
                    OnMouseHold?.Invoke(position.Value);
                }
            }
    }

    private void CheckClickUpEvent()
    {
        if( Input.GetMouseButtonUp(0) && EventSystem.current.IsPointerOverGameObject() == false)
        {
                // Debug.Log("Mouse Button Up");

            OnMouseUp?.Invoke();
        }
    }

    private void CheckClickDownEvent()
    {
            if( Input.GetMouseButtonDown(0) && EventSystem.current.IsPointerOverGameObject() == false)
            {
                // Debug.Log("Mouse Button Down");
                var position = RaycastGround();
                if(position != null){
                    OnMouseClick?.Invoke(position.Value);
                }
            }
    }

    private void CheckEscClick()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnEscape.Invoke();
        }
    }
}
