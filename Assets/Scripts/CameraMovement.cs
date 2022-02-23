// Implementation based on Sunny Vale Studio

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    public Camera gameCamera;
    public float cameraMovementSpeed =5f;
    public float yRotation = 30f;


    // Start is called before the first frame update
    void Start()
    {
        gameCamera = GetComponent<Camera>();
    }

    public void MoveCamera(Vector3 inputVector){
        var movementVector = Quaternion.Euler(0,yRotation,0) * inputVector;
        gameCamera.transform.position += movementVector * Time.deltaTime * cameraMovementSpeed;
    }


}
