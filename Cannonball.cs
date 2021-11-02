using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Serialization;

public class Cannonball : MonoBehaviour
{
    private float gravity = 0.0004f;
    public float initAngle;
    public float horizontalVelocity = 0.04f;
    public float verticalVelocity;
    
    //store position of vector 5 seconds ago
    private Vector3 oldPosition; 
    
    void FixedUpdate()
    {
        //vertical velocity decreases due to gravity every update
        verticalVelocity -= gravity;
        
        //convert Unity storage of angles to familiar radians
        float angle = (float) ((initAngle + 90) % 360 * Math.PI / 180f);
        
        //direction travelling computed with cos and sin of direction angle
        transform.Translate(horizontalVelocity * Mathf.Cos(angle), verticalVelocity * Mathf.Sin(angle), 0f);

        //if to left or right of screen, destroy
        if ( Mathf.Abs(transform.position.x) > 9)
            Destroy(this.gameObject);

        //send out 4 ray casts every update, distance of cast is radius of sphere/cannonball
        var pos = transform.position;
        RaycastHit2D castUp = Physics2D.Raycast(pos, Vector2.up, 0.25f);
        RaycastHit2D castDown = Physics2D.Raycast(pos, Vector2.down, 0.25f);
        RaycastHit2D castLeft = Physics2D.Raycast(pos, Vector2.left, 0.25f);
        RaycastHit2D castRight = Physics2D.Raycast(pos, Vector2.right, 0.25f);

        RaycastHit2D[] rays = {castUp, castDown, castLeft, castRight};
        
        //check if collision is null, else check name of collider
        for (int i = 0; i < 4; i++)
        {
            if (rays[i].collider != null)
            {
                if (rays[i].collider.name == "Ground")
                {
                    
                    // Debug.Log("hit Ground");
                    
                    //bounce if ground is hit
                    verticalVelocity *= -0.8f;
                    horizontalVelocity *= -0.8f;
                }

                if (rays[i].collider.name == "Water")
                {
                    // Debug.Log("hit Water");
                    
                    //destroy if water is hit
                    Destroy(this.gameObject);
                }
            }
        }
    }
}
    
