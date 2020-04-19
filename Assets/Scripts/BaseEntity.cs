using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEntity : MonoBehaviour {
    public int id;
    public bool tagged;
    public Transform view;

    public enum EntityType { DefaultEntity };
    public EntityType type = EntityType.DefaultEntity;
    public Vector3 pos {
        set {
            transform.position = value;
        }
        get {
            return transform.position;
        }
    }
    public float BRadius() {

    }
}
