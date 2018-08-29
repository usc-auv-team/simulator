using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

public class ObjectCreator : MonoBehaviour {

	public Button PlayPause;
	public Slider Progress;

	bool isPlaying;

	int frameCount;
	int objectCount;

	int currentFrameNum;

	GameData[] gameDatas;
	GameObject[] gameObjects;

	// Use this for initialization
	void Start () {


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

		foreach (BasicObject obj in gameDatas[currentFrameNum].objects) {
			PrimitiveType type = PrimitiveType.Cube;
			switch (obj.type) {
			case 0:
				type = PrimitiveType.Cube;
				break;
			case 1:
				type = PrimitiveType.Sphere;
				break;
			case 2:
				type = PrimitiveType.Cylinder;
				break;
			}
			gameObjects[obj.id] = GameObject.CreatePrimitive (type);
			gameObjects[obj.id].transform.position = obj.position;
			gameObjects[obj.id].transform.eulerAngles = obj.orientation;
		}


		isPlaying = false;
		Progress.maxValue = frameCount - 1;


//			objs = loadedData.objects;
//			for (int i = 0; i < objs.Length; i++) {
//				Debug.Log ("object N.O.: " + i);
//				Debug.Log ("id: " + objs [i].id);
//				Debug.Log ("type: " + objs [i].type);
//				Debug.Log ("position: " + objs [i].position);
//				Debug.Log ("orientation: " + objs [i].orientation);
//				Debug.Log ("linear_velocity: " + objs [i].linear_velocity);
//				Debug.Log ("angular_velocity: " + objs [i].angular_velocity);
//				Debug.Log ("linear_acceleration: " + objs [i].linear_acceleration);
//				Debug.Log ("angular_acceleration: " + objs [i].angular_acceleration);
//
//				PrimitiveType type = PrimitiveType.Cube;
//				switch (objs [i].type) {
//				case 0:
//					type = PrimitiveType.Cube;
//					break;
//				case 1:
//					type = PrimitiveType.Sphere;
//					break;
//				}
//
//				GameObject instance = GameObject.CreatePrimitive (type);
//				instance.transform.position = objs [i].position;
//				instance.transform.eulerAngles = objs [i].orientation;
//
//			}
	}

	void Update () {

		if (isPlaying) {
			Progress.value = (Progress.value + 1) % frameCount;
		}

		currentFrameNum = (int)Progress.value;

		foreach (BasicObject obj in gameDatas[currentFrameNum].objects) {
			gameObjects[obj.id].transform.position = obj.position;
			gameObjects[obj.id].transform.eulerAngles = obj.orientation;
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

}
