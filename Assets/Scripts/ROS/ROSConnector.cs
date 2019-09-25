using UnityEngine;
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
        if (status == Status.SUCCESS) rosSocket.Close();
    }

    // Connect to ROS
    public void Connect() {
        // Close any prior connection
        if (status == Status.SUCCESS) rosSocket.Close();

        string uri = "ws://" + inputField.GetComponent<Text>().text;
        //string uri = "ws://192.168.1.195:9090";
        Debug.Log("Attempting connection @ \"" + uri + "\"");
        UpdateStatus(Status.TRYING);

        // Create protocol and attempt connection
        protocol = new WebSocketSharpProtocol(uri);
        protocol.OnConnected += Protocol_OnConnected;  // Setup callback
        protocol.Connect();

        // If timeout set status to failed
    }

    // Callback function for when protocol connects
    private void Protocol_OnConnected(object sender, EventArgs e) {
        Debug.Log("Socket connected!");
        // If socket connected, create the RosSocket
        rosSocket = new RosSocket(protocol);
        PublishString("/listener", "Connected to ROS!");
        UpdateStatus(Status.SUCCESS);
    }

    // Publish string to ROS
    public void PublishString(string topic, string msg) {
        // Create new standard string and set its data to msg
        std_msgs.String message = new std_msgs.String {
            data = msg
        };

        string publicationId = rosSocket.Advertise<std_msgs.String>(topic); // Topic must be in "/topic" format
        rosSocket.Publish(publicationId, message);
        Debug.Log("Sent:" + msg);
    }

    // Update status icon based on status
    private void UpdateStatus(Status status) {
        this.status = status;
        switch(status) {
            case Status.SUCCESS:
                statusImage.color = Color.green;
                break;
            case Status.TRYING:
                statusImage.color = Color.yellow;
                break;
            case Status.FAILED:
                statusImage.color = Color.red;
                break;
        }
    }
}
