using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class MovingEntity : BaseEntity {
    //速度
    public Vector3 velocity;
    //朝向
    private Vector3 _heading;
    public Vector3 heading {
        set{
            Assert.AreApproximatelyEqual(value.sqrMagnitude, 1.0f,"must normalize");
            _heading = value;
            side = Vector2.Perpendicular(_heading);
        }
        get {
            return _heading;
        }
    }
    public Vector3 pos {
        set {
            transform.position = value;
        }
        get {
            return transform.position;
        }
    }
    public Vector3 side { set; get; }
    public float mass { protected set; get; }
    //最大速度
    public float maxSpeed;
    public float maxForce;
    public float maxTurnRate;

   public bool IsSpeedMaxedOut() {
        return maxSpeed * maxSpeed >= velocity.sqrMagnitude;
   }
   
    public float Speed() {
        return velocity.magnitude;
    }

    public float SpeedSq() {
        return velocity.sqrMagnitude;
    }
    //旋转Entity的朝向，若当前朝向跟target方向一致时返回true，否则false
    public bool RotateHeadingToFacePosition(Vector3 target) {
        Vector3 toTarget = (target - (Vector3)transform.position).normalized;
        float angle = Mathf.Acos(Vector3.Dot(toTarget, heading));

        if (angle < 0.0001) {
            return true;
        }
        if (angle > maxTurnRate) {
            angle = maxTurnRate;
        }
        var r = Quaternion.AngleAxis(angle, transform.up);
        heading = r * heading;
        velocity = r * velocity;
        side = Vector2.Perpendicular(heading);
        return false;
    }
}
