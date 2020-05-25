using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputControl : MonoBehaviour{
    ActorAnimateControl aac;
    public int steer = 36000;
    public int speed = 0;
    public int unit = 100;
    private void Start() {
        aac = GetComponent<ActorAnimateControl>();
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        Vector3 dir = new Vector3(x, 0, y);
        
        if (!Mathf.Approximately(y, 0.0f)) {
            if (Input.GetKey(KeyCode.LeftShift)) {
                speed = 350; //同时下shit键跑
            } else {
                speed = 150; //走
            }
            aac.SetMoveSpeed(speed);
        } else if (speed != 0) {
            speed = 0;
            aac.SetMoveSpeed(speed);
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            //speed = 0;
            aac.Jump();
        }
        if(!Mathf.Approximately(y, 0.0f) || !Mathf.Approximately(x, 0.0f)) {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir, transform.up), Time.deltaTime * 10f);
        }
        transform.position += transform.forward * (speed / unit * Time.deltaTime);
    }
}
