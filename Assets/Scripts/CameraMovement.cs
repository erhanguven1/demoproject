using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraMovement : MonoBehaviour
{
    public static CameraMovement instance;
    private void Awake()
    {
        instance = this;
    }

    private Camera cam;
    private void Start()
    {
        cam = Camera.main;
    }

    public void MoveCameraToPosition(Vector3 position)
    {
        cam.transform.DOMove(position, .5f);
    }

    public void RotateTo(Vector3 rotation)
    {
        cam.transform.DORotate(rotation, .5f);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            cam.transform.position += Vector3.forward * Input.GetAxis("Mouse Y") + Vector3.right * Input.GetAxis("Mouse X");
        }
    }
}
