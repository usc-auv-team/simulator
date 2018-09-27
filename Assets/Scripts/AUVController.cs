using UnityEngine;
using UnityEngine.UI;
using RosSharp.RosBridgeClient;

public class AUVController : MonoBehaviour {

    //uri should be ROS server IP
    static readonly string uri = "ws://192.168.56.102:9090";

    //RosSocket and publicationId connect unity to ROS
    private RosSocket rosSocket = new RosSocket(uri);
    private string publicationId;
    private StandardString Message;
    //private StandardString Mode;
    private bool manual= false;

    //Unity specific vars
    public Text position;
    Rigidbody rb;
    float speed;

    //Use this for initialization
    void Start() {
        rb = GetComponent<Rigidbody>();
        Debug.Log(GetComponent<MeshFilter>().mesh.bounds);

        //Connect to ROS
        publicationId = rosSocket.Advertise("/message", "std_msgs/String"); // topic, type
        Message = new StandardString();
    }

    //Update is called once per frame
    void Update() {
        updatePositionText();
    }

    void FixedUpdate() {
        //Boolean so rosSocket doesn't keep sending messages on idle
        bool keyPress = false;
        
        //3D Movement
        if (Input.GetKey(KeyCode.W)) {
            //move  forward
            Message.data = "FORWARD";
            keyPress = true;
        }
        if (Input.GetKey(KeyCode.S))
        {
            //move  back
            Message.data = "BACKWARD";
            keyPress = true;
        }
        if (Input.GetKey(KeyCode.A)) {
            //move  left
            Message.data = "LEFT";
            keyPress = true;
        }
        if (Input.GetKey(KeyCode.D)) {
            //move  right
            Message.data = "RIGHT";
            keyPress = true;
        }
        if (Input.GetKey(KeyCode.LeftShift)) {
            //move  down
            Message.data = "DOWN";
            keyPress = true;
        }
        if (Input.GetKey(KeyCode.Space)) {
            //move  up
            Message.data = "UP";
            keyPress = true;
        }

        //Publish [Message] to publicationId
        if (keyPress) rosSocket.Publish(publicationId, Message);
    }

    void toggleMode() {
        StandardString Mode = new StandardString();

        //Connect to ROS
        publicationId = rosSocket.Advertise("/message", "std_msgs/String"); // topic, type

        //Toggle between manual and auto modes
        if (manual){
            //Update button text
            GameObject.Find("ModeSwitch").GetComponentInChildren<Text>().text = "Auto";
            Mode.data = "AUTO";
            //send data to ROS
            rosSocket.Publish(publicationId, Mode);
            manual = !manual;
        }
        else {
            GameObject.Find("ModeSwitch").GetComponentInChildren<Text>().text = "Manual";
            Mode.data = "MANUAL";
            rosSocket.Publish(publicationId, Mode);
            manual = !manual;
        }
        Debug.Log("Mode Toggled");
        Debug.Log(publicationId);
    }

    //Updates the position and velocity information of our AUV
    void updatePositionText() {
        position.text = "AUV position:" +
        "\nx: " + gameObject.transform.position.x.ToString() +
        "\ny: " + gameObject.transform.position.y.ToString() +
        "\nz: " + gameObject.transform.position.z.ToString() +
        "\nAUV velocity:" +
        "\nx: " + rb.velocity.x.ToString() +
        "\ny: " + rb.velocity.y.ToString() +
        "\nz: " + rb.velocity.z.ToString();
    }
}
