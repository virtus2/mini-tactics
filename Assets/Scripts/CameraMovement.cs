using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Camera mainCamera;
    public float movementSpeed = 0.05f;
    void Start()
    {
        mainCamera = Camera.main;
    }
    
    void Update()
    { 
        if (Input.GetKey(KeyCode.W))
        {
            mainCamera.transform.Translate(Vector3.up * movementSpeed);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            mainCamera.transform.Translate(Vector3.down * movementSpeed);
        }
        if(Input.GetKey(KeyCode.A))
        {
            mainCamera.transform.Translate(Vector3.left * movementSpeed);

        }
        else if (Input.GetKey(KeyCode.D))
        {
            mainCamera.transform.Translate(Vector3.right * movementSpeed);
        }
    }
}
