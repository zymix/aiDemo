using UnityEngine;
public static class ExtendFunctions{
    public static Vector3 Truncate(this Vector3 vector, float max){
        if(vector.sqrMagnitude<max*max){
            vector.Normalize();
            vector*=max;
        }
        return vector;
    }
}