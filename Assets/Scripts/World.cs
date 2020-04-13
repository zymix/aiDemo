
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour{
    public Transform entity;
    public Vector3 crosshair = new Vector3();
    private List<Vehicle> vehicles;
    private float width;
    private float height;
    void Start() {
        var b = entity.GetComponent<Collider>().bounds;
        var ls = entity.localScale;
        width = b.size.x * 0.49f;
        height = b.size.z * 0.49f;
        Debug.Log("w:" + width + " h:" + height);
        InitVehicle();
    }

    void InitVehicle() {
        vehicles = new List<Vehicle>();
        for (int i = 0; i < 2; ++i) {
            var v = Vehicle.Create(this);
            v.transform.position = new Vector3(Random.Range(-1f, 1f) * width, 0, Random.Range(-1f, 1f) * height);
            vehicles.Add(v);
        }
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit)) {
                crosshair.Set(hit.point.x, 0f, hit.point.z);
            }
        }
    }
}
