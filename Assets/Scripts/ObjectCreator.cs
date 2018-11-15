using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using SimpleJSON;
using RosSharp.RosBridgeClient;

public class ObjectCreator : MonoBehaviour {

    private RosSocket rosSocket = new RosSocket("ws://192.168.56.102:9090");

    public Button PlayPause;
    public Button LiveSwitch;
	public Slider Progress;

	bool isPlaying;
    bool isLive;

	int frameCount;
	int objectCount;

    int jsonNum;
    FileInfo[] files;

	//int currentFrameNum;

    DataFrame BigFrame;

    // Use this for initialization
    void Start () {
        rosSocket.Subscribe("/listener", "std_msgs/String", subscriptionHandler);
        //string subscription_id = rosSocket.Subscribe("/listener", "std_msgs/String", subscriptionHandler);

		isPlaying = false;
        isLive = false;

        DirectoryInfo directory = new DirectoryInfo("Assets/Data");
        jsonNum = directory.GetFiles().Length;
        files = directory.GetFiles();

        Progress.maxValue = jsonNum - 1;
	}

	void Update () {
        if (isPlaying && !isLive) {
           Progress.value = (Progress.value + 1) % jsonNum;
           DataFrame dframe = Parse(JSON.Parse(File.ReadAllText(files[(int)(Progress.value)].FullName)));
           CreateObjects(dframe.objects);
        }
    }


    DataFrame Parse(JSONNode jsonData) {
        DataFrame dFrame = new DataFrame();
        List<BasicObject> objects = new List<BasicObject>();
        // JSONNode jsonData = JSON.Parse(File.ReadAllText(filePath));

        //Main DataFrame components
        dFrame.timeStamp = jsonData["ts"];
        dFrame.position.Set(jsonData["x"], jsonData["y"], jsonData["z"]);
        dFrame.acceleration.Set(jsonData["ax"], jsonData["ay"], jsonData["az"]);
        dFrame.speed.Set(jsonData["ux"], jsonData["uy"], jsonData["uz"]);
        dFrame.depth = jsonData["d"];
        dFrame.rotation = jsonData["th"];

        //Objects
        for(int i = 0; i < jsonData["objects"].Count; i++) {
            JSONNode currObj = jsonData["objects"][i];
            BasicObject basicObj = new BasicObject {
                id = currObj["id"],
                angle = currObj["th"],
                probability = currObj["p"]
            };
            basicObj.position.Set(currObj["x"], currObj["y"], currObj["z"]);

            objects.Add(basicObj);
            Debug.Log(basicObj.ToString());
        }
        dFrame.objects = objects;
        Debug.Log(dFrame.ToString());
        return dFrame;
    }

	public void togglePlay () {
        if (isPlaying) {
            PlayPause.GetComponentInChildren<Text>().text = "Pause";
        } 
        else {
            PlayPause.GetComponentInChildren<Text>().text = "Play";
        }
		isPlaying = !isPlaying;
	}

    public void toggleLive() {
        if (isLive) {
            GameObject.Find("LiveSwitch").GetComponentInChildren<Text>().text = "Logged";
        } 
        else {
            GameObject.Find("LiveSwitch").GetComponentInChildren<Text>().text = "Live";
        }
        isLive = !isLive;
    }

    // At some point we'll make the code perty
    void CreateObjects(List<BasicObject> objects) {
        if (GameObject.FindGameObjectsWithTag("obstacle") != null) DeleteAll();
        foreach(BasicObject obj in objects) {
            if(obj.id == 0) {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.tag = "obstacle";
                cube.transform.position = obj.position;
                cube.transform.rotation = Quaternion.Euler(0, 0, obj.angle); // Will change will angle is better defined
                cube.GetComponent<Renderer>().material.color = GetColor(obj.probability);
                Instantiate(cube);
            } else if (obj.id == 1) {
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.tag = "obstacle";
                sphere.transform.position = obj.position;
                sphere.transform.rotation = Quaternion.Euler(0, 0, obj.angle); // Will change will angle is better defined
                sphere.GetComponent<Renderer>().material.color = GetColor(obj.probability);
                Instantiate(sphere);
            } else if (obj.id == 2) {
                GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                cylinder.tag = "obstacle";
                cylinder.transform.position = obj.position;
                cylinder.transform.rotation = Quaternion.Euler(0, 0, obj.angle); // Will change will angle is better defined
                cylinder.GetComponent<Renderer>().material.color = GetColor(obj.probability);
                Instantiate(cylinder);
            }
        }
    }

    void DeleteAll() {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("obstacle")) {
            Destroy(obj);
        }
    }

    Color GetColor(double probability) {
        if (probability >= 0 && probability < 25) {
            return Color.red;
        } else if (probability >= 25 && probability < 50) {
            return Color.yellow;
        } else if (probability >= 50 && probability < 75) {
            return Color.green;
        } else if (probability >= 75 && probability <= 100) {
            return Color.blue;
        } else {
            return Color.black;
        }
    }

    void subscriptionHandler(Message message) {
        StandardString standardString = (StandardString)message;
        Debug.Log(standardString.data);
        Parse(JSON.Parse(standardString.data));
    }
}
