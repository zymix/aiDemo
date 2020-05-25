using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Vector3 distance = new Vector3(0, 3, -4);
    public Camera cam;
    public Vector3 offset = new Vector3(0, 1, 0);
    public float speed = 3f;

    public void Awake() {
        cam = GetComponent<Camera>();
    }

}
