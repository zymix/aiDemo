using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Steering{
    public enum Deceleration { slow = 3, normal = 2, fast = 1 };
    private Vehicle vehicle;
    private Vector3 steeringForce;
    private Vector3 wanderTarget;
    //行动目标
    private Vehicle _targetAgent1;
    private Vehicle _targetAgent2;
    private Vector3 _offsetPursuit;
    public Deceleration deceleration;

    public Steering(Vehicle vehicle) {
        this.vehicle = vehicle;
        steeringForce = new Vector3(0f, 0f, 0f);
        wanderTarget = new Vector3(0f, 0f, 0f);
        deceleration = Deceleration.fast;
        initSteerParam();
        ObstacleAvoidanceOn();
        WallAvoidanceOn();
    }

    //计算合力
    public Vector3 Calculate() {
        steeringForce.Set(0f, 0f, 0f);
        CalculatePrioritized();
        return steeringForce;
    }

    private void CalculatePrioritized() {
        if (On(SteeringType.wallAvoidance)){
            Vector3 force = WallAvoidance(vehicle.world.walls) * weightWallAvoidance;
            if(!AccumulateForce(ref steeringForce, force)) {return;}
        }
        
        if (On(SteeringType.obstacleAvoidance)){
            Vector3 force = ObstacleAvoidance(vehicle.world.obstacles) * weightObstacleAvoidance;
            if(!AccumulateForce(ref steeringForce, force)) {return;}
        }

        if (On(SteeringType.evade)){
            Vector3 force = Evade(_targetAgent1) * weightEvade;
            if(!AccumulateForce(ref steeringForce, force)) {return;}
        }
        if (On(SteeringType.flee)){
            Vector3 force = Flee(vehicle.world.crosshair) * weightFlee;
            if(!AccumulateForce(ref steeringForce, force)) {return;}
        }

        if (On(SteeringType.seek)){
            Vector3 force = Seek(vehicle.world.crosshair) * weightSeek;
            if(!AccumulateForce(ref steeringForce, force)) {return;}
        }
        if (On(SteeringType.arrive)){
            Vector3 force = Arrive(vehicle.world.crosshair, deceleration) * weightArrive;
            if(!AccumulateForce(ref steeringForce, force)) {return;}
        }
        if (On(SteeringType.wander)){
            Vector3 force = Wander() * weightWander;
            if(!AccumulateForce(ref steeringForce, force)) {return;}
        }
        if (On(SteeringType.pursuit)){
            Vector3 force = Pursuit(_targetAgent1) * weightPursuit;
            if(!AccumulateForce(ref steeringForce, force)) {return;}
        }
        if (On(SteeringType.offsetPursuit)) {
            Vector3 force = OffsetPursuit(_targetAgent1, _offsetPursuit) * weightOffsetPursuit;
            if (!AccumulateForce(ref steeringForce, force)) { return; }
        }
    }

    private bool AccumulateForce(ref Vector3 runingTot, in Vector3 forceToAdd){
        float curMagnitude = runingTot.magnitude;
        float remainForce = vehicle.maxForce - curMagnitude;
        if(remainForce<=0.0){
            return false;
        }
        float addMagnitude = forceToAdd.magnitude;
        if(addMagnitude <= remainForce){
            runingTot.Set(
                runingTot.x + forceToAdd.x, 
                runingTot.y + forceToAdd.y, 
                runingTot.z + forceToAdd.z);
        }else{
            runingTot.Set(
                runingTot.x + forceToAdd.x / addMagnitude * remainForce,
                runingTot.y + forceToAdd.y / addMagnitude * remainForce,
                runingTot.z + forceToAdd.z / addMagnitude * remainForce);
        }
        return true;
    }

    //靠近的力（最大速度）
    public Vector3 Seek(in Vector3 target) {
        Vector3 desiredV = (target - vehicle.pos).normalized * vehicle.maxSpeed;
        return desiredV - vehicle.velocity;
    }

    //离开的力（最大速度）
    public Vector3 Flee(in Vector3 target) {
        Vector3 v = vehicle.pos - target;
        //加入触发范围判断
        //if (v.sqrMagnitude > 10000f) {
        //    return Vector3.zero;
        //}
        Vector3 desiredV = v.normalized * vehicle.maxSpeed;
        return desiredV - vehicle.velocity;
    }

    //抵达的力（根据距离变换）
    public Vector3 Arrive(in Vector3 target,  Deceleration deceleration) {
        Vector3 toTarget = target - vehicle.pos;
        float dist = toTarget.magnitude;
        if (dist > 0.00001f) {
            float toTime = 1.0f * (int)deceleration;
            float speed = dist / toTime;
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
        // var right = Vector3.Cross(vehicle.transform.up, vehicle.heading);
        // var up = Vector3.Cross(vehicle.heading, right);
        // var matrix = new Matrix4x4(right, up, vehicle.heading, -vehicle.pos);
        // matrix = matrix.transpose;
        // matrix.m33 = 1.0f;
        var matrix = vehicle.transform.localToWorldMatrix;
        Vector3 targetW = matrix.MultiplyPoint3x4(target);
        //同一坐标系下计算偏移力
        Vector3 f = targetW - vehicle.pos;
        return f;
    }

    public Vector3 ObstacleAvoidance(List<Obstacle> obstacles) {
        _detectBoxLength = (1.0f + vehicle.Speed() / vehicle.maxSpeed) * GameConfig.MinDetectionBoxLength;
        //标记范围内的障碍物
        vehicle.world.TagObstaclesWithinViewRange(vehicle, _detectBoxLength);
        //计算世界坐标到vehicle局部坐标的矩阵
        var right = Vector3.Cross(vehicle.transform.up, vehicle.heading);
        var up = Vector3.Cross(vehicle.heading, right);

        var worldMatrix = vehicle.transform.localToWorldMatrix;
        var toLocalMatrix = vehicle.transform.worldToLocalMatrix;
        //遍历
        float disClosest =  GameConfig.MaxFloat;
        BaseEntity obCloset = null;
        Vector3 localPosOb = new Vector3();
        foreach (var obstacle in obstacles) {
            //找到标记的障碍物
            if (!obstacle.IsTagged()) {
                continue;
            }
            Vector3 wobPos = obstacle.pos;
            Vector4 obPos = toLocalMatrix.MultiplyPoint3x4(wobPos);
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
            float multi = (_detectBoxLength - localPosOb.x) / _detectBoxLength + 1.0f;
            force.z = (obCloset.BRadius() - localPosOb.z) * multi;
            //制动力
            force.x = (obCloset.BRadius() - localPosOb.x) * 0.2f;
            force = worldMatrix.MultiplyPoint3x4(force);
        }
        return force;
    }

    public Vector3 WallAvoidance(List<Wall> walls) {
        List<Vector3> detectLines = _createDetectLines();
        float disToThis = GameConfig.MaxFloat;
        float disToClosest = GameConfig.MaxFloat;
        int closestWallIndx = -1;
        int closestDetectLine = -1;
        
        Vector3 closestPoint = new Vector3();
        Vector3 point = new Vector3();
        Vector3 force = new Vector3();
        for(int i = 0;i<detectLines.Count; ++i){
            Vector3 dl = detectLines[i];
            //Debug.LogFormat("{0}->{1}",vehicle.pos, dl);
            Debug.DrawLine (vehicle.pos, dl, Color.red);
            for(int j = 0;j<walls.Count; ++j){
                bool isIntersect = GeometryFunctions.LineIntersection2D(
                    vehicle.pos, dl, walls[j].from, walls[j].to, ref disToThis, ref point
                );
                if(isIntersect && disToClosest>disToThis){
                    disToClosest = disToThis;
                    closestWallIndx = j;
                    closestDetectLine = i;
                    closestPoint = point;
                }
            }
        }
        if(closestWallIndx>=0){
                float mag = (detectLines[closestDetectLine] - closestPoint).sqrMagnitude;
                force.Set(
                    walls[closestWallIndx].normal.x * mag,
                    walls[closestWallIndx].normal.y * mag,
                    walls[closestWallIndx].normal.z * mag
                );
            }
        return force;
    }

    private List<Vector3> _createDetectLines(){
         float scaleLength = wallDetectionFeelerLength;

        List <Vector3> dlines = new List<Vector3>();
        dlines.Add(vehicle.pos + vehicle.heading * scaleLength);

        var temp = Quaternion.AngleAxis(50, vehicle.transform.up)*vehicle.heading;
        dlines.Add(vehicle.pos + temp* scaleLength * 0.5f);
        
        temp = Quaternion.AngleAxis(-50, vehicle.transform.up)*vehicle.heading;
        dlines.Add(vehicle.pos + temp * scaleLength * 0.5f);
        return dlines;
    }
    
    public Vector3 Interpose(Vehicle agentA, Vehicle agentB) {
        Vector3 midPos = (agentA.pos + agentB.pos)*0.5f;
        float toTime = Vector3.Distance(midPos, vehicle.pos) / vehicle.maxSpeed;
        midPos = (agentA.pos+agentA.velocity*toTime + agentB.pos+agentB.velocity*toTime)*0.5f;
        return Arrive(midPos, Deceleration.fast);
    }

    public Vector3 GetHidingPosition(in Vector3 posOb, float radiusOb, in Vector3 posHunter) {
        float distanceFormBoundary = 30.0f;
        float distance = radiusOb + distanceFormBoundary;
        Vector3 toOb = Vector3.Normalize(posOb - posHunter);
        return posOb + toOb * distance;
    }

    public Vector3 Hide(Vehicle target, List<BaseEntity> obstacles) {
        Vector3 bestHidePos = new Vector3();
        float disToClosest = GameConfig.MaxFloat;
        BaseEntity closeOb = null;
        foreach(BaseEntity obstacle in obstacles){
            Vector3 hidePos = GetHidingPosition(obstacle.pos, obstacle.BRadius(), target.pos);
            float x = hidePos.x - vehicle.pos.x;
            float y = hidePos.y - vehicle.pos.y;
            float sqDist = x*x+y*y;
            if(disToClosest > sqDist){
                disToClosest = sqDist;
                bestHidePos.Set(hidePos.x, hidePos.y, hidePos.z);
                closeOb = obstacle;
            }
        }
        if(null == closeOb){
            return Evade(target);
        }
        return Arrive(bestHidePos, Deceleration.fast);
    }

    public Vector3 FollowPath() {
        return Vector3.zero;
    }

    public Vector3 OffsetPursuit(Vehicle leader, Vector3 offset) {
        Vector3 worldOffset = leader.transform.localToWorldMatrix.MultiplyPoint3x4(offset);
        Vector3 dist = worldOffset - vehicle.pos;
        float toTime = dist.magnitude / (leader.Speed() + vehicle.maxSpeed);
        return Arrive(worldOffset + leader.velocity*toTime, Deceleration.fast);
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
        _targetAgent1 = vehicle;
    }
    public void SetTargetAgent2(Vehicle vehicle) {
        _targetAgent2 = vehicle;
    }
}
