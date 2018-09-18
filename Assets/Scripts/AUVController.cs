using UnityEngine;
using UnityEngine.UI;
using RosSharp.RosBridgeClient;

public class AUVController : MonoBehaviour {

    public Text position;

    Rigidbody rb;
    float speed;
    static readonly string uri = "ws://192.168.56.101:9090";
    private RosSocket rosSocket;
    private StandardString Message;
    private string publicationId;
    
    // Use this for initialization
    void Start() {
        speed = 5.0f;
        rb = GetComponent<Rigidbody>();
        Debug.Log(GetComponent<MeshFilter>().mesh.bounds);
        rosSocket = new RosSocket(uri);
        publicationId = rosSocket.Advertise("/message", "std_msgs/String")
        Message = new StandardString();
    }

    // Update is called once per frame
    void Update() {
        updatePositionText();
    }

    void FixedUpdate() {
        
        //Basic Movement
        if (Input.GetKey(KeyCode.W))
        {
            //move  forward
            Message.data = "FORWARD";
        }
        if (Input.GetKey(KeyCode.A))
        {
            //move  left
            Message.data = "LEFT";
        }
        if (Input.GetKey(KeyCode.S))
        {
            //move  back
            Message.data = "BACKWARD";
        }
        if (Input.GetKey(KeyCode.D))
        {
            //move  right
            Message.data = "RIGHT";
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            //move  down
            Message.data = "DOWN";
        }
        if (Input.GetKey(KeyCode.Space))
        {
            //move  up
            Message.data = "UP";
        }

        rosSocket.Publish(publicationId, Message);
    }

    // Updates the position and velocity information of our AUV
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
