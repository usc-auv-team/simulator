using UnityEngine;
using UnityEngine.UI;
using RosSharp.RosBridgeClient;

public class AUVController : MonoBehaviour {
    //private StandardString Mode;
    private bool manual;

    //Unity specific vars
    public Text position;
    Rigidbody rb;
    float speed;

    //Use this for initialization
    void Start() {
        manual = true;
        rb = GetComponent<Rigidbody>();
        Debug.Log(GetComponent<MeshFilter>().mesh.bounds);
    }

    //Update is called once per frame
    void Update() {
        updatePositionText();
    }

    void FixedUpdate() {
        //Boolean so rosSocket doesn't keep sending messages on idle
        string msg = "";
        bool keyPress = false;
        
        //3D Movement
        if (Input.GetKey(KeyCode.W)) {
            // Move  forward
            msg = "FORWARD";
            keyPress = true;
        }
        if (Input.GetKey(KeyCode.S))
        {
            // Move  back
            msg = "BACKWARDS";
            keyPress = true;
        }
        if (Input.GetKey(KeyCode.A)) {
            // Move  left
            msg = "LEFT";
            keyPress = true;
        }
        if (Input.GetKey(KeyCode.D)) {
            // Move  right
            msg = "RIGHT";
            keyPress = true;
        }
        if (Input.GetKey(KeyCode.LeftShift)) {
            // Move  down
            msg = "DOWN";
            keyPress = true;
        }
        if (Input.GetKey(KeyCode.Space)) {
            // Move  up
            msg = "UP";
            keyPress = true;
        }

        //Publish message to ROS
        if (keyPress && manual) ROSConnector.SendRosMessage(msg);
        //todo this doesn't play nice with mode toggle. fix eventually. kinda important!
    }

    void toggleMode() {
        //Toggle between manual and auto modes
        if (manual){
            //Update button text
            GameObject.Find("RemoteSwitch").GetComponentInChildren<Text>().text = "Auto";
            ROSConnector.SendRosMessage("AUTO");
            manual = !manual;
        }
        else {
            GameObject.Find("RemoteSwitch").GetComponentInChildren<Text>().text = "Manual";
            ROSConnector.SendRosMessage("MANUAL");
            manual = !manual;
        }
        Debug.Log("Mode Toggled:" + manual);
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
