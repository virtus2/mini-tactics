using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    private Camera mainCamera;
    [Header("최대, 최소 Orthograhpic Size")]
    public int ZOOM_OUT = 5;
    public int ZOOM_IN = 2;
    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f) 
        {
            if(mainCamera.orthographicSize > 2)
                mainCamera.orthographicSize--;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f) 
        {
            if(mainCamera.orthographicSize < 5)
                mainCamera.orthographicSize++;
        }
    }
}
