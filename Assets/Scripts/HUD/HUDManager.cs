using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour {
    [SerializeField] public GameObject HUD;
    [SerializeField] private GameObject submarine;
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
            String IMUText =
                $"Acceleration\n" +
                $"x: {info.acceleration.x}\n" +
                $"y: {info.acceleration.y}\n" +
                $"z: {info.acceleration.z}\n\n" +
                $"Pitch {info.rotation.x}\n" +
                $"Yaw: {info.rotation.y}\n" +
                $"Roll: {info.rotation.z}";
            IMUData.text = IMUText;

        }

    }
}
