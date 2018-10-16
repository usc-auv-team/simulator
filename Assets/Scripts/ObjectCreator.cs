using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using SimpleJSON;

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

		//if (isPlaying) {
		//	Progress.value = (Progress.value + 1) % frameCount;
		//}

		//currentFrameNum = (int)Progress.value;

		//foreach (BasicObject obj in gameDatas[currentFrameNum].objects) {
		//	gameObjects[obj.id].transform.position = obj.position;
		//}

  //      DirectoryInfo directory = new DirectoryInfo("C:/Users/Public/Json");
  //      FileInfo myFile = (from f in directory.GetFiles() orderby f.LastWriteTime descending select f).First();

        //string jsonData = File.ReadAllText(myFile.FullName);

        //Parse(myFile.FullName);
    }

	public void togglePlay () {
		isPlaying = !isPlaying;
		if (isPlaying) {
			PlayPause.GetComponentInChildren<Text> ().text = "Pause";
		} else {
			PlayPause.GetComponentInChildren<Text> ().text = "Play";
		}
	}

    DataFrame Parse(string filePath) {
        DataFrame dFrame = new DataFrame();
        List<BasicObject> objects = new List<BasicObject>();
        JSONNode jsonData = JSON.Parse(File.ReadAllText(filePath));

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
        Debug.Log(dFrame.ToString());
        return dFrame;
    }
}