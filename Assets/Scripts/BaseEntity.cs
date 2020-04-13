using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEntity : MonoBehaviour {
    public int id;
    public bool tagged;
    public Transform view;

    public enum EntityType { DefaultEntity };
    public EntityType type = EntityType.DefaultEntity;
}
