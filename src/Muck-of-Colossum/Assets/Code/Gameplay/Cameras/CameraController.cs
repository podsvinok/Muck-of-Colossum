using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera currentCamera;
    public Camera CurrentCamera => currentCamera;

    public void Init()
    {
        currentCamera = GetComponent<Camera>();
    }
}
