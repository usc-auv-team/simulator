using RosSharp.RosBridgeClient.MessageTypes.MotionController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PID: MonoBehaviour {
    [SerializeField] private PhysicsSim physicsSim = null;
    [SerializeField] private IMUEmulator imu = null;

    // Desired points
    float desiredRoll = 0;
    float desiredPitch = 0;
    float desiredYaw = 0;
    float desiredDepth = 0;

    // Last data recieved from sensors
    float sensorRoll = 0;
    float sensorPitch = 0;
    float sensorYaw = 0;
    float sensorDepth = 0;

    // If false, all outputs are disabled and no data gets fed into PID controller
    public bool Status { get; set; } = false;

    void Start() {
        if (!imu) {
            Debug.LogError("IMU not set!");
        }
    }

    private void Update() {
        // If disabled, don't do anything
        if (!Status) return;

        UpdateSensors();
    }

    private void UpdateSensors() {
        sensorPitch = imu.Rotation.x;
        sensorYaw = imu.Rotation.y;
        sensorRoll = imu.Rotation.z;
    }

    public void SetForwardsPower(float power) {
        // Set sub power to some amount
        physicsSim.soloMotors[0].SetMotorPower(power);
        physicsSim.soloMotors[1].SetMotorPower(power);
        Debug.Log("Forwards Power set to: " + power);
    }

}