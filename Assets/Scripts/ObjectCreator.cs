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

	//int currentFrameNum;

	GameData[] gameData;

    DataFrame BigFrame;

    // Use this for initialization
    void Start () {
        rosSocket.Subscribe("/listener", "std_msgs/String", subscriptionHandler);
        //string subscription_id = rosSocket.Subscribe("/listener", "std_msgs/String", subscriptionHandler);

        //Parse("Assets/Data/sample.json");

        objectCount = 10; // should read from configuration file later on

		Regex reg = new Regex (@"^[0-9]+\.json$");

		string[] files = Directory.GetFiles (Application.streamingAssetsPath.ToString (), "*");

		for (int i = 0; i < files.Length; i++) {
			files[i] = Path.GetFileName(files[i]);
		}

		files = files.Where(path => reg.IsMatch(path)).ToArray();

		frameCount = files.Length; // same for this one

		//currentFrameNum = 0;
		gameData = new GameData[frameCount];

		for (int i = 0; i < frameCount; i++) {
			string filePath = Path.Combine (Application.streamingAssetsPath, files[i]);
			string dataAsJson = File.ReadAllText (filePath);
			gameData [i] = JsonUtility.FromJson<GameData> (dataAsJson);
		}

		isPlaying = false;
        isLive = false;
		Progress.maxValue = frameCount - 1;
	}

	void Update () {
        //if (isPlaying) {
        //   Progress.value = (Progress.value + 1) % frameCount;
        //}
        //currentFrameNum = (int)Progress.value;

        //foreach (BasicObject obj in gameDatas[currentFrameNum].objects) {
        //   gameObjects[obj.id].transform.position = obj.position;
        //}

        if (!isLive) { // If not live
            DirectoryInfo directory = new DirectoryInfo("Assets/Data");
            int jsonNum = directory.GetFiles().Length;
            FileInfo[] files = directory.GetFiles();
            // FileInfo myFile = (from f in directory.GetFiles() orderby f.LastWriteTime descending select f).First();
            //DataFrame dframe = Parse(JSON.Parse(File.ReadAllText(files[(int)(jsonNum * Progress.value)].FullName)));
            //CreateObjects(dframe.objects);
        } 
        // else { // If live
        //     // Subscribe rosSocket
        //     rosSocket.Subscribe("/listener", "std_msgs/String", subscriptionHandler);
        // }
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
        //if (GameObject.FindGameObjectsWithTag("obstacle") != null) DeleteAll();
        foreach(BasicObject obj in objects) {
            if(obj.id == 0) {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.tag = "obstacle";
                cube.transform.position = obj.position;
                cube.transform.rotation = Quaternion.Euler(0, 0, obj.angle); // Will change will angle is better defined
                Instantiate(cube);
            } else if (obj.id == 1) {
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.tag = "obstacle";
                sphere.transform.position = obj.position;
                sphere.transform.rotation = Quaternion.Euler(0, 0, obj.angle); // Will change will angle is better defined
                Instantiate(sphere);
            } else if (obj.id == 2) {
                GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                cylinder.tag = "obstacle";
                cylinder.transform.position = obj.position;
                cylinder.transform.rotation = Quaternion.Euler(0, 0, obj.angle); // Will change will angle is better defined
                Instantiate(cylinder);
            }
        }
    }

    void DeleteAll() {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("obstacle")) {
            Destroy(obj);
        }
    }

    void subscriptionHandler(Message message) {
        StandardString standardString = (StandardString)message;
        Debug.Log(standardString.data);
        Parse(JSON.Parse(standardString.data));
    }
}
