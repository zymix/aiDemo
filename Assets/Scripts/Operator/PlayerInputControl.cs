using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputControl : MonoBehaviour{
    [SerializeField]
    Transform playerInputSpace = default;
    ActorAnimateControl aac;
    Rigidbody body;
    public int speed = 0;
    public int unit = 100;
    public int jumpSpeed = 300;
    const float gravity = -9.8f;
    private float curUpSpeed = 0.0f;
    private bool bJumping = false;
    private void Start() {
        aac = GetComponent<ActorAnimateControl>();
        body = GetComponent<Rigidbody>();
    }

    void Update()
    {
        InputMove();
    }

    void FixedUpdate() {
        JumpUp();
    }

    void InputMove(){
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        Vector3 dir = new Vector3 (x, 0f ,y);
        if (playerInputSpace) {
            Vector3 forward = playerInputSpace.forward;
            Vector3 right = playerInputSpace.right;
            right.y = forward.y = 0f;
            dir = dir.x * right.normalized + dir.z * forward.normalized;
        }
        bool isRun = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);        
        if (!bJumping && Input.GetKeyDown(KeyCode.Space)) {
            bJumping = true;
            curUpSpeed = jumpSpeed/unit;
            aac.Jump();
        }
        if(Mathf.Abs(y)> 0.01f || Mathf.Abs(x) > 0.01f) {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir, transform.up), Time.deltaTime * 10f);
            speed = isRun?350:150; //同时下shit键跑或单纯行走
            aac.SetMoveSpeed(speed);
        } else if (speed != 0) {
            speed = 0;
            aac.SetMoveSpeed(speed);
        }
        transform.position += dir * (speed / unit * Time.deltaTime);
    }

    void JumpUp(){
        if(!bJumping){
            return;
        }
        curUpSpeed = curUpSpeed + gravity * Time.deltaTime;
        Vector3 newPos = transform.position + curUpSpeed * Time.deltaTime *transform.up;
        if(newPos.y<=0.0f){ //这里先临时这么写吧，实际上应该以所在地块的y值为对比基准
            bJumping = false;
            newPos.y = 0;
            curUpSpeed = 0;
        }
        transform.position = newPos;
    }
}
