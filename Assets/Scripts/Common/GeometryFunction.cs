using UnityEngine;
public static class GeometryFunctions{
    public static bool LineIntersection2D(in Vector3 A, in Vector3 B, in Vector3 C, in Vector3 D, ref float dist, ref Vector3 point){
        float rTop = (A.z-C.z)*(D.x-C.x)-(A.x-C.x)*(D.z-C.z);
        float rBot = (B.x-A.x)*(D.z-C.z)-(B.z-A.z)*(D.x-C.x);

        float sTop = (A.z-C.z)*(B.x-A.x)-(A.x-C.x)*(B.z-A.z);
        float sBot = (B.x-A.x)*(D.z-C.z)-(B.z-A.z)*(D.x-C.x);

        if (rBot == 0 || sBot == 0){
            return false;
        }

        float r = rTop/rBot; //t
        float s = sTop/sBot; //u

        if( (r > 0) && (r < 1) && (s > 0) && (s < 1) ){
            float x = (A.x-B.x), z = (A.z-B.z);
            dist = Mathf.Sqrt(x*x+z*z) * r;
            point = A + r * (B - A);
            return true;
        }
        return false;
    }
}