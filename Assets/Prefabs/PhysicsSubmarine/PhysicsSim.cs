using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsSim : MonoBehaviour
{
    //General Variables
    private Rigidbody rb;
    private bool motorChanged = true;
    private Vector3 force, torque;
    [SerializeField]
    private Vector3 centerOfGravity = Vector3.zero;
    private Vector3 centerOfBouyancy = Vector3.zero;

    //Arrays containing all the motor information
    public SoloMotor[] soloMotors;
    public Motor[] motors;

    //Physics Properties
    [SerializeField]
    private float weight = 20.0f;
    [SerializeField]
    private float inertia = 50.0f;
    [SerializeField]
    private float waterHeight;

    //Todo: Make drag meaningful
    private float drag;

    public void Start()
	{
        //Motors are set in the editor in a predefined order
        //If you don't care about motor index, use:
        //soloMotors = GetComponentsInChildren<SoloMotor>();

        /*
         * Diagram of motor indexes     
         * 0 - Horizontal motor on left side of sub
         * 1 - Horizontal motor on right side of sub
         * 2 - Vertical motor on front left of sub
         * 3 - Vertical motor on front right of sub
         * 4 - Vertical motor on back left of sub
         * 5 - Vertical motor on back right of sub      
         *        
         *      FRNT
         *   2 /    \ 3
         *  0 |      | 1
         *   4 \____/ 5
         *      BACK
         */


        motors = new Motor[soloMotors.Length];
        for(int i = 0; i < soloMotors.Length; i++){
        	motors[i] = soloMotors[i].motor;
            motors[i].SetGlobalCog(centerOfGravity);
        }

        //Get rigidbody component
        rb = GetComponent<Rigidbody>();
	}

	//Calculate sum of forces and sum of torques
	public void FixedUpdate(){
        //Set the global center of gravity for all the motors
        for (int i = 0; i < soloMotors.Length; i++){
            motors[i].SetGlobalCog(transform.position);
        }

        //Calculate force and torque of motors
        //Only need to recalculate when motor updates
        if (motorChanged){
            UpdateSums(motors, ref force, ref torque);
        }

        //Calculate center of bouyancy and center of gravity righting torque
        //Right now center of gravity and center of bouyancy are just fixed vector3
        //This section of the god is responsible for calculating the "righting torque"
        //of the submarine based on our fixed vector3 values
        if(transform.position.y < waterHeight){
            Vector3 cob1 = centerOfGravity + transform.up, cob2 = centerOfGravity - 0.5f * transform.up;
            Vector3 f1 = Vector3.up * 20, f2 = Vector3.up * 5;
            torque += Vector3.Cross(cob1, f1);
            torque += Vector3.Cross(cob2, f2);
        }

        //After calculating force and torque, apply it to the rigidbody
        rb.AddForce(force);
        rb.AddTorque(torque);
	}

    //Takes in a list of motors, and 2 vector3s representing acceleration and torque
    private void UpdateSums(Motor[] motors, ref Vector3 force, 
                               ref Vector3 torque){
        force = Vector3.zero;
        torque = Vector3.zero;
        for(int i = 0; i < soloMotors.Length; ++i){
            if (soloMotors[i].transform.position.y < waterHeight){
                force += motors[i].GetForce();
                torque += motors[i].GetTorque();
            }
        }
        foreach (Motor m in motors){

        }
        force += Vector3.down * weight;
    }

    private float CalculateDrag(Quaternion quaternion){
        //TODO: WRITE CODE HERE THAT CALCULATES DRAG BASED ON THE ROTATION QUATERNION
        //OF THE SUBMARINE.
        return 1f;
    }

    void OnDrawGizmos(){
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, force/10f+ transform.position);

    }
}
