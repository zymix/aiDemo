using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    float distance = 5f;
    float rotationSpeed = 90f;
    float lastManualRotationTime = 0f;
    Vector3 lastFocusPosition;

    [SerializeField, Range(-89f, 89f)]
    float maxRotateAngleX = 45f, minRotateAngleX = 15f;
    [SerializeField, Min(0f)]
    float focusRadius = 1f;
    [SerializeField, Range(0f, 1f)]
    float focusCentering = 0.5f;
    [SerializeField]
    Transform focus = default;
    [SerializeField, Min(0f)]
    float alignDelay = 5f;
    [SerializeField, Range(0f, 90f)]
    float alignSmoothRange = 45f;

    Camera cam;
    Vector2 orbitAngles;
    Vector3 focusPoint;
    void Awake() {
        cam = GetComponent<Camera>();
        focusPoint = focus.position;
        orbitAngles = new Vector2( (maxRotateAngleX + minRotateAngleX) / 2.0f, 0f);
        transform.localRotation = Quaternion.Euler(orbitAngles);
        lastManualRotationTime = Time.unscaledTime;
    }

    void OnValidate() {
        if (maxRotateAngleX < minRotateAngleX) {
            minRotateAngleX = maxRotateAngleX;
        }
    }

    void LateUpdate() {
        UpdateFocusPoint();
        Quaternion lookRotation;
        if(ManualRotation() || AutoRotation()) {
            ClampRotateAngle();
            lookRotation = Quaternion.Euler(orbitAngles);
        }else{
            lookRotation = transform.localRotation;
        }
        Vector3 lookDirection = lookRotation * Vector3.forward;
        Vector3 lookPosition = focusPoint - lookDirection * distance;
        transform.SetPositionAndRotation(lookPosition, lookRotation);
    }
	void UpdateFocusPoint () {
        lastFocusPosition = focusPoint;
        Vector3 targetPoint = focus.position;
        if (focusRadius > 0f) {
            float distance = Vector3.Distance(targetPoint, focusPoint);
            if(focusRadius < distance) {
                //若前后两帧距离在摄像机容忍关注点偏移的范围之外，则以距离比作插值趋向目标值
                focusPoint = Vector3.Lerp(targetPoint, focusPoint, focusRadius / distance);
            }
            if(distance > 0.01f && focusCentering > 0f) {
                //若前后两帧关注点不在同一个位置，则曲线插值修正距离
                focusPoint = Vector3.Lerp(targetPoint, focusPoint, Mathf.Pow(1f - focusCentering, Time.unscaledDeltaTime));
            }
        } else {
            focusPoint = targetPoint;
        }
	}
    bool AutoRotation() {
        if(alignDelay == 0 || Time.unscaledTime - lastManualRotationTime < alignDelay) {
            return false;
        }
        Vector2 movement = new Vector2(focusPoint.x - lastFocusPosition.x, focusPoint.z - lastFocusPosition.z);
        float movementDeltaSqr = movement.sqrMagnitude;
        if (movementDeltaSqr < 0.00001) {
            return false;
        }
        Vector2 direction = movement / Mathf.Sqrt(movementDeltaSqr);

        float angle = Mathf.Acos(direction.y) * Mathf.Rad2Deg;
        angle = direction.x < 0f ? 360 - angle : angle;
        float rotationChange = alignSmoothRange * Mathf.Min(Time.unscaledDeltaTime, movementDeltaSqr);
        float deltaAbs = Mathf.Abs(Mathf.DeltaAngle(orbitAngles.y, angle));
        if(deltaAbs < alignSmoothRange) {
            rotationChange *= deltaAbs / alignSmoothRange;
        }else if(180f - deltaAbs< alignSmoothRange) {
            rotationChange *= deltaAbs / alignSmoothRange;
        }
        orbitAngles.y = Mathf.MoveTowardsAngle(orbitAngles.y, angle, rotationChange);
        return true;
    }

    bool ManualRotation(){
        Vector2 inputDelta = new Vector2(Input.GetAxis("Vertical Camera"), Input.GetAxis("Horizontal Camera"));
        float e = 0.01f;
        if(Mathf.Abs(inputDelta.x) > e || Mathf.Abs(inputDelta.y)>e) {
            orbitAngles += inputDelta*rotationSpeed*Time.unscaledDeltaTime;
            lastManualRotationTime = Time.unscaledTime;
            return true;
        }
        return false;
    }
    void ClampRotateAngle() {
        orbitAngles.x = Mathf.Clamp(orbitAngles.x, minRotateAngleX, maxRotateAngleX);
        if (orbitAngles.y < 0f) {
            orbitAngles.y += 360f;
        } else if (orbitAngles.y >= 360f) {
            orbitAngles.y -= 360f;
        }
    }
}
