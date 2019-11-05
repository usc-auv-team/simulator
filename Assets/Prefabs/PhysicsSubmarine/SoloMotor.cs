using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoloMotor : MonoBehaviour
{
    [SerializeField]
    private float force = 5f;
    public Motor motor;

    public void Start()
    {
        motor.SetForce(force);
        motor.SetDistCog(transform.position);
        motor.SetMotorDirection(transform.up);
    }

    public void FixedUpdate(){
        motor.SetForce(force);
        motor.SetDistCog(transform.position);
        motor.SetMotorDirection(transform.up);
    }

    public void SwitchOnOff() {
        motor.motorOn = !motor.motorOn;
    }

    void OnDrawGizmos()
    {
        if (motor.motorOn)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, motor.GetForce() / 10f + transform.position);
        }
    }
}
