// Implementation based on Sunny Vale Studio

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    
    [Header("Camera movement script that sits on the Main camera")]
    public CameraMovement cameraMovement;
    public InputManager inputManager;
    public RoadManager roadManager;
    public UIController uiController;
    public StructureManager structureManager;

    private void Start() {
        uiController.OnRoadPlacement += RoadPlacementHandler;
        uiController.OnHousePlacement += HousePlacementHouseHandler;
        uiController.OnSpecialPlacement += SpecialPlacementHandler;
        uiController.OnBigStructurePlacement += BigStructurePlacementHandler;




    }

    private void BigStructurePlacementHandler()
    {
        ClearInputActions();
        inputManager.OnMouseClick += structureManager.PLaceBigStructure;

    }

    private void SpecialPlacementHandler()
    {
        ClearInputActions();
        inputManager.OnMouseClick += structureManager.PlaceSpecial;
        Debug.Log("Place Special");


    }

    private void HousePlacementHouseHandler()
    {
        ClearInputActions();
        inputManager.OnMouseClick += structureManager.PlaceHouse;
        Debug.Log("PlaceHouse");

    }

    private void RoadPlacementHandler()
    {
        ClearInputActions();
        inputManager.OnMouseClick += roadManager.PlaceRoad;
        inputManager.OnMouseHold += roadManager.PlaceRoad;
        inputManager.OnMouseUp += roadManager.FinishPlacingRoad;
    }

    private void ClearInputActions()
    {
        inputManager.OnMouseClick = null;
        inputManager.OnMouseHold = null;
        inputManager.OnMouseUp = null;
    }

    private void Update()
    {
        cameraMovement.MoveCamera(new Vector3(inputManager.CameraMovementVector.x,0,
        inputManager.CameraMovementVector.y));
    }
}
