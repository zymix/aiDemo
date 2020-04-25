using UnityEngine;
public enum SteeringType{
    none = 0x00000,
    seek = 0x00002,
    flee = 0x00004,
    arrive = 0x00008,
    wander = 0x00010,
    cohesion = 0x00020,
    separation = 0x00040,
    allignment = 0x00080,
    obstacleAvoidance = 0x00100,
    wallAvoidance = 0x00200,
    followPath = 0x00400,
    pursuit = 0x00800,
    evade = 0x01000,
    interpose = 0x02000,
    hide = 0x04000,
    flock = 0x08000,
    offsetPursuit = 0x10000
}
public partial class Steering {
    private SteeringType stFlag; //保存所有行为
    //徘徊
    private float wanderRadius;
    private float wanderDistance;
    private float wanderJitter;
    public float weightSeparation;
    public float weightCohesion;
    public float weightAlignment;
    public float weightWander;
    public float weightObstacleAvoidance;
    public float weightWallAvoidance;
    public float weightSeek;
    public float weightFlee;
    public float weightArrive;
    public float weightPursuit;
    public float weightOffsetPursuit;
    public float weightInterpose;
    public float weightHide;
    public float weightEvade;
    public float weightFollowPath;
    public float wallDetectionFeelerLength;
    public float wiewDistance;
    //public Path path;
    public float waypointSeekDistSq;
    Vector3     offset;
    //检测盒（AABB）长度
    private float _detectBoxLength;
    private void initSteerParam(){
       _detectBoxLength = GameConfig.MinDetectionBoxLength;
       weightCohesion = GameConfig.CohesionWeight;
       weightAlignment = GameConfig.AlignmentWeight;
       weightSeparation = GameConfig.SeparationWeight;
       weightObstacleAvoidance = GameConfig.ObstacleAvoidanceWeight;
       weightWander = GameConfig.WanderWeight;
       weightWallAvoidance = GameConfig.WallAvoidanceWeight;
       //viewDistance = GameConfig.ViewDistance;
       wallDetectionFeelerLength = GameConfig.WallDetectionFeelerLength;
       //deceleration = normal,
       wanderDistance = GameConfig.WanderDist;
       wanderJitter = GameConfig.WanderJitterPerSec;
       wanderRadius = GameConfig.WanderRad;
       waypointSeekDistSq = GameConfig.WaypointSeekDist*GameConfig.WaypointSeekDist;
       weightSeek = GameConfig.SeekWeight;
       weightFlee = GameConfig.FleeWeight;
       weightArrive = GameConfig.ArriveWeight;
       weightPursuit = GameConfig.PursuitWeight;
       weightOffsetPursuit = GameConfig.OffsetPursuitWeight;
       weightInterpose = GameConfig.InterposeWeight;
       weightHide = GameConfig.HideWeight;
       weightEvade = GameConfig.EvadeWeight;
       weightFollowPath = GameConfig.FollowPathWeight;
    }

    public bool On(SteeringType st) {
        return (stFlag & st) == st;
    }
    //开启行为
    public void SeekOn() {stFlag |= SteeringType.seek;}
    public void FleeOn() {stFlag |= SteeringType.flee;}
    public void ArriveOn() {stFlag |= SteeringType.arrive;}
    public void WanderOn() {stFlag |= SteeringType.wander;}
    public void PursuitOn() {stFlag |= SteeringType.pursuit;}
    public void EvadeOn() {stFlag |= SteeringType.evade;}
    public void CohesionOn() {stFlag |= SteeringType.cohesion;}
    public void SeparationOn() {stFlag |= SteeringType.separation;}
    public void AlignmentOn() {stFlag |= SteeringType.allignment;}
    public void WallAvoidanceOn() {stFlag |= SteeringType.wallAvoidance;}
    public void ObstacleAvoidanceOn() {stFlag |= SteeringType.obstacleAvoidance;}
    public void FollowPathOn() {stFlag |= SteeringType.followPath;}
    public void InterposeOn(Vehicle v1, Vehicle v2) {
        stFlag |= SteeringType.interpose;
        _targetAgent1 = v1;
        _targetAgent1 = v2;
    }
    public void HideOn(Vehicle v1) {
        stFlag |= SteeringType.hide;
        _targetAgent1 = v1;
    }
    public void OffsetPursuitOn(Vehicle v1, Vector3 offset) {
        stFlag |= SteeringType.offsetPursuit;
        _targetAgent1 = v1;
        _offsetPursuit = offset;
    }
    public void FlockingOn() { 
        CohesionOn(); 
        AlignmentOn(); 
        SeparationOn(); 
        WanderOn(); 
    }

    //关闭行为
    void FleeOff()  {if(On(SteeringType.flee))   stFlag ^= SteeringType.flee;}
    void SeekOff()  {if(On(SteeringType.seek))   stFlag ^= SteeringType.seek;}
    void ArriveOff(){if(On(SteeringType.arrive)) stFlag ^= SteeringType.arrive;}
    void WanderOff(){if(On(SteeringType.wander)) stFlag ^= SteeringType.wander;}
    void PursuitOff(){if(On(SteeringType.pursuit)) stFlag ^= SteeringType.pursuit;}
    void EvadeOff(){if(On(SteeringType.evade)) stFlag ^= SteeringType.evade;}
    void CohesionOff(){if(On(SteeringType.cohesion)) stFlag ^= SteeringType.cohesion;}
    void SeparationOff(){if(On(SteeringType.separation)) stFlag ^= SteeringType.separation;}
    void AlignmentOff(){if(On(SteeringType.allignment)) stFlag ^= SteeringType.allignment;}
    void ObstacleAvoidanceOff(){if(On(SteeringType.obstacleAvoidance)) stFlag ^= SteeringType.obstacleAvoidance;}
    void WallAvoidanceOff(){if(On(SteeringType.wallAvoidance)) stFlag ^= SteeringType.wallAvoidance;}
    void FollowPathOff(){if(On(SteeringType.followPath)) stFlag ^= SteeringType.followPath;}
    void InterposeOff(){if(On(SteeringType.interpose)) stFlag ^= SteeringType.interpose;}
    void HideOff(){if(On(SteeringType.hide)) stFlag ^= SteeringType.hide;}
    void OffsetPursuitOff(){if(On(SteeringType.offsetPursuit)) stFlag ^= SteeringType.offsetPursuit;}
    void FlockingOff(){
        CohesionOff();
        AlignmentOff();
        SeparationOff();
        WanderOff();
    }
}