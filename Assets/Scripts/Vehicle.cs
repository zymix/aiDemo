using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MovingEntity{
    public World world;
    public Steering pSteering;
    Vehicle() {
        pSteering = new Steering(this);
        mass = 10f;
        maxSpeed = 10f;
    }

    void Start(){
        if (!world) {
            return;
        }
        GameObject res = ResManager.LoadAsset<GameObject>("Models/Role/Cube");
        view = Instantiate<GameObject>(res, transform).transform;
        //GameObject.Destroy(res);
    }

    
    void Update(){
        //通过加速度计算当前速度
        Vector3 steeringForce = pSteering.Calculate();
        Vector3 acceleration = steeringForce / mass;
        velocity += acceleration * Time.deltaTime;
        if (velocity.sqrMagnitude > maxSpeed * maxSpeed) {
            velocity = heading * maxSpeed;
        }
        //计算当前移动距离
        transform.position += velocity * Time.deltaTime;
        if (velocity.sqrMagnitude>0.00001) {
            heading = velocity.normalized;
            side = Vector2.Perpendicular(heading);
        }
    }

    public static Vehicle Create(World world) {
        var obj = new GameObject();
        var vehicle = obj.AddComponent<Vehicle>();
        vehicle.world = world;
        return vehicle;
    }
}
