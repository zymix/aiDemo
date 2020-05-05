using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour{
    public Transform entity;
    public Vector3 crosshair = new Vector3();
    public CellSpacePartition<Vehicle> cellSpace;
    public List<Vehicle> vehicles;
    public List<Obstacle> obstacles;
    public List<Wall> walls;
    public List<GameObject> wallPrefabs;
    private float width;
    private float height;

    private GameObject crosshairView;
    void Start() {
        var b = entity.GetComponent<Collider>().bounds;
        var ls = entity.localScale;
        width = b.size.x;
        height = b.size.z;
        cellSpace = new CellSpacePartition<Vehicle>(Vector3.zero, new Vector3(width, 1, height), new Vector3(6, 1, 6), 50);

        InitVehicle();
        InitObstacles();
        InitWalls();
        crosshairView = GameObject.Find("crossHair");
        cellSpace.DebugDrawOn();
    }

    void InitVehicle() {
        vehicles = new List<Vehicle>();
        float halfwidth = width * 0.49f, halfheight = height * 0.49f;
        for (int i = 0; i < 25; ++i) {
            var v = BaseEntity.Create<Vehicle>(this);
            v.transform.position = new Vector3(UnityEngine.Random.Range(-1f, 1f) * halfwidth, 0, UnityEngine.Random.Range(-1f, 1f) * halfheight);
            vehicles.Add(v);
            v.pSteering.FlockingOn();
        }
    }

    void InitObstacles() {
        obstacles = new List<Obstacle>();
        // for (int i = 0; i < 1; ++i) {
        //     var ob = BaseEntity.Create<Obstacle>(this);
        //     //view.position = new Vector3(UnityEngine.Random.Range(-1f, 1f) * width, 0, UnityEngine.Random.Range(-1f, 1f) * height);
        //     ob.transform.position = new Vector3(0, 0, 0);
        //     obstacles.Add(ob);
        // }
    }
    void InitWalls() {
        walls = new List<Wall>();
        for(int i = 0; i<wallPrefabs.Count; ++i){
            var ob = Wall.Create(wallPrefabs[i], this);
            ob.view = wallPrefabs[i].transform;
            ob.transform.position = wallPrefabs[i].transform.position;
            walls.Add(ob);
        }
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit)) {
                crosshair.Set(hit.point.x, 0f, hit.point.z);
                crosshairView.transform.position = crosshair;
            }
        }
    }
    public void TagVehicleWithinViewRange(BaseEntity vehicle, float detectBoxLength) {
        EntityFunctions.TagNeighbors<BaseEntity, List<Vehicle>>(vehicle, vehicles, detectBoxLength);
    }
    public void TagObstaclesWithinViewRange(BaseEntity vehicle, float detectBoxLength) {
        EntityFunctions.TagNeighbors<BaseEntity, List<Obstacle>>(vehicle, obstacles, detectBoxLength);
    }
}
