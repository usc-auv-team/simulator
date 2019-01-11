using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using SimpleJSON;
using RosSharp.RosBridgeClient;

class ROSConnector : MonoBehaviour {
    // RosSocket and publicationId connect unity to ROS
    static readonly string uri = "ws://192.168.56.102:9090"; // IP of ROS machine
    public static RosSocket rosSocket = new RosSocket(uri);
    public static string publicationId;
       
    private static StandardString Message; // Message to be sent via ROS

    private void Start() {
        Message = new StandardString();
        publicationId = rosSocket.Advertise("/unity", "std_msgs/String"); // topic, type
        SendRosMessage("Connected to ROS!"); // Confirmation message to ROS
    }

    public static void SendRosMessage(string msg) {
        Message.data = msg;
        rosSocket.Publish(publicationId, Message);
        Debug.Log("Sent:" + msg);
        Message.data = "";
    }
}

