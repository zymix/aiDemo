using UnityEngine;

public class ActorAnimateControl : MonoBehaviour{
    private Animator animator;
    private int speedParamID;
    private int jumpParamID;
    private int dieParamID;
    private int idleParamID;

    public int idleStateID;
    public int walkStateID;
    public int runStateID;
    public int jumpStateID;
    public int dieStateID;

    private void Awake() {
        speedParamID = Animator.StringToHash("speed");
        jumpParamID = Animator.StringToHash("jump");
        dieParamID = Animator.StringToHash("die");
        idleParamID = Animator.StringToHash("idle");

        idleStateID = Animator.StringToHash("Idle");
        walkStateID = Animator.StringToHash("Walk");
        runStateID = Animator.StringToHash("Run");
        jumpStateID = Animator.StringToHash("Jump");
        dieStateID = Animator.StringToHash("Die");
    }

    void Start() {
        animator = GetComponentInChildren<Animator>();
    }
    public void Walk() {
        animator.SetInteger(speedParamID, 150);
    }

    public void Run() {
        animator.SetInteger(speedParamID, 350);
    }

    public void Idle() {
        animator.SetInteger(speedParamID, 0);
        animator.SetTrigger(idleParamID);
    }

    public void SetMoveSpeed(int speed) {
        if (speed == 0) {
            Idle();
        }
        animator.SetInteger(speedParamID, speed);
    }

    public void Jump() {
        animator.SetTrigger(jumpParamID);
    }

    public void Die() {
        animator.SetTrigger(dieParamID);
    }

    public bool IsCurrentState(string name) {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName(name);
    }

    public int CurrentStateNameHash() {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.shortNameHash;
    }
}
