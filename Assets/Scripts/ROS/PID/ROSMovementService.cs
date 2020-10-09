using RosSharp.RosBridgeClient.MessageTypes.MotionController;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ROSMovementService : ROSComponent {
    [SerializeField] private PID pid = null;
    private bool initialized = false;
    private List<String> initializedServices;

    // Start is called before the first frame update
    override protected void Start() {
        base.Start();

        if (!pid) {
            Debug.LogError("PhysicsSim not set!");
        }

        initializedServices = new List<string>();
    }

    // Update is called once per frame
    void Update() {
        if (IsEnabled && !initialized) {
            InitServices();        
        }
        else if (!IsEnabled && initialized) {
            TearDownServices();
        }

    }

    public bool SetForwardsPowerCallHandler(SetForwardsPowerRequest request, out SetForwardsPowerResponse response) {
        // Set sub power to some amount
        pid.SetForwardsPower(request.forwardsPower);
        response = new SetForwardsPowerResponse();
        return true;
    }

    private void InitServices() {
        // Set up services
        string setForwardsPowerPublicationId =
            connector.RosSocket.AdvertiseService<SetForwardsPowerRequest, SetForwardsPowerResponse>("/setForwardsPower", SetForwardsPowerCallHandler);
        initializedServices.Add(setForwardsPowerPublicationId);
        initialized = true;
    }

    private void TearDownServices() {
        foreach (String id in initializedServices) {
            connector.RosSocket.UnadvertiseService(id);
        }
        initialized = false;
    }
}
