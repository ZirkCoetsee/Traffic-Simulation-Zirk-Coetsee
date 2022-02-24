using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureModel : MonoBehaviour
{
    // Parent of game object so we need the height
    float yHeight = 0;

    public void CreateModel(GameObject model)
    {
        var structure = Instantiate(model,transform);
        yHeight = structure.transform.position.y;
    }

    public void SwapModel(GameObject model, Quaternion rotation)
    {
        foreach (Transform child in transform)
        {
            // Destroy the previously created model mesh
            Destroy(child.gameObject);
        }
        // We then replace the mesh
        var structure = Instantiate(model,transform);
        structure.transform.localPosition = new Vector3(0, yHeight,0);
        structure.transform.localRotation = rotation;
    }
}
