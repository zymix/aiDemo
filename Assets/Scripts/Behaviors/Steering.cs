using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Steering
{
    public enum Deceleration { slow = 3, normal = 2, fast = 1 };
    private Vehicle vehicle;
    private Vector3 steeringForce;
    //徘徊
    private float wanderRadius = 1.2f;
    private float wanderDistance = 2.0f;
    private float wanderJitter = 80.0f;
    private Vector3 wanderTarget;
    //行动目标
    private Vehicle TargetAgent1;
    private Vehicle TargetAgent2;
    //检测盒（AABB）长度
    private float detectBoxLength;

    public Steering(Vehicle vehicle) {
        this.vehicle = vehicle;
        steeringForce = new Vector3(0f, 0f, 0f);
        wanderTarget = new Vector3(0f, 0f, 0f);
    }
    //计算合力
    public Vector3 Calculate() {
        steeringForce.Set(0f, 0f, 0f);
        CalculateWeightedSum();
        return steeringForce;
    }

    private void CalculateWeightedSum() {
        steeringForce += Wander();
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
    public Vector3 Arrive(Vector3 target,  Deceleration deceleration) {
        Vector3 toTarget = target - vehicle.pos;
        float dist = toTarget.magnitude;
        if (dist > 0) {
            float dec = 0.3f;
            float speed = dist / (dec * (float)deceleration);
            speed = Mathf.Min(speed, vehicle.maxSpeed);
            Vector3 v = toTarget / dist * speed;
            return v - vehicle.velocity;
        }
        return Vector3.zero;
    }
    //追逐的力（根据逃脱者会达到的位置计算靠近的力）
    public Vector3 Pursuit(Vehicle evader) {
        if(evader == null) {
            return Vector3.zero;
        }
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
        if (pursuer == null) {
            return Vector3.zero;
        }
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
        var right = Vector3.Cross(vehicle.transform.up, vehicle.heading);
        var up = Vector3.Cross(vehicle.heading, right);
        var matrix = new Matrix4x4(right, up, vehicle.heading, vehicle.pos);
        matrix.m33 = 1.0f;
        Vector3 targetW = matrix * new Vector4(target.x, target.y, target.z, 1.0f);
        //同一坐标系下计算偏移力
        Vector3 f = targetW - vehicle.pos;
        return f;
    }

    public Vector3 ObstacleAvoidance(List<BaseEntity> obstacles) {
        detectBoxLength = (1.0f + vehicle.Speed() / vehicle.maxSpeed) * GameConfig.MinDetectionBoxLength;
        //标记范围内的障碍物
        vehicle.world.TagObstaclesWithinViewRange(vehicle, detectBoxLength);
        //计算世界坐标到vehicle局部坐标的矩阵
        var right = Vector3.Cross(vehicle.transform.up, vehicle.heading);
        var up = Vector3.Cross(vehicle.heading, right);
        var worldMatrix = new Matrix4x4(right, up, vehicle.heading, vehicle.pos);
        var toLocalMatrix = worldMatrix.transpose;
        toLocalMatrix.m33 = 1.0f;
        //遍历
        float disClosest = 99999999.0f;
        BaseEntity obCloset = null;
        Vector3 localPosOb = new Vector3();
        foreach (var obstacle in obstacles) {
            //找到标记的障碍物
            if (!obstacle.tagged) {
                continue;
            }
            Vector3 obPos = toLocalMatrix * obstacle.pos;
            //在Vehicle后面不考虑
            if (obPos.x < 0) {
                continue;
            }
            float expandR = obstacle.BRadius() + vehicle.BRadius();
            float y = obPos.z, x = obPos.x;
            //Vehicle检测盒与障碍物不相交则不考虑
            if (expandR < Mathf.Abs(y)) {
                continue;
            }
            //以X轴上的点为参考，确定检测盒与障碍物相交最近的点，借此确认最近的障碍物
            float res = Mathf.Sqrt(expandR * expandR - y * y);
            float dis = x - res;
            if (dis <= 0) {//不能考虑Vehicle后面相关的点
                dis = x + res;
            }
            //找出最近的
            if (dis < disClosest) {
                disClosest = dis;
                obCloset = obstacle;
                localPosOb.Set(x, 0, y);
            }
        }
        //计算躲避障碍的力
        Vector3 force = new Vector3();
        if (null != obCloset) {
            //偏向力
            float multi = (detectBoxLength - localPosOb.x) / detectBoxLength + 1.0f;
            force.z = (obCloset.BRadius() - localPosOb.z) * multi;
            //制动力
            force.x = (obCloset.BRadius() - localPosOb.x) * 0.2f;
        }
        return worldMatrix*force;
    }

    //public Vector3 WallAvoidance(List<Wall2D> obstacles) {
    //    return Vector3.zero;
    //}

    public Vector3 Interpose(Vehicle agentA, Vehicle agentB) {
        return Vector3.zero;
    }

    public Vector3 GetHidingPosition(Vector3 posOb, float radiusOb, Vector3 target) {
        return Vector3.zero;
    }

    public Vector3 Hide(Vehicle target, List<BaseEntity> obstacles) {
        return Vector3.zero;
    }

    public Vector3 FollowPath() {
        return Vector3.zero;
    }

    public Vector3 OffsetPursuit(Vehicle leader, Vector3 offset) {
        return Vector3.zero;
    }

    void TagNeighbors<T, conT>(T entity, conT containerOfEntities, double radius)
        where T:BaseEntity where conT:IEnumerator<T> {

    }

    Vector3 Separation(List<Vehicle> neighbors){
        return Vector3.zero;
    }

    Vector3 Cohesion(List<Vehicle> neighbors) {
        return Vector3.zero;
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
        TargetAgent1 = vehicle;
    }
    public void SetTargetAgent2(Vehicle vehicle) {
        TargetAgent2 = vehicle;
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
