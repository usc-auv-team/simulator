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
}