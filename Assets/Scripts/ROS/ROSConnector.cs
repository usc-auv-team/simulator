﻿using UnityEngine;
using UnityEngine.UI;
using System;
using RosSharp.RosBridgeClient;
using RosSharp.RosBridgeClient.Protocols;
using std_msgs = RosSharp.RosBridgeClient.Messages.Standard;

public class ROSConnector : Singleton<ROSConnector> {
    RosSocket rosSocket = null;
    WebSocketSharpProtocol protocol = null;

    public enum Status { NONE, TRYING, FAILED, SUCCESS };
    public Status status { get; private set; } = Status.NONE;
    [SerializeField] public GameObject inputField = null;
    [SerializeField] public GameObject statusObject = null;
    private Image statusImage = null;

    // Start is called before the first frame update
    private void Start() {
        if (!inputField) {
            Debug.LogError("Failed to read input field.");
        }
        if (!statusObject) {
            Debug.LogError("Failed to read status indicator.");
        }

        statusImage = statusObject.GetComponent<Image>();

        if (!statusImage) {
            Debug.LogError("Failed to get image component of status object.");
        }

        statusImage.color = Color.white;
    }

    // Update is called once per frame
    private void OnDestroy() {
        // Close any existing connection
        if (status == Status.SUCCESS && rosSocket != null) rosSocket.Close();
    }

    // Connect to ROS
    public void Connect() {
        // Close any prior connection
        if (status == Status.SUCCESS) rosSocket.Close();

        string uri = "ws://" + inputField.GetComponent<Text>().text;
        Debug.Log("Attempting connection @ \"" + uri + "\"");
        UpdateStatus(Status.TRYING);

        // Create protocol and attempt connection
        protocol = new WebSocketSharpProtocol(uri);
        protocol.OnConnected += Protocol_OnConnected;  // Setup callback
        protocol.Connect();

        //TODO: If timeout, set status to failed
    }

    // Callback function for when protocol connects
    private void Protocol_OnConnected(object sender, EventArgs e) {
        Debug.Log("Socket connected!");
        
        // If socket connected, create the RosSocket
        rosSocket = new RosSocket(protocol);
        Debug.Log("Created RosSocket");

        UpdateStatus(Status.SUCCESS);
        PublishString("/unity", "Connected to ROS!");
    }

    // Publish string to ROS
    // Topic must be in "/topic" format
    public void PublishString(string topic, string msg) {
        // If not connected, do nothing
        if (status != Status.SUCCESS) return;

        // Create new standard string and set its data to msg
        std_msgs.String message = new std_msgs.String {
            data = msg
        };

        string publicationId = rosSocket.Advertise<std_msgs.String>(topic);
        rosSocket.Publish(publicationId, message);
        Debug.Log("Sent:" + msg);
    }

    // Update status
    private void UpdateStatus(Status status) {
        this.status = status;
    }
}
