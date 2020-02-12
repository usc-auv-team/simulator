using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour {
    [SerializeField] public GameObject HUD;
    [SerializeField] private GameObject submarine;
    [SerializeField] private IMUEmulator imu;
    private Information info;
    private Text IMUData;

    // Start is called before the first frame update
    void Start() {
        //Gives a reference to the text box component in the HUD
        IMUData = HUD.GetComponent<Text>();
        if (!IMUData) {
            Debug.LogError("Text component on HUD not set!");
        }

        // Gives a reference to the information component in the sub
        info = submarine.GetComponent<Information>();

        if (!info) {
            Debug.LogError("Information component on sub not set!");
        }
    }

    // Update is called once per frame
    void Update() {

        if (info && IMUData) {
            String x = imu.Acceleration.x.ToString("+0.00;-0.00");
            String y = imu.Acceleration.y.ToString("+0.00;-0.00");
            String z = imu.Acceleration.z.ToString("+0.00;-0.00");
            String pitch = imu.Rotation.x.ToString("+0.00;-0.00");
            String yaw = imu.Rotation.y.ToString("+0.00;-0.00");
            String roll = imu.Rotation.z.ToString("+0.00;-0.00");

            String IMUText =
                $"Acceleration\n" +
                $"x: {x}\n" +
                $"y: {y}\n" +
                $"z: {z}\n\n" +
                $"Pitch {pitch}\n" +
                $"Yaw: {yaw}\n" +
                $"Roll: {roll}";
            IMUData.text = IMUText;

        }

    }
}
