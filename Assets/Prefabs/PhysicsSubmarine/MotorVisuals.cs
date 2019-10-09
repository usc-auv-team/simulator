using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotorVisuals : MonoBehaviour
{
    public SoloMotor soloMotor;
    public GameObject rotor = null;

    private void Start()
    {
        //soloMotor = GetComponent<SoloMotor>();
        //rotor = transform.Find("USCTurtle Assembly.001").gameObject;
    }

    void Update()
    {
        if (soloMotor.motor.motor_on && rotor != null)
        {
            Debug.Log("rotating");
            rotor.transform.Rotate(transform.right, 150f * Time.deltaTime, Space.Self);
        }
    }
}
