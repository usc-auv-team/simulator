using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoloMotor : MonoBehaviour
{
    public float force = 5f;
    public Motor motor;

    public void Start()
    {
        motor.set_force(force);
        motor.set_dist_cog(transform.position);
        motor.set_motor_direction(transform.up);
    }

    public void FixedUpdate(){
        motor.set_force(force);
        motor.set_dist_cog(transform.position);
        motor.set_motor_direction(transform.up);
    }

    void OnDrawGizmos()
    {
        if (motor.motor_on)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, motor.get_force() / 10f + transform.position);
        }
    }
}
