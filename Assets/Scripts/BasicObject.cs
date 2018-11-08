using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BasicObject {
	public int id;
	public int type;
    public double probability;
	public float angle;
	public Vector3 position;

    public override string ToString() {
        string temp =
            "Id:" + id + "\n" +
            "Type:" + type + "\n" +
            "Position:(" + position.x + "," + position.y + "," + position.z + ")" + "\n" +
            "Angle:" + angle + "\n" +
            "Probability:" + probability;
        return temp;
    }
}
