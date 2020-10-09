using UnityEngine;
using UnityEngine.UI;
using System;
using RosSharp.RosBridgeClient;
using RosSharp.RosBridgeClient.Protocols;
using std_msgs = RosSharp.RosBridgeClient.MessageTypes.Std;
using System.Collections;
using geometry_msgs = RosSharp.RosBridgeClient.MessageTypes.Geometry;

public class ROSConnector : Singleton<ROSConnector> {
    private RosSocket rosSocket = null;
    public RosSocket RosSocket { get => rosSocket; }
    WebSocketSharpProtocol protocol = null;

    public enum Status { NONE, TRYING, FAILED, SUCCESS };
    public Status status { get; private set; } = Status.NONE;

    [SerializeField] public GameObject inputField = null;
    [SerializeField] public GameObject statusObject = null;
    private Image statusImage = null;
    private Coroutine coroutineTimeout = null;
    private bool statusChange = false;

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

    private IEnumerator ConnectTimeout(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        Debug.Log("Running timeout coroutine.");
        if (!protocol.IsAlive()) {
            UpdateStatus(Status.FAILED);
        }
    }

    // Connect to ROS
    public void Connect() {
        // Close any prior connection

        if (coroutineTimeout != null) {
            StopCoroutine(coroutineTimeout);
        }

        if (status == Status.SUCCESS) rosSocket.Close();

        string uri = "ws://" + inputField.GetComponent<Text>().text;
        Debug.Log("Attempting connection @ \"" + uri + "\"");
        UpdateStatus(Status.TRYING);

        // Create protocol and attempt connection
        protocol = new WebSocketSharpProtocol(uri);
        protocol.OnConnected += Protocol_OnConnected;  // Setup callback
        protocol.Connect();

        // If timeout, set status to failed
        // Use coroutine to increment delayed timer, end of 
        // timer should then check the status of protocol
        coroutineTimeout = StartCoroutine(ConnectTimeout(5.0f));
    }

    // Callback function for when protocol connects
    private void Protocol_OnConnected(object sender, EventArgs e) {
        Debug.Log("Socket connected!");

        UpdateStatus(Status.SUCCESS);

        // If socket connected, create the RosSocket
        rosSocket = new RosSocket(protocol);
        Debug.Log("Created RosSocket");
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

    public void PublishVector3(string topic, Vector3 vector3) {
        // If not connected, do nothing
        if (status != Status.SUCCESS) return;

        geometry_msgs.Vector3Stamped message = new geometry_msgs.Vector3Stamped {
            vector = new geometry_msgs.Vector3 {
                x = vector3.x,
                y = vector3.y,
                z = vector3.z
            },
            header = new std_msgs.Header {
                frame_id = "1",
                seq = 1,
                stamp = new std_msgs.Time()
            }
        };

        string publicationId = rosSocket.Advertise<geometry_msgs.Vector3>(topic);
        rosSocket.Publish(publicationId, message);
        Debug.Log("Sent:" + vector3);
    }

    // Update status
    private void UpdateStatus(Status inStatus) {
        statusChange = true;
        status = inStatus;
    }

    void LateUpdate() {
        if (statusChange) {
            switch (status) {
                case Status.SUCCESS:
                    Debug.Log("Connected, changing color to green.");
                    statusImage.color = Color.green;
                    statusChange = false;
                    break;
                case Status.TRYING:
                    Debug.Log("Attempting to connect, changing color to yellow.");
                    statusImage.color = Color.yellow;
                    statusChange = false;
                    break;
                case Status.FAILED:
                    Debug.Log("Failed to connect, changing color to red.");
                    statusImage.color = Color.red;
                    statusChange = false;
                    break;
            }
        }
    }

}
