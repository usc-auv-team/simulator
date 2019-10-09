using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Motor// : MonoBehaviour
{
    [Header("Motor Settings")]
    [SerializeField]
    private Vector3 motor_direction, dist_cog; //dist cog is force center to center of gravity
    [SerializeField]
    private float force;
    [SerializeField]
    public bool motor_on;
    private Vector3 global_cog = Vector3.zero;

    public void set_global_cog(Vector3 real_cog) { global_cog = real_cog; }
    public Vector3 get_force() { return motor_direction * force * ((motor_on) ? (1) : (0)); }
    public void set_force(float inForce) { force = inForce; }
    public void set_motor_direction(Vector3 inVec) { motor_direction = inVec; }
    public void set_dist_cog(Vector3 inVec) { dist_cog = inVec - global_cog; }
    public Vector3 get_torque(){ return Vector3.Cross(dist_cog, get_force()); }

    public Motor()
    {
        motor_direction = Vector3.zero;
        force = 0f;
        dist_cog = Vector3.zero;
        motor_on = true;
    }

    public Motor(Vector3 _motor_direction, float _force, Vector3 _dist_cog,
                 bool _motor_on)
    {
        motor_direction = _motor_direction;
        dist_cog = _dist_cog;
        force = _force;
        motor_on = _motor_on;
    }
}

public class PhysicsSim : MonoBehaviour
{
    private Rigidbody rb;
    private float drag;
    private bool motor_changed = true;
    private Vector3 force, torque;
    public SoloMotor[] solo_motors;
    public Motor[] motors;
    private Vector3 center_of_gravity = Vector3.zero;
    //public Vector3 center_of_buoyancy_disp = Vector3.zero;
    private Vector3 center_of_buoyancy = Vector3.zero;
    //cog is just 0,0,0 for now
    //later we need to make a formula to calculate it based on mass of different
    //parts of the sub
    public float weight = 5.0f;
    public float inertia = 50.0f;
    public float x_thresh = 3f, z_thresh = 3f;


	public void Start()
	{
        solo_motors = GetComponentsInChildren<SoloMotor>();
        motors = new Motor[solo_motors.Length];
        for(int i = 0; i < solo_motors.Length; i++){
        	motors[i] = solo_motors[i].motor;
            motors[i].set_global_cog(center_of_gravity);
        }
        rb = GetComponent<Rigidbody>();
	}

	//Calculate sum of forces and sum of torques
	public void FixedUpdate()
	{
        //Only need to recalculate when motor updates
        for (int i = 0; i < solo_motors.Length; i++)
        {
            motors[i].set_global_cog(transform.position);
        }

        //Calculate force and torque of motors
        if (motor_changed){
            update_sums(motors, ref force, ref torque);
        }

        //Calculate center of bouyance and center of gravity righting torque
        Vector3 cob1 = center_of_gravity + transform.up, cob2 = center_of_gravity - 0.5f * transform.up;
        Vector3 f1 = Vector3.up * 20, f2 = Vector3.up * 5;
        torque += Vector3.Cross(cob1, f1);
        torque += Vector3.Cross(cob2, f2);

        //there is buoyant force underneath the cog and above the cog both applying force and torque
        //buoyant force should be equal to gravity even thought it realistically isnt exactly the same
        //buoyant force above the cog should be greater than the buoyant force below the COG.


        rb.AddForce(force);
        rb.AddTorque(torque);
	}

    //Takes in a list of motors, and 2 vector3s representing acceleration and torque
    public void update_sums(Motor[] motors, ref Vector3 force, 
                               ref Vector3 torque){
        force = Vector3.zero;
        torque = Vector3.zero;
        foreach(Motor m in motors){
            force += m.get_force();
            torque += m.get_torque();
        }
        force += Vector3.down * weight;
    }

    public float calculate_drag(Quaternion quaternion){
        //WRITE CODE HERE THAT CALCULATES DRAG BASED ON THE ROTATION QUATERNION
        //OF THE SUBMARINE.

        //QUESTIONS: QUATERNION OR VECTOR3 REPRESENTING ROTATION
        //PROS CONS
        return 1f;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, force/10f+ transform.position);

    }
}
