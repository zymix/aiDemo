public class Obstacle : BaseEntity{

    void Start(){
        if (!world) {
            return;
        }
        setView("Models/Objects/Obstacle");
        //GameObject.Destroy(res);
    }
}
