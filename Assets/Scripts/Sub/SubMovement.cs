using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The <c>SubMovement</c> class handles the movement on the submarine and the 
/// broadcasting of those movements over ROS
/// </summary>
public class SubMovement : MonoBehaviour {
    [SerializeField] private PhysicsSim physicsSim = null;
    [SerializeField] private InputManager keyboardInput = null;
    [SerializeField] private InputManager controllerInput = null;
    private Vector3 direction;

    private void Start() {
        // KeyboardInput must be present on startup
        if (!physicsSim) {
            Debug.LogError("PhysicsSim not set!!");
        }

        // KeyboardInput must be present on startup
        if (!keyboardInput) {
            Debug.LogError("KeyboardInput not set!!");
        }
    }

    private void Update() {
        GetDirection();
        MoveSub();
    }

    private void FixedUpdate() {

    }

    // ============================================================
    // Private class methods

    /// <summary>
    /// This method gets the direction from all input sources and merges them into one
    /// </summary>
    private void GetDirection() {
        // Only KeyboardInput currently exists
        if (controllerInput.Direction == Vector3.zero) {
            direction = keyboardInput.Direction;
        }
        if ((controllerInput.Direction != Vector3.zero)) {
            direction = controllerInput.Direction;
        }
    }

    private void MoveSub() {
        if(direction.y != 0) {
            MoveUpAxis(direction.y);
        }else if(direction.y == 0) {
            MoveUpAxis(0);
        }
        if (direction.x != 0) {
            RotateLeft(direction.x);
            RotateRight(direction.x);
        }else if(direction.x == 0) {
            RotateLeft(0);
            RotateRight(0);
        }
        if (direction.z != 0) {
            MoveForwardAxis(direction.z);
        }
    }

    private void MoveForwardAxis(float power) {
        //<1,1>
        //<-1,-1>
        //// Motor 1 str = distance from y = x
        //// Motor 0 str = distance from y = -x
        physicsSim.soloMotors[0].SetMotorPower(power);
        physicsSim.soloMotors[1].SetMotorPower(power);

    }

    private void RotateLeft(float power) {
        physicsSim.soloMotors[0].SetMotorPower(-power);
        physicsSim.soloMotors[1].SetMotorPower(power);
    }

    private void RotateRight(float power) {
        physicsSim.soloMotors[0].SetMotorPower(power);
        physicsSim.soloMotors[1].SetMotorPower(-power);
    }

    private void MoveUpAxis(float power) {
        physicsSim.soloMotors[2].SetMotorPower(power);
        physicsSim.soloMotors[3].SetMotorPower(power);
        physicsSim.soloMotors[4].SetMotorPower(power);
        physicsSim.soloMotors[5].SetMotorPower(power);
    }

    private void BroadcastMovement() {

    }
}
