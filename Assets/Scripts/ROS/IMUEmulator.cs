using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IMUEmulator : ROSComponent {
    [SerializeField] private string topic = "/ngimu/euler";

    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {

    }

    private void FixedUpdate() {
        if(!IsEnabled) {
            return;
        }

        // Publish IMU data
    }
}
