using UnityEngine;
using UnityEngine.UI;
using RosSharp.RosBridgeClient;

[RequireComponent(typeof(RosConnector))]
public class AUVController : MonoBehaviour {

	public Text position;

	Rigidbody rb;
	float speed;
    //static readonly string uri = "ws://192.168.70.129:9090";
    private RosSocket rosSocket;
    public string topic = "/message";
    private StandardString Message;
    private string publicationId;


    // Use this for initialization
    void Start () {
		speed = 5.0f;
		rb = GetComponent<Rigidbody> ();
		Debug.Log (GetComponent<MeshFilter>().mesh.bounds);
        rosSocket = GetComponent<RosConnector>().RosSocket;
        publicationId = rosSocket.Advertise(topic, "std_msgs/String");
        Message = new StandardString();
    }
	
	// Update is called once per frame
	void Update () {
		updatePositionText ();
        Vector3 values = new Vector3(Random.Range(0, 1000.0f), Random.Range(0, 1000.0f), Random.Range(0, 1000.0f));
        Message.data = values.ToString();
        rosSocket.Publish(publicationId, Message);
    }

    void FixedUpdate()
    {
        //Basic Movement
        if (Input.GetKey(KeyCode.W))
        {
            //move  up
            Message.data = "W";
        }
        else if (Input.GetKey(KeyCode.A))
        {
            //move  left
            Message.data = "A";
        }
        else if (Input.GetKey(KeyCode.S))
        {
            //move  down
            Message.data = "S";
        }
        else if (Input.GetKey(KeyCode.D))
        {
            //move  right
            Message.data = "D";
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            //move  down
            Message.data = "LShift";
        }
        else if (Input.GetKey(KeyCode.Space))
        {
            //move  up
            Message.data = "Space";
        }

        // Publish the message
        rosSocket.Publish(publicationId, Message);
    }

    // Updates the position and velocity information of our AUV
    void updatePositionText() {
		position.text = "AUV position:" +
		"\nx: " + gameObject.transform.position.x.ToString () +
		"\ny: " + gameObject.transform.position.y.ToString () +
		"\nz: " + gameObject.transform.position.z.ToString () +
		"\nAUV velocity:" +
		"\nx: " + rb.velocity.x.ToString () +
		"\ny: " + rb.velocity.y.ToString () +
		"\nz: " + rb.velocity.z.ToString ();
	}
}
