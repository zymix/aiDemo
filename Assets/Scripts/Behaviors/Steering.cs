using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Steering
{
    private Vehicle vehicle;
    private Vector3 steeringForce;
    //徘徊
    private float wanderRadius;
    private float wanderDistance;
    private float wanderJitter;
    private Vector3 wanderTarget;

    public Steering(Vehicle vehicle) {
        this.vehicle = vehicle;
        steeringForce = new Vector3(0f, 0f, 0f);
    }
    //计算合力
    public Vector3 Calculate() {
        steeringForce.Set(0f, 0f, 0f);
        CalculateWeightedSum();
        return steeringForce;
    }

    private void CalculateWeightedSum() {
        steeringForce += Seek(vehicle.world.crosshair);
    }


    //靠近的力（最大速度）
    public Vector3 Seek(Vector3 target) {
        Vector3 desiredV = (target - vehicle.pos).normalized * vehicle.maxSpeed;
        return desiredV - vehicle.velocity;
    }

    //离开的力（最大速度）
    public Vector3 Flee(Vector3 target) {
        Vector3 v = vehicle.pos - target;
        //加入触发范围判断
        //if (v.sqrMagnitude > 10000f) {
        //    return Vector3.zero;
        //}
        Vector3 desiredV = v.normalized * vehicle.maxSpeed;
        return desiredV - vehicle.velocity;
    }

    //抵达的力（根据距离变换）
    public Vector3 Arrive(Vector3 target) {
        Vector2 toTarget = target - vehicle.pos;
        float dist = toTarget.magnitude;
        if (dist > 0) {
            float dec = 0.3f;
            float speed = dist / dec;
            speed = Mathf.Min(speed, vehicle.maxSpeed);
            Vector3 v = toTarget / dist * speed;
            return v - vehicle.velocity;
        }
        return Vector3.zero;
    }
    //追逐的力（根据逃脱者会达到的位置计算靠近的力）
    public Vector3 Pursuit(Vehicle evader) {
        Vector3 toEvader = evader.pos - vehicle.pos;
        float relativeHeading = Vector3.Dot(vehicle.heading, evader.heading);
        //当前追逐逃脱者方向和当前前进方向相同，且脱逃者前进方向的反方向与当前vehicle前进方向的夹角小于20度时
        if (Vector3.Dot(toEvader, vehicle.heading)>0f && relativeHeading < -0.95) {
            return Seek(evader.pos);//直接计算靠近的力
        }
        //否则预判，考虑追上所需要的时间后，再计算靠近的力
        float needTime = toEvader.magnitude / (vehicle.maxSpeed + evader.Speed());
        return Seek(evader.pos + needTime * evader.velocity);
    }
    //逃避的力（根据追逐者会达到的位置计算离开的力）
    public Vector3 Evade(Vehicle pursuer) {
        Vector3 toPusuer = pursuer.pos - vehicle.pos;
        //否则预判，考虑追上所需要的时间后，再计算靠近的力
        float needTime = toPusuer.magnitude / (vehicle.maxSpeed + pursuer.Speed());
        return Flee(pursuer.pos + needTime * pursuer.velocity);
    }
    //Reynolds的计算光滑没抖动地徘徊的力（根据追逐者会达到的位置计算离开的力）
    public Vector3 Wander() {
        //计算一个随机抖动的单位矢量
        float jitterThisTimeSlice = wanderJitter * Time.deltaTime;
        wanderTarget.Set(
            wanderTarget.x + Random.Range(-1.0f, 1.0f) * jitterThisTimeSlice,
            wanderTarget.y,
            wanderTarget.z + Random.Range(-1.0f, 1.0f) * jitterThisTimeSlice
        );
        wanderTarget.Normalize();
        wanderTarget *= wanderRadius;
        //上次的移动位置，沿这个矢量前移一定距离后，把该位置转换到世界坐标系
        Vector3 target = new Vector3(wanderTarget.x + wanderDistance, wanderTarget.y, wanderTarget.z);
        var matrix = new Matrix4x4(vehicle.side, vehicle.transform.up, vehicle.heading, Vector4.zero);
        matrix.m33 = 1f;
        target = matrix * (Vector4)target;
        //同一坐标系下计算偏移力
        return target - vehicle.pos;
    }

    public Vector3 ForwardComponent() {
        return Vector3.zero;
    }
    public Vector3 SideComponent() {
        return Vector3.zero;
    }
    public void SetPath() {

    }
    public void SetTarget(Vector3 target) {

    }
    public void SetTargetAgent1(Vehicle vehicle) {

    }
    public void SetTargetAgent2(Vehicle vehicle) {

    }
    public void SeekOn() {

    }
    public void FleeOn() {

    }
    public void ArriveOn() {

    }
    public void SeekOff() {

    }
    public void FleeOff() {

    }
    public void ArriveOff() {

    }
}
