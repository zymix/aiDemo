public static class GameConfig {
    public static int NumAgents = 300;
    public static int NumObstacles = 7;
    public static float MinObstacleRadius = 10.0f;
    public static float MaxObstacleRadius = 30.0f;

    //number of horizontal cells used for spatial partitioning
    public static int NumCellsX;
    //number of vertical cells used for spatial partitioning
    public static int NumCellsY;

    //how many samples the smoother will use to average a value
    public static int NumSamplesForSmoothing;

    //used to tweak the combined steering force (simply altering the MaxSteeringForce
    //will NOT work!This tweaker affects all the steering force multipliers
    //too).
    public static float SteeringForceTweaker;

    public static float MaxSteeringForce;
    public static float MaxSpeed;
    public static float VehicleMass;

    public static float VehicleScale;
    public static float MaxTurnRatePerSecond;

    public static float SeparationWeight;
    public static float AlignmentWeight;
    public static float CohesionWeight;
    public static float ObstacleAvoidanceWeight;
    public static float WallAvoidanceWeight;
    public static float WanderWeight;
    public static float SeekWeight;
    public static float FleeWeight;
    public static float ArriveWeight;
    public static float PursuitWeight;
    public static float OffsetPursuitWeight;
    public static float InterposeWeight;
    public static float HideWeight;
    public static float EvadeWeight;
    public static float FollowPathWeight;

    //how close a neighbour must be before an agent perceives it (considers it
    //to be within its neighborhood)
    public static float ViewDistance;

    //used in obstacle avoidance
    public static float MinDetectionBoxLength = 40.0f;

    //used in wall avoidance
    public static float WallDetectionFeelerLength = 40.0f;

    //these are the probabilities that a steering behavior will be used
    //when the prioritized dither calculate method is used
    public static float prWallAvoidance;
    public static float prObstacleAvoidance;
    public static float prSeparation;
    public static float prAlignment;
    public static float prCohesion;
    public static float prWander;
    public static float prSeek;
    public static float prFlee;
    public static float prEvade;
    public static float prHide;
    public static float prArrive;
}
