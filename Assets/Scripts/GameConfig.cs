public static class GameConfig {
    public static float MaxFloat = 9999999.0f;
    public static int NumAgents = 300;
    public static int NumObstacles = 7;
    public static float MinObstacleRadius = 1.0f;
    public static float MaxObstacleRadius = 3.0f;

    //number of horizontal cells used for spatial partitioning
    public static int NumCellsX = 7;
    //number of vertical cells used for spatial partitioning
    public static int NumCellsY =7;

    //how many samples the smoother will use to average a value
    public static int NumSamplesForSmoothing;

    //used to tweak the combined steering force (simply altering the MaxSteeringForce
    //will NOT work!This tweaker affects all the steering force multipliers
    //too).
    public static float SteeringForceTweaker = 300.0f;

    public static float MaxSteeringForce = 150.0f;
    public static float MaxSpeed = 10.0f;
    public static float VehicleMass = 10.0f;

    public static float VehicleScale= 3.0f;
    public static float MaxTurnRatePerSecond = 2.0f;

    public static float SeparationWeight = 1.0f;
    public static float AlignmentWeight = 1.0f;
    public static float CohesionWeight = 2.0f;
    public static float ObstacleAvoidanceWeight = 10.0f;
    public static float WallAvoidanceWeight = 10.0f;
    public static float WanderWeight = 1.0f;
    public static float SeekWeight = 1.0f;
    public static float FleeWeight = 1.0f;
    public static float ArriveWeight = 1.0f;
    public static float PursuitWeight = 1.0f;
    public static float OffsetPursuitWeight = 1.0f;
    public static float InterposeWeight = 1.0f;
    public static float HideWeight = 1.0f;
    public static float EvadeWeight = 0.01f;
    public static float FollowPathWeight = 0.05f;

    //how close a neighbour must be before an agent perceives it (considers it
    //to be within its neighborhood)
    public static float ViewDistance = 5.0f;

    //used in obstacle avoidance
    public static float MinDetectionBoxLength = 2.0f;

    //used in wall avoidance
    public static float WallDetectionFeelerLength = 3.0f;

    //these are the probabilities that a steering behavior will be used
    //when the prioritized dither calculate method is used
    public static float prWallAvoidance  = 0.5f;
    public static float prObstacleAvoidance  = 0.5f;
    public static float prSeparation  = 0.2f;
    public static float prAlignment  = 0.3f;
    public static float prCohesion  = 0.6f;
    public static float prWander  = 0.8f;
    public static float prSeek  = 0.8f;
    public static float prFlee  = 0.6f;
    public static float prEvade  = 1.0f;
    public static float prHide  = 0.8f;
    public static float prArrive  = 0.5f;

    public static float WanderDist = 2.0f;
    public static float WanderJitterPerSec = 80.0f;
    public static float WanderRad = 1.2f;
    public static float WaypointSeekDist = 20.0f;
}
