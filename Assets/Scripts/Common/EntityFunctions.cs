using System.Collections.Generic;
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
}
