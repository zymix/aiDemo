using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEntity : MonoBehaviour {
    public int id;
    private bool _tagged;
    public World world;
    protected Bounds _bounds;
    private Transform _view;
    public Transform view{
        get{
            return _view;
        }
        set{
            _view = value;
            _bounds = _view.GetComponent<Collider>().bounds;
        }
    }

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
        float r = 0.0f;
        if(null != _bounds){
            r = _bounds.size.x;
        }
        return r;
    }

    public bool IsTagged(){
        return _tagged;
    }
    public void Tagged(){
        _tagged = true;
    }
    
    public void UnTagged(){
        _tagged = false;
    }

    public void setView(string path){
        GameObject res = ResManager.LoadAsset<GameObject>(path);
        view = Instantiate<GameObject>(res, transform).transform;
    }
    public static T Create<T>(World world) where T:BaseEntity{
        GameObject obj = new GameObject();
        T comp = obj.AddComponent<T>();
        comp.world = world;
        return comp;
    }
}
