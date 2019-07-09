using RosSharp.RosBridgeClient;
using RosSharp.RosBridgeClient.Protocols;
using std_msgs = RosSharp.RosBridgeClient.Messages.Standard;
using UnityEngine;

public class ROSConnector : MonoBehaviour {

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    readonly static string uriBase = "ws://";
    IProtocol protocol;
    RosSocket rosSocket;

    // Connect to ROS
    void Connect(string ip) {
        string uri = uriBase + ip;
        protocol = new WebSocketSharpProtocol(uri);
        if (protocol.IsAlive()) {
            rosSocket = new RosSocket(protocol);
            PublishString("/simulator", "Connected to ROS!");
        }
        else {
            // Bad shit
        }
    }

    // Publish string to ROS
    void PublishString(string topic, string msg) {
        // Create new standard string and set its data to msg
        std_msgs.String message = new std_msgs.String {
            data = msg
        };

        string publicationId = rosSocket.Advertise<std_msgs.String>(topic); // Topic must be in "/topic" format
        rosSocket.Publish(publicationId, message);
        Debug.Log("Sent:" + msg);
    }
}
