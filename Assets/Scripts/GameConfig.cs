public static class GameConfig {
    public static int NumAgents = 300;
    public static int NumObstacles = 7;
    public static double MinObstacleRadius;
    public static double MaxObstacleRadius;

    //number of horizontal cells used for spatial partitioning
    public static int NumCellsX;
    //number of vertical cells used for spatial partitioning
    public static int NumCellsY;

    //how many samples the smoother will use to average a value
    public static int NumSamplesForSmoothing;

    //used to tweak the combined steering force (simply altering the MaxSteeringForce
    //will NOT work!This tweaker affects all the steering force multipliers
    //too).
    public static double SteeringForceTweaker;

    public static double MaxSteeringForce;
    public static double MaxSpeed;
    public static double VehicleMass;

    public static double VehicleScale;
    public static double MaxTurnRatePerSecond;

    public static double SeparationWeight;
    public static double AlignmentWeight;
    public static double CohesionWeight;
    public static double ObstacleAvoidanceWeight;
    public static double WallAvoidanceWeight;
    public static double WanderWeight;
    public static double SeekWeight;
    public static double FleeWeight;
    public static double ArriveWeight;
    public static double PursuitWeight;
    public static double OffsetPursuitWeight;
    public static double InterposeWeight;
    public static double HideWeight;
    public static double EvadeWeight;
    public static double FollowPathWeight;

    //how close a neighbour must be before an agent perceives it (considers it
    //to be within its neighborhood)
    public static double ViewDistance;

    //used in obstacle avoidance
    public static double MinDetectionBoxLength;

    //used in wall avoidance
    public static double WallDetectionFeelerLength;

    //these are the probabilities that a steering behavior will be used
    //when the prioritized dither calculate method is used
    public static double prWallAvoidance;
    public static double prObstacleAvoidance;
    public static double prSeparation;
    public static double prAlignment;
    public static double prCohesion;
    public static double prWander;
    public static double prSeek;
    public static double prFlee;
    public static double prEvade;
    public static double prHide;
    public static double prArrive;
}
