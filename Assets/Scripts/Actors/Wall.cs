using UnityEngine;
public class Wall:BaseEntity{
    public Vector3 from;
    public Vector3 to;
    public Vector3 normal { get; private set; }

    private void Start() {
        //setView("Models/Role/Cube");
        normal = view.right;
        if (_bounds.size.x > _bounds.size.z){
            float dis = normal.z * _bounds.size.z * 0.5f;
            from.Set(transform.position.x - _bounds.size.x*0.5f, transform.position.y, transform.position.z + dis);
            to.Set(transform.position.x + _bounds.size.x*0.5f, transform.position.y, transform.position.z + dis);
        } else{
            float dis = normal.x * _bounds.size.x * 0.5f;
            from.Set(transform.position.x + dis, transform.position.y, transform.position.z - _bounds.size.z*0.5f);
            to.Set(transform.position.x + dis, transform.position.y, transform.position.z + _bounds.size.z*0.5f);
        }
    }
    
    public static Wall Create(GameObject obj, World world){
        Wall comp = obj.AddComponent<Wall>();
        comp.world = world;
        return comp;
    }
}