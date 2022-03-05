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
    // Changed actions to be events only, assign and un-assign
    public event Action<Ray> OnMouseClick, OnMouseHold;
    public event Action OnMouseUp, OnEscape;
    private Vector2 mouseMovementVector = Vector2.zero;
    public Vector2 CameraMovementVector { get => mouseMovementVector; }

    private void Update() {
        // Get Inputs from player
        CheckClickDownEvent();
        CheckClickUpEvent();
        CheckClickHoldEvent();
        CheckArrowInput();
        CheckEscClick();
    }

    private void CheckArrowInput()
    {
        // Get arrows or WASD keys
        mouseMovementVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    private void CheckClickHoldEvent()
    {

        // Refer to the left mouse button
        // Check if mouse pointer is over game object
        // Important to have a EventSystem object in current scene!
            if( Input.GetMouseButton(0) && EventSystem.current.IsPointerOverGameObject() == false)
            {
                OnMouseHold?.Invoke(mainCamera.ScreenPointToRay(Input.mousePosition));
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
                OnMouseClick?.Invoke(mainCamera.ScreenPointToRay(Input.mousePosition)); 
            }
    }

    private void CheckEscClick()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnEscape.Invoke();
        }
    }

    public void ClearEvents()
    {
        OnMouseClick = null;
        OnMouseHold = null;
        OnEscape = null;
        OnMouseUp = null;
    }
}
