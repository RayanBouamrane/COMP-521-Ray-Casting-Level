using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    
    public GameObject cannonBall;
    
    //bool for which cannon is being controlled
    public bool isSelected;

    public int minAngle;
    public int maxAngle;
    
    //sensitivity when moving cannons
    public float sens = 1.5f;

    public float muzzleVelocity = 0.08f;
    
    public Cannonball c;
    void FixedUpdate()
    {
        if (isSelected)
        {
            var angle = transform.localRotation.eulerAngles.z;
            // if(Input.GetKey("f")) { Debug.Log(angle); }

            //quaternions converted to euler angles to allow simple trig calculations
            if (Input.GetKey(KeyCode.DownArrow) && ((angle - sens >= minAngle) )) 
            {
                transform.rotation = Quaternion.AngleAxis(transform.localRotation.eulerAngles.z - sens, Vector3.forward);
            }

            if (Input.GetKey(KeyCode.UpArrow) && ((angle + sens <= maxAngle))) 
            {
                transform.rotation = Quaternion.AngleAxis(transform.rotation.eulerAngles.z + sens, Vector3.forward);
            }

            //create cannonball when space pressed
            if (Input.GetKeyDown(KeyCode.Space))
            {
                var t = transform;
                c = (Instantiate(cannonBall, t.position, Quaternion.identity)).GetComponent<Cannonball>();
                c.initAngle = transform.localRotation.eulerAngles.z;
                c.verticalVelocity = muzzleVelocity;
            }

            //change velocity of cannon when pressed
            if (Input.GetKey(KeyCode.LeftArrow)) muzzleVelocity -= 0.001f;
            if (Input.GetKey(KeyCode.RightArrow)) muzzleVelocity += 0.001f;
        }
    }
}
