using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MovingEntity{
    public Steering pSteering;
    Vehicle() {
        pSteering = new Steering(this);
        mass = GameConfig.VehicleMass;
        maxSpeed = GameConfig.MaxSpeed;
        maxForce = GameConfig.MaxSteeringForce;
        maxTurnRate = GameConfig.MaxTurnRatePerSecond;
    }

    void Start(){
        if (!world) {
            return;
        }
        setView("Models/Role/Cube");
        //GameObject.Destroy(res);
        world.cellSpace.AddEntity(this);
    }

    
    public void LogicStep(){
        //通过加速度计算当前速度
        Vector3 steeringForce = pSteering.Calculate();
        Vector3 acceleration = steeringForce / mass;
        velocity += acceleration * Time.deltaTime;
        if (velocity.sqrMagnitude > maxSpeed * maxSpeed) {
            velocity.Normalize();
            velocity *= maxSpeed;
        }
        //EntityFunctions.EnforceNonPenetrationConstraint<Vehicle, List<Vehicle>>(this, world.cellSpace.getLastNeighbors());
        //计算当前移动距离
        Vector3 oldPos = pos;
        pos += velocity * Time.deltaTime;
        world.cellSpace.UpdateEntity(this, oldPos);
        if (velocity.sqrMagnitude>0.00001) {
            heading = Vector3.Normalize(velocity);
        }
    }
}
