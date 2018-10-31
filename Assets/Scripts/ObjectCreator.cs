﻿using System.Collections;
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
	public Slider Progress;

	bool isPlaying;
    bool isLive;

	int frameCount;
	int objectCount;

	int currentFrameNum;

	GameData[] gameDatas;
	GameObject[] gameObjects;

    DataFrame BigFrame;

    // Use this for initialization
    void Start () {
        //string subscription_id = rosSocket.Subscribe("/listener", "std_msgs/String", subscriptionHandler);

        Parse("Assets/Data/sample.json");

        objectCount = 10; // should read from configuration file later on

		Regex reg = new Regex (@"^[0-9]+\.json$");

		string[] files = Directory.GetFiles (Application.streamingAssetsPath.ToString (), "*");

		for (int i = 0; i < files.Length; i++) {
			files[i] = Path.GetFileName(files[i]);
		}

		files = files.Where(path => reg.IsMatch(path)).ToArray();

		frameCount = files.Length; // same for this one

		currentFrameNum = 0;
		gameDatas = new GameData[frameCount];
		gameObjects = new GameObject[objectCount];

		for (int i = 0; i < frameCount; i++) {
			string filePath = Path.Combine (Application.streamingAssetsPath, files[i]);
			string dataAsJson = File.ReadAllText (filePath);
			gameDatas [i] = JsonUtility.FromJson<GameData> (dataAsJson);
		}

		isPlaying = false;
		Progress.maxValue = frameCount - 1;
	}

	void Update () {
        if (isPlaying) {
           Progress.value = (Progress.value + 1) % frameCount;
        }

        currentFrameNum = (int)Progress.value;

        //foreach (BasicObject obj in gameDatas[currentFrameNum].objects) {
        //   gameObjects[obj.id].transform.position = obj.position;
        //}

        if (!isLive) {
            DirectoryInfo directory = new DirectoryInfo("C:/Users/Public/Json");
            int jsonNum = directory.GetFiles().Length;
            FileInfo[] files = directory.GetFiles();
            // FileInfo myFile = (from f in directory.GetFiles() orderby f.LastWriteTime descending select f).First();
            dframe = Parse(JSON.Parse(File.ReadAllText(files[(int)(jsonNum * Progress.value)].FullName)));
            CreateObjects(drame.objects);
        } else if (isLive) {
            // Subscribe rosSocket
            rosSocket.Subscribe("/listener", "std_msgs/String", subscriptionHandler);
        }
    }

	public void togglePlay () {
		isPlaying = !isPlaying;
		if (isPlaying) {
			PlayPause.GetComponentInChildren<Text> ().text = "Pause";
		} else {
			PlayPause.GetComponentInChildren<Text> ().text = "Play";
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
            BasicObject basicObj = new BasicObject();
            basicObj.id = currObj["id"];
            basicObj.angle = currObj["th"];
            basicObj.probability = currObj["p"];
            basicObj.position.Set(currObj["x"], currObj["y"], currObj["z"]);

            objects.Add(basicObj);
            Debug.Log(basicObj.ToString());
        }
        dFrame.objects = objects;
        Debug.Log(dFrame.ToString());
        return dFrame;
    }

    void toggleLive() {
        if (isLive) {
            isLive = false;
            GameObject.Find("ModeSwitch").GetComponentInChildren<Text>().text = "Logged";
        } else if (!isLive) {
            isLive = true;
            GameObject.Find("ModeSwitch").GetComponentInChildren<Text>().text = "Live";
        }
    }
    // At some point we'll make the code perty

    void subscriptionHandler(Message message) {
        StandardString standardString = (StandardString)message;
        Debug.Log(standardString.data);
        Parse(JSON.Parse(standardString.data));
    }

    void CreateObjects(List<BasicObject> obj)
    {
        foreach(BasicObject objects in obj) {
            if(obj.id == 0) {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = obj.position;
                cube.transform.orientation = obj.angle;
                Instantiate(cube);
            } else if (obj.id == 1) {
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.position = obj.position;
                sphere.transform.orientation = obj.angle;
                Instantiate(sphere);
            } else if (obj.id == 2) {
                GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                cylinder.transform.position = obj.psoition;
                cylinder.transform.orientation = obj.angle;
                Instatiate(cylinder);
            }
        }
    }
}