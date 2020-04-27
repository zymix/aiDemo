using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour{
    public Transform entity;
    public Vector3 crosshair = new Vector3();
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
        width = b.size.x * 0.49f;
        height = b.size.z * 0.49f;
        InitVehicle();
        InitObstacles();
        InitWalls();
        crosshairView = GameObject.Find("crossHair");
    }

    void InitVehicle() {
        vehicles = new List<Vehicle>();
        for (int i = 0; i < 4; ++i) {
            var v = BaseEntity.Create<Vehicle>(this);
            v.transform.position = new Vector3(UnityEngine.Random.Range(-1f, 1f) * width, 0, UnityEngine.Random.Range(-1f, 1f) * height);
            vehicles.Add(v);
        }
        //vehicles[0].transform.localScale *= 2.0f;
        vehicles[0].pSteering.ArriveOn();
        vehicles[1].pSteering.OffsetPursuitOn(vehicles[0], new Vector3(0, 0, 1));
        vehicles[2].pSteering.OffsetPursuitOn(vehicles[0], new Vector3(-1, 0, 0));
        vehicles[3].pSteering.OffsetPursuitOn(vehicles[0], new Vector3(0, 0, -1));
    }

    void InitObstacles() {
        obstacles = new List<Obstacle>();
        for (int i = 0; i < 1; ++i) {
            var ob = BaseEntity.Create<Obstacle>(this);
            //view.position = new Vector3(UnityEngine.Random.Range(-1f, 1f) * width, 0, UnityEngine.Random.Range(-1f, 1f) * height);
            ob.transform.position = new Vector3(0, 0, 0);
            obstacles.Add(ob);
        }
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
