using System.Collections.Generic;
using UnityEngine;
public static class EntityFunctions{
    public static void TagNeighbors<T, ConT>(T entity, ConT entityContainer, float radius)
        where T:BaseEntity where ConT:IEnumerable<T>{
        foreach(var target in entityContainer){
            target.UnTagged();
            if(entity == target){
                continue;
            }
            float detectLength = target.BRadius() + radius;
            float dis = (entity.pos-target.pos).sqrMagnitude;
            if(dis < detectLength * detectLength){
                target.Tagged();
            }
        }
    }

    public static void EnforceNonPenetrationConstraint<T, ConT>(T entity, ConT containerOfEntities)
        where T:BaseEntity where ConT: IEnumerable<T> {

        foreach(var ele in containerOfEntities) {
            if(ele == entity) {
                continue;
            }
            Vector3 toE = entity.pos - ele.pos;
            float length = toE.magnitude;
            float over = entity.BRadius() + ele.BRadius() - length;
            if (over >= 0) {
                entity.pos += toE / length * over;
            }
        }
    }
}
