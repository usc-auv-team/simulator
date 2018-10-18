using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataFrame
{
    public int timeStamp;

    public List<BasicObject> objects; //data of external objectss

    //vehicle data
    //position
    public Vector3 position;
    //acceleration
    public Vector3 acceleration;
    //velocity
    public Vector3 speed;
    //depth
    public double depth;
    //rotation
    public double rotation;

    public override string ToString()
    {
        string temp =
            "Timestamp:" + timeStamp + "\n" +
            "Position:(" + position.x + "," + position.y + "," + position.z + ")" + "\n" +
            "Accelaration:(" + acceleration.x + "," + acceleration.y + "," + acceleration.z + ")" + "\n" +
            "Speed:(" + speed.x + "," + speed.y + "," + speed.z + ")" + "\n" +
            "Depth:" + depth + "\n" +
            "Rotation" + rotation;
        return temp;
    }
}