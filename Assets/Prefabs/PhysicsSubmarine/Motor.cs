using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Motor// : MonoBehaviour
{
    [Header("Motor Settings")]
    [SerializeField]
    private Vector3 motorDirection, distCog; //dist cog is force center to center of gravity
    [SerializeField]
    private float force;
    [SerializeField]
    public bool motorOn;
    [SerializeField]
    public float power; //power varies -1 and 1 like the ros does
    private Vector3 globalCog = Vector3.zero;

   
    public void SetGlobalCog(Vector3 realCog) { globalCog = realCog; }
    public Vector3 GetForce() { return motorDirection * power * force * ((motorOn) ? (1) : (0)); }
    public void SetForce(float inForce) { force = inForce; }
    public void SetMotorDirection(Vector3 inVec) { motorDirection = inVec; }
    public Vector3 GetMotorDirection() { return motorDirection; }
    public void SetDistCog(Vector3 inVec) { distCog = inVec - globalCog; }
    public Vector3 GetTorque(){ return Vector3.Cross(distCog, GetForce()); }
    public float GetPower() { return power; }
    public void SetPower(float inPower) { power = Mathf.Clamp(inPower, -1f, 1f); }

    public Motor()
    {
        motorDirection = Vector3.zero;
        force = 0f;
        power = 0f;
        distCog = Vector3.zero;
        motorOn = true;
    }

    public Motor(Vector3 _motorDirection, float _force, Vector3 _distCog,
                 bool _motorOn)
    {
        motorDirection = _motorDirection;
        distCog = _distCog;
        force = _force;
        motorOn = _motorOn;
        power = 1f;
    }
}