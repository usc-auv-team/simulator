using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IMUEmulator : ROSComponent {
    [SerializeField] private string Topic = "/ngimu/euler";
    [SerializeField] private GameObject submarine;
    public Vector3 Rotation { get; private set; }
    public Vector3 Acceleration { get; private set; }
    private Information info;
    private bool passthrough; // Flag for getting information from ROS directly

    // Start is called before the first frame update
    void Start() {
        info = submarine.GetComponent<Information>();
        Rotation = info.rotation;
        Acceleration = info.acceleration;
    }

    // Update is called once per frame
    void Update() {
        Rotation = info.rotation;
        Acceleration = info.acceleration;
    }

    private void FixedUpdate() {
        if(!IsEnabled) {
            return;
        }
        // Publish IMU data
    }
}
